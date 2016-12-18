using System;
using System.IO;
using System.Text.RegularExpressions;
using Renci.SshNet;


namespace vyatta_config_updater
{
	public class RouterLogin : BusyWorkInterface
	{
		private string TempPath;

		private string Address;
		private string Username;
		private string Password;

		public RouterLogin( string Address, string Username, string Password )
		{
			this.Address = Address;
			this.Username = Username;
			this.Password = Password;
			
			TempPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
		}

		public string GetTempPath()
		{
			return TempPath;
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
			if( ShouldCancel() ) { return false; }

			using( SshClient Client = new SshClient( Address, Username, Password ) )
			{
				Client.Connect();

				if( ShouldCancel() ) { return false; }

				//Verify that we can identify the device

				SetStatus( "Identifying device...", 5 );
					
				var Version = RunCommand( Client, "cat /proc/version" );
				if( !Version.Contains( "edgeos" ) )
				{
					throw new Exception( "The device is not running EdgeOS and is not supported. Device ");
				}

				if( ShouldCancel() ) { return false; }

				/*//Enter configure mode

				var TermOptions = new Dictionary<TerminalModes,uint>();
				TermOptions.Add( TerminalModes.ECHO, 0 );

				SetStatus( "Creating shell...", 10 );

				using( ShellStream Shell = Client.CreateShellStream( "bash", 80, 24, 800, 600, 64 * 1024, TermOptions ) )
				{
					string Initial = Shell.Expect(new Regex(@"[$>]"));
					System.Console.Out.WriteLine( Initial );

					SetStatus( "Entering configure mode...", 8 );
					string ShowInterfaces = RunShellCommand( Shell, "show interfaces", false );
					string Exit = RunShellCommand( Shell, "exit", false );
						
				}*/

				SetStatus( "Disconnecting from SSH...", 50 );

				Client.Disconnect();
			}

			SetStatus( "Connecting over SCP...", 60 );

			if( ShouldCancel() ) { return false; }

			using( ScpClient Client = new ScpClient( Address, Username, Password ) )
			{
				Client.Connect();

				if( ShouldCancel() ) { return false; }

				SetStatus( "Downloading config...", 80 );

				using( Stream tempFile = new FileStream( TempPath, FileMode.CreateNew ) )
				{
					Client.Download( "/config/config.boot", tempFile );
				}

				if( ShouldCancel() ) { return false; }

				SetStatus( "Disconnecting...", 90 );

				Client.Disconnect();
			}

			SetStatus( "Completed.", 100 );

			return true;
		}
	}
}
