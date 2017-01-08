using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;
using vyatta_config_updater.VyattaConfig;
using System.Diagnostics;

namespace vyatta_config_updater
{
	public class RouterLogDNS : BusyWorkInterface
	{
		private RouterData Data;
		private string LogPath;

		public RouterLogDNS( RouterData Data )
		{
			this.Data = Data;
		}

		public string GetLogPath()
		{
			return LogPath;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			SetStatus( "Flushing local DNS cache...", 0 );

			try
			{
				string IPConfigPath = Environment.ExpandEnvironmentVariables( @"%SystemRoot%\System32\ipconfig.exe" );
				var IPConfig = Process.Start( IPConfigPath, "/flushdns" );
				if( IPConfig == null )
				{
					throw new Exception( "Unable to flush local DNS cache. Could not run ipconfig." );
				}
				IPConfig.WaitForExit();
			}
			catch( Exception )
			{
				throw new Exception( "Unable to flush local DNS cache. Could not run ipconfig." );
			}

			SetStatus( "Connecting to SSH and SCP...", 5 );

			using( VyattaShell Shell = new VyattaShell( Data.Address, Data.Username, Data.Password ) )
			{
				using( ScpClient Client = new ScpClient( Data.Address, Data.Username, Data.Password ) )
				{
					Client.Connect();

					SetStatus( "Grabbing dnsmasq config...", 10 );

					string DNSMasqConfigPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
					string NewDNSMasqConfigPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
					
					using( Stream tempFile = new FileStream( DNSMasqConfigPath, FileMode.CreateNew ) )
					{
						Client.Download( "/etc/dnsmasq.conf", tempFile );
					}

					SetStatus( "Backing up dnsmasq config...", 15 );

					Shell.RunCommand( "sudo cp /etc/dnsmasq.conf /etc/dnsmasq.conf.bak" );

					SetStatus( "Processing config file...", 20 );
										
					string[] ConfigFile = File.ReadAllLines( DNSMasqConfigPath );
					
					for( int LineIndex = 0; LineIndex < ConfigFile.Length; LineIndex++ )
					{
						//Comment out log related lines
						if( ConfigFile[LineIndex].StartsWith( "log-" ) || ConfigFile[LineIndex].StartsWith( "cache-size=" ) )
						{
							ConfigFile[LineIndex] = "#" + ConfigFile[LineIndex];
						}
					}

					string[] ConfigFileFooter = new string[]
					{
						"log-queries",
						//"log-async=25", // This just confuses the log and makes it more difficult to parse.
						"log-facility=/tmp/dnslog.txt",
						"cache-size=0"
					};

					string[] ConfigFileFinal = new string[ ConfigFile.Length + ConfigFileFooter.Length ];
					ConfigFile.CopyTo( ConfigFileFinal, 0 );
					ConfigFileFooter.CopyTo( ConfigFileFinal, ConfigFile.Length );			

					using (TextWriter FileOut = new StreamWriter( NewDNSMasqConfigPath ))
					{
						FileOut.NewLine = "\n";

						foreach( var Line in ConfigFileFinal )
						{
							FileOut.WriteLine( Line );
						}
					}

					SetStatus( "Uploading new temporary config file...", 30 );

					using( Stream uploadFile = new FileStream( NewDNSMasqConfigPath, FileMode.Open ) )
					{
						Client.Upload( uploadFile, "/tmp/NewConfigFile" );
					}

					SetStatus( "Preparing config file...", 40 );
					
					Shell.RunCommand( "sudo cp /tmp/NewConfigFile /etc/dnsmasq.conf" );
					Shell.RunCommand( "rm /tmp/NewConfigFile" );
					Shell.RunCommand( "sudo chmod 0644 /etc/dnsmasq.conf" );

					SetStatus( "Restarting dnsmasq...", 60 );

					//Do stop/start separately so we can delete the file in case
					//something went wrong previously.
					Shell.RunCommand( "sudo /etc/init.d/dnsmasq stop" );
					Shell.RunCommand( "sudo rm /tmp/dnslog.txt" );
					Shell.RunCommand( "sudo /etc/init.d/dnsmasq start" );

					SetStatus( "Collecting data, press Cancel to stop...", 100 );
					
					while( !ShouldCancel() )
					{
						System.Threading.Thread.Sleep( 100 );
					}

					SetStatus( "Restarting dnsmasq as normal...", 0 );

					//Restore backup
					Shell.RunCommand( "sudo cp /etc/dnsmasq.conf.bak /etc/dnsmasq.conf" );

					//Restart dnsmasq with the old config again
					Shell.RunCommand( "sudo /etc/init.d/dnsmasq restart" );

					SetStatus( "Downloading collected data...", 50 );

					LogPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());

					Shell.RunCommand( "sudo chmod 777 /tmp/dnslog.txt" );
					
					using( Stream tempFile = new FileStream( LogPath, FileMode.CreateNew ) )
					{
						Client.Download( "/tmp/dnslog.txt", tempFile );
					}

					//Delete the log file now we've got it.
					Shell.RunCommand( "sudo rm /tmp/dnslog.txt" );
					
					SetStatus( "Disconnecting.", 95 );

					Client.Disconnect();
				}
			}
		
			SetStatus( "Completed.", 100 );

			return true;
		}
	}
}
