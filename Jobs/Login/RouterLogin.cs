using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Common;
using vyatta_config_updater.VyattaConfig;
using vyatta_config_updater.Routing;

namespace vyatta_config_updater
{
	public class RouterLogin : BusyWorkInterface
	{
		private string TempPath;
		private string TempTemplatePath;

		private RouterData Data;

		public RouterLogin( RouterData Data )
		{
			this.Data = Data;
			
			TempPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
			TempTemplatePath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
		}

		public string GetTempPath()
		{
			return TempPath;
		}

		public string GetTempTemplatePath()
		{
			return TempTemplatePath;
		}

		private string RunCommand( SshClient Client, string CommandLine )
		{
			System.Console.Out.WriteLine( "$ " + CommandLine );

			var Command = Client.RunCommand( CommandLine );
			string Output = Command.Execute();
			
			if( Command.Error.Length > 0 )
			{
				System.Console.Error.WriteLine( Command.Error );
			}

			if( Output.Length > 0 )
			{
				System.Console.Out.WriteLine( Output );
			}
						
			if( Command.Error.Length > 0 || Command.ExitStatus != 0 )
			{
				throw new Exception( string.Format( "Command failed with the exit code {0}.\nCommand:\n{1}\nError:{2}\n", Command.ExitStatus, CommandLine, Command.Error ) );
			}

			return Output;
		}

		private string RunShellCommand( ShellStream Stream, string CommandLine, bool ExpectRootPrompt )
		{
			System.Console.Out.WriteLine( "CL$ " + CommandLine );

			Stream.WriteLine( CommandLine );

			string Response = "";

			bool End = false;

			while( !End )
			{
				Stream.Expect(
					new ExpectAction( new Regex(@"\w+@\w+\:[\w~\/\\]+\$"), (Input) =>
					{
						End = true;
						Response += Input;
					} ),
					new ExpectAction( new Regex(@"\w+@\w+\:[\w~\/\\]+#"), (Input) =>
					{
						End = true;
						Response += Input;
					} ),
					new ExpectAction( new Regex(@"\n\:"), (Input) =>
					{
						Response += Input;
						Stream.Write(" ");
					} )
				);
			}
			
			System.Console.Out.WriteLine( Response );
			
			return Response;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			if( ShouldCancel() ) { return false; }

			SetStatus( "Connecting to SSH...", 0 );

			using( VyattaShell Shell = new VyattaShell( Data.Address, Data.Username, Data.Password ) )
			{
				if( ShouldCancel() ) { return false; }

				//Verify that we can identify the device

				SetStatus( "Identifying device...", 5 );
					
				var Version = Shell.RunCommand( "cat /proc/version" );
				if( !Version.Contains( "edgeos" ) )
				{
					throw new Exception( "The device is not running EdgeOS and is not supported. Device ");
				}

				if( ShouldCancel() ) { return false; }

				//Enter configure mode
				
				SetStatus( "Processing routing interfaces...", 8 );
				
				Dictionary<string, string> Gateways = IPRoute.GetDefaultGateways( Shell );
												
				SetStatus( "Processing interface list...", 16 );
				string ShowInterfaces = Shell.RunCommand( "show interfaces" );

				Regex ParseInterfaces = new Regex( @"(\w+)\s+([0-9.\-]+(:?\/[0-9]+)?)\s+(\w\/\w)\s+(\w+)?" );

				Data.Interfaces = new List<InterfaceMapping>();

				string[] InterfaceLines = ShowInterfaces.Split( new char[] { '\n' } );
				foreach( string Line in InterfaceLines )
				{
					Match Match = ParseInterfaces.Match( Line );
					if( Match.Success )
					{
						InterfaceMapping Mapping = new InterfaceMapping();

						Mapping.Interface = Match.Groups[1].Value;
						Mapping.IPAddress = Match.Groups[2].Value == "-" ? "" : Match.Groups[2].Value;
						Mapping.Codes = Match.Groups[4].Value;
						Mapping.Description = Match.Groups[5].Value;

						string Gateway;
						if( Gateways.TryGetValue( Mapping.Interface, out Gateway ))
						{
							Mapping.Gateway = Gateway;
						}

						Data.Interfaces.Add( Mapping );
					}
				}
			}

			SetStatus( "Connecting over SCP...", 60 );

			if( ShouldCancel() ) { return false; }

			using( ScpClient Client = new ScpClient( Data.Address, Data.Username, Data.Password ) )
			{
				Client.Connect();

				if( ShouldCancel() ) { return false; }

				SetStatus( "Downloading config...", 80 );

				using( Stream tempFile = new FileStream( TempPath, FileMode.CreateNew ) )
				{
					Client.Download( "/config/config.boot", tempFile );
				}

				SetStatus( "Parsing existing config...", 85 );

				string Errors = "";
				Data.OldConfigLines = File.ReadAllLines( TempPath );
				Data.ConfigRoot = VyattaConfigUtil.ReadFromFile( TempPath, ref Errors );
				if( Errors.Length > 0 )
				{
					throw new Exception( Errors );
				}

				SetStatus( "Downloading current template...", 90 );
				
				try
				{
					using( Stream tempTemplateFile = new FileStream( TempTemplatePath, FileMode.CreateNew ) )
					{
						Client.Download( "/config/vcu/current.vcu", tempTemplateFile );
					}

					SetStatus( "Parsing current template...", 95 );

					Errors = "";
					Data.TemplateRoot = VyattaConfigUtil.ReadFromFile( TempTemplatePath, ref Errors );
					if( Errors.Length > 0 )
					{
						throw new Exception( Errors );
					}
				}
				catch( SshException e )
				{
					if( !e.Message.Contains( "No such file or directory" ) )
					{
						throw e;
					}

					//It's quite okay to fail here, it means the user hasn't uploaded
					//a config with the tool yet.

					Data.TemplateRoot = new VyattaConfigObject( null );
				}

				if( ShouldCancel() ) { return false; }

				SetStatus( "Disconnecting...", 98 );

				Client.Disconnect();
			}

			SetStatus( "Completed.", 100 );

			return true;
		}
	}
}
