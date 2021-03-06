﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;
using vyatta_config_updater.VyattaConfig;

namespace vyatta_config_updater
{
	public class RouterEnableDNSCrypt : BusyWorkInterface
	{
		private RouterData Data;
		private Form Parent;

		public RouterEnableDNSCrypt( RouterData Data, Form Parent )
		{
			this.Data = Data;
			this.Parent = Parent;
		}

		bool ChooseResolver( string Input, ShellStream Stream )
		{
			//NOTE: We could pick a resolver here, but we need to parse the file again later
			//anyway so we'll do it then. I'm also a little nervous about leaving the SSH
			//connection open inside an ExpectAction.

			Stream.WriteLine( "" );
			return false;
		}

		static public void KillDNSCrypt( VyattaShell Shell )
		{
			string RunningDNSCrypt = Shell.RunCommand( "sudo ps ax | grep '[d]nscrypt-proxy'" );

			var MatchDNSCryptProcess = new Regex( @"\W+(\d+)\W+\w+\W+\d+:\d+\W+dnscrypt-proxy.*" );

			var MatchDNSCryptProcessResult = MatchDNSCryptProcess.Match( RunningDNSCrypt );
			if( MatchDNSCryptProcessResult.Success )
			{
				//DNSCrypt is currently running

				Shell.RunCommand( string.Format( "sudo kill {0}", MatchDNSCryptProcessResult.Groups[1].Value ) );
			}
		}

		public bool FreeUpPreviousImage( VyattaShell Shell, string CommandResult )
		{
			if( CommandResult.Contains( "No space left on device") || CommandResult.Contains( "Not found") || CommandResult.Contains( "not found") || CommandResult.Contains( "Some index files failed to download" ) )
			{
				if( MessageBox.Show( "You've run out of disk space on your device,\ndo you want to free up some space by\ndeleting the previous system image?", "Out of disk space!", MessageBoxButtons.YesNoCancel ) == DialogResult.Yes )
				{
					Shell.RunCommand( "sudo delete system image" );
					Shell.RunCommand( "sudo rm -rf /root.dev/w.*" );
					Shell.RunCommand( "sudo apt-get autoclean" );

					return true;
				}
			}

			return false;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			SetStatus( "Connecting to SSH...", 0 );

			using( VyattaShell Shell = new VyattaShell( Data.Address, Data.Username, Data.Password ) )
			{
				SetStatus( "Checking for wget...", 5 );

				string DoesWgetExist = Shell.RunCommand( "wget" );
				if( DoesWgetExist.Contains( "command not found" ) )
				{
					SetStatus( "Adding main debian sources...", 10 );
				
					Shell.RunCommand( "configure" );
					Shell.RunCommand( "set system package repository wheezy components 'main contrib non-free'" );
					Shell.RunCommand( "set system package repository wheezy distribution wheezy" );
					Shell.RunCommand( "set system package repository wheezy url http://http.us.debian.org/debian" );

					SetStatus( "Committing changes...", 10 );
					Shell.RunCommand( "commit" );
					Shell.RunCommand( "save" );
					Shell.RunCommand( "exit" );

					var WgetResult = Shell.RunCommand( "sudo apt-get install wget" );
					if(		WgetResult.Contains( "Package 'wget' has no installation candidate" )
						||	WgetResult.Contains( "Unable to locate package wget" ) )
					{
						Prev:
						SetStatus( "Updating package lists...", 15 );
						var UpdateResult = Shell.RunCommand( "sudo apt-get update" );
						if( FreeUpPreviousImage( Shell, UpdateResult ) )
						{
							goto Prev;
						}
						if( UpdateResult.Contains( "Not Found" ) || UpdateResult.Contains( "not Found" ) )
						{
							string[] Lines = UpdateResult.Split( new [] { '\n' }, StringSplitOptions.RemoveEmptyEntries );
							int Max5Lines = Lines.Length - 5;
							string LastUpdateBlock = string.Join( "\n", Lines, Max5Lines, Lines.Length - Math.Max( Max5Lines, 0 ) );

							throw new Exception( "One or more packages was not found:\n" + LastUpdateBlock );
						}

						Prev2:
						SetStatus( "Installing wget...", 20 );
						WgetResult = Shell.RunCommand( "sudo apt-get install wget" );
						if(		WgetResult.Contains( "Package 'wget' has no installation candidate" )
							||	WgetResult.Contains( "Unable to locate package wget" ) )
						{
							string[] Lines = UpdateResult.Split( new [] { '\n' }, StringSplitOptions.RemoveEmptyEntries );
							int Max5Lines = Lines.Length - 5;
							string LastUpdateBlock = string.Join( "\n", Lines, Max5Lines, Lines.Length - Math.Max( Max5Lines, 0 ) );

							throw new Exception( "Unable to install wget:\n" + LastUpdateBlock + WgetResult );
						}
						if( FreeUpPreviousImage( Shell, UpdateResult ) )
						{
							goto Prev2;
						}

						SetStatus( "Checking wget...", 25 );
						string RecheckWget = Shell.RunCommand( "wget" );

						if( !RecheckWget.Contains( "wget: missing URL" ) )
						{
							throw new Exception( "wget not found after installation." );
						}
					}
				}
				else if( !DoesWgetExist.Contains( "wget: missing URL" ) )
				{
					throw new Exception( "Unknown state - could not determine if wget exists." );
				}

				if( ShouldCancel() ) { return false; }

				SetStatus( "Checking for Entware...", 30 );

				string DoesOpkgExist = Shell.RunCommand( "/opt/bin/opkg" );
				
				if( DoesOpkgExist.Contains( "No such file or directory" ) )
				{
					SetStatus( "Installing Entware...", 32 );

					string GetInstaller = Shell.RunCommand( "wget -O - https://pkg.entware.net/binaries/mipsel/installer/installer.sh | sudo sh" );
					if( GetInstaller.Contains("Connection refused") )
					{
						if( MessageBox.Show("The connection to the Entware package server was refused over https.\nDo you want to try again over http? This might indicate a problem on their server or an attempt to interfere with your installation.", "Try again over an insecure connection?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes )
						{
							Shell.RunCommand( "wget -O - http://pkg.entware.net/binaries/mipsel/installer/installer.sh | sudo sh" );
						}
						else
						{
							return false;
						}
					}
					
					SetStatus( "Checking for Entware...", 60 );

					DoesOpkgExist = Shell.RunCommand( "/opt/bin/opkg" );
					if( !DoesOpkgExist.Contains( "usage: opkg" ) )
					{
						throw new Exception( "Unknown state - could not determine if opkg exists." );
					}
				}
				else if( !DoesOpkgExist.Contains( "usage: opkg" ) )
				{
					throw new Exception( "Unknown state - could not determine if opkg exists." );
				}

				if( ShouldCancel() ) { return false; }

				SetStatus( "Checking for newer packages...", 61 );
				Shell.RunCommand( "sudo /opt/bin/opkg update" );
				
				SetStatus( "Checking for DNSCrypt...", 63 );

				bool WasInstalled = false;

				string DoesDNSCryptExist = Shell.RunCommand( "/opt/sbin/dnscrypt-proxy --help" );
				if( DoesDNSCryptExist.Contains( "No such file or directory") )
				{
					SetStatus( "Installing DNSCrypt...", 66 );

					Shell.RunCommand( "sudo /opt/bin/opkg install dnscrypt-proxy", new Regex( "Choose server from list or hit Enter to continue" ), ChooseResolver );

					DoesDNSCryptExist = Shell.RunCommand( "/opt/sbin/dnscrypt-proxy --help" );
					if( !DoesDNSCryptExist.Contains( "dnscrypt-proxy "))
					{
						throw new Exception( "Unknown state - could not determine if DNSCrypt exists." );
					}

					WasInstalled = true;
				}
				else if( !DoesDNSCryptExist.Contains( "dnscrypt-proxy "))
				{
					throw new Exception( "Unknown state - could not determine if DNSCrypt exists." );
				}

				if( ShouldCancel() ) { return false; }

				if( !WasInstalled )
				{
					//It's entirely possible that we've changed something and we previously had DNSCrypt running
					//So we should find it and kill it.

					SetStatus( "Killing DNSCrypt if running...", 68 );

					KillDNSCrypt( Shell );
				
					Shell.RunCommand( "sudo /opt/bin/opkg upgrade dnscrypt-proxy", new Regex( "Choose server from list or hit Enter to continue" ), ChooseResolver );
				}

				if( ShouldCancel() ) { return false; }

				SetStatus( "Connecting via SCP...", 80 );

				string DNSCryptProxyPort = "65053";

				using( ScpClient Client = new ScpClient( Data.Address, Data.Username, Data.Password ) )
				{
					Client.Connect();

					if( ShouldCancel() ) { return false; }

					SetStatus( "Downloading daemon profile...", 85 );

					string OriginalDaemonPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
					string NewDaemonPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
					string DNSCryptResolvers = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
					string StartupScriptPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
					
					using( Stream tempFile = new FileStream( OriginalDaemonPath, FileMode.CreateNew ) )
					{
						Client.Download( "/opt/etc/init.d/S09dnscrypt-proxy", tempFile );
					}

					SetStatus( "Downloading resolver list...", 87 );

					using( Stream tempFile = new FileStream( DNSCryptResolvers, FileMode.CreateNew ) )
					{
						Client.Download( "/opt/share/dnscrypt-proxy/dnscrypt-resolvers.csv", tempFile );
					}

					SetStatus( "Processing daemon file...", 88 );
										
					string[] DaemonFile = File.ReadAllLines( OriginalDaemonPath );

					string LocalAddress = "--local-address=127.0.0.1:";
					string Resolver = "-R ";
					//First match group is the port it starts on, the second is the current 
					var DaemonArgsRegex = new Regex( @".*" + LocalAddress + @"(\d+).*" + Resolver + @"([\w.-]+).*" );
					for( int LineIndex = 0; LineIndex < DaemonFile.Length; LineIndex++ )
					{


						if( DaemonFile[LineIndex].StartsWith( "ARGS=" ) )
						{
							var Match = DaemonArgsRegex.Match( DaemonFile[LineIndex] );
							if( !Match.Success )
							{
								throw new Exception( "Unable to parse dnscrypt-proxy arg line: " + DaemonFile[LineIndex] );
							}

							DNSCryptProxyPort = Match.Groups[1].Value;

							var OldResolverName = Match.Groups[2].Value;
							var ResolverName = Match.Groups[2].Value;
					
							bool Aborted = false;
							Parent.Invoke( new Action( () =>
							{
								var PickDNS = new DNSCryptResolverPicker( DNSCryptResolvers );
								if( PickDNS.ShowDialog() != DialogResult.OK )
								{
									Aborted = true;
								}

								ResolverName = PickDNS.GetPickedResolver();
							} ) );

							if( Aborted )
							{
								return false;
							}

							DaemonFile[LineIndex] = DaemonFile[LineIndex].Replace( Resolver + OldResolverName, Resolver + ResolverName );
						}					
					}

					SetStatus( "Uploading new daemon file...", 88 );

					using (TextWriter FileOut = new StreamWriter( NewDaemonPath ))
					{
						FileOut.NewLine = "\n";

						foreach( var Line in DaemonFile )
						{
							FileOut.WriteLine( Line );
						}
					}

					using( Stream uploadFile = new FileStream( NewDaemonPath, FileMode.Open ) )
					{
						Client.Upload( uploadFile, "/tmp/NewDaemonProfile" );
					}

					Shell.RunCommand( "sudo cp /tmp/NewDaemonProfile /opt/etc/init.d/S09dnscrypt-proxy" );
					Shell.RunCommand( "rm /tmp/NewDaemonProfile" );
					Shell.RunCommand( "sudo chmod 0755 /opt/etc/init.d/S09dnscrypt-proxy" );

					SetStatus( "Uploading new startup script...", 90 );

					string[] StartupScript = new string[]
					{
						"#!/bin/bash",
						"sudo /opt/etc/init.d/S09dnscrypt-proxy start",
						"exit"
					};		

					using (TextWriter FileOut = new StreamWriter( StartupScriptPath ))
					{
						FileOut.NewLine = "\n";

						foreach( var Line in StartupScript )
						{
							FileOut.WriteLine( Line );
						}
					}

					using( Stream uploadFile = new FileStream( StartupScriptPath, FileMode.Open ) )
					{
						Client.Upload( uploadFile, "/tmp/NewStartupScript" );
					}

					Client.Disconnect();
				}

				Shell.RunCommand( "sudo cp /tmp/NewStartupScript /config/scripts/post-config.d/start_dnscrypt.sh" );
				Shell.RunCommand( "rm /tmp/NewStartupScript" );
				Shell.RunCommand( "sudo chmod 0755 /config/scripts/post-config.d/start_dnscrypt.sh" );

				SetStatus( "Starting DNSCrypt...", 92 );

				Shell.RunCommand( "sudo /config/scripts/post-config.d/start_dnscrypt.sh" );

				SetStatus( "Configuring dnsmasq to use DNSCrypt...", 94 );

				Shell.RunCommand( "configure" );
				Shell.RunCommand( string.Format( "set service dns forwarding options \"server=127.0.0.1#{0}\"", DNSCryptProxyPort ) );
				Shell.RunCommand( "set service dns forwarding options proxy-dnssec" );

				//WARNING: This line disables all other forms of DNS. If DNSCrypt is not working you'll lose DNS completely.
				Shell.RunCommand( "set service dns forwarding options no-resolv" );

				SetStatus( "Committing changes...", 96 );
				Shell.RunCommand( "commit" );
				Shell.RunCommand( "save" );
				Shell.RunCommand( "exit" );

				SetStatus( "Restarting dnsmasq...", 98 );

				Shell.RunCommand( "sudo /etc/init.d/dnsmasq restart" );
			}

			//^(\d+\))\W+([a-zA-Z0-9\-.]+)\W+\((.*)\)$
			
			SetStatus( "Completed.", 100 );

			return true;
		}
	}
}
