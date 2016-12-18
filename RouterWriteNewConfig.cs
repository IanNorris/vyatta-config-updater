using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace vyatta_config_updater
{
	public class RouterWriteNewConfig : BusyWorkInterface
	{
		private string TempPath;

		private string Address;
		private string Username;
		private string Password;
		private string NewConfig;

		public RouterWriteNewConfig( string Address, string Username, string Password, string NewConfig )
		{
			this.Address = Address;
			this.Username = Username;
			this.Password = Password;
			this.NewConfig = NewConfig;
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

			string Response = Stream.Expect( ExpectRootPrompt ? new Regex(@"[#>]") : new Regex(@"[$>]") );

			System.Console.Out.WriteLine( Response );
			
			return Response;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			using( SshClient Client = new SshClient( Address, Username, Password ) )
			{
				Client.Connect();

				//Enter configure mode

				var TermOptions = new Dictionary<TerminalModes,uint>();
				TermOptions.Add( TerminalModes.ECHO, 0 );

				SetStatus( "Creating shell...", 10 );

				using( ShellStream Shell = Client.CreateShellStream( "bash", 80, 24, 800, 600, 64 * 1024, TermOptions ) )
				{
					string Initial = Shell.Expect(new Regex(@"[$>]"));
					System.Console.Out.WriteLine( Initial );

					SetStatus( "Entering configure mode...", 8 );
					RunShellCommand( Shell, "configure", true );

					if( !ShouldCancel() )
					{
						using( ScpClient ScpClient = new ScpClient( Address, Username, Password ) )
						{
							ScpClient.Connect();

							SetStatus( "Uploading new config...", 15 );

							if( !ShouldCancel() )
							{
								using( Stream configFile = new FileStream( NewConfig, FileMode.Open ) )
								{
									ScpClient.Upload( configFile, "/config/config.boot" );
								}
							}

							SetStatus( "Disconnecting SCP...", 20 );

							ScpClient.Disconnect();
						}
					}

					SetStatus( "Loading new config...", 25 );

					RunShellCommand( Shell, "load", true );

					SetStatus( "Comparing config...", 35 );
					RunShellCommand( Shell, "compare", true );

					SetStatus( "Committing new config (this will take a while)...", 45 );
					RunShellCommand( Shell, "commit", true );

					SetStatus( "Committing new config (this will take a while)...", 95 );
					RunShellCommand( Shell, "exit", false );
						
				}

				SetStatus( "Disconnecting from SSH...", 98 );

				Client.Disconnect();
			}
			
			SetStatus( "Completed.", 100 );

			return true;
		}
	}
}
