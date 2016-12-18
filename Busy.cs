using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace vyatta_config_updater
{
	public partial class Busy : Form
	{
		private string TempPath;
		private bool Completed = false;

		private string Address;
		private string Username;
		private string Password;

		delegate void WriteToStream( string Input ); 

		public Busy( string Address, string Username, string Password )
		{
			this.Address = Address;
			this.Username = Username;
			this.Password = Password;

			InitializeComponent();
			TempPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
		}

		public string GetTempPath()
		{
			return TempPath;
		}

		private void SetStatus( string NewStatus, int NewProgress )
		{
			BeginInvoke( new Action(
				() => 
				{
					Progress.Value = NewProgress;
					Status.Text = NewStatus;
					Progress.Refresh();
					Status.Refresh();
				}
			) );
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

		private void BusyBackgroundWorker_DoWork( object sender, DoWorkEventArgs e )
		{
			SetStatus( "Connecting over SSH...", 0 );

			try
			{
				using( SshClient Client = new SshClient( Address, Username, Password ) )
				{
					Client.Connect();

					//Verify that we can identify the device

					SetStatus( "Identifying device...", 5 );
					
					var Version = RunCommand( Client, "cat /proc/version" );
					if( !Version.Contains( "edgeos" ) )
					{
						throw new Exception( "The device is not running EdgeOS and is not supported. Device ");
					}

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

				SetStatus( "Connecting over SCP...", 0 );

				using( ScpClient Client = new ScpClient( Address, Username, Password ) )
				{
					Client.Connect();

					SetStatus( "Downloading config...", 10 );

					using( Stream tempFile = new FileStream( TempPath, FileMode.CreateNew ) )
					{
						Client.Download( "/config/config.boot", tempFile );
					}

					SetStatus( "Disconnecting...", 10 );

					Client.Disconnect();
				}

				SetStatus( "Completed.", 100 );

				Completed = true;
			}
			catch( SshException exception )
			{
				BeginInvoke( new Action(
					() => 
					{
						MessageBox.Show( exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				) );
			}
			catch( IOException exception )
			{
				BeginInvoke( new Action(
					() => 
					{
						MessageBox.Show( exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				) );
			}
			catch( Exception exception )
			{
				BeginInvoke( new Action(
					() => 
					{
						MessageBox.Show( exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				) );
			}

		}

		private void BusyBackgroundWorker_Completed( object sender, RunWorkerCompletedEventArgs e )
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void Cancel_Click( object sender, EventArgs e )
		{
		}

		private void Busy_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( BusyBackgroundWorker.IsBusy )
			{
				e.Cancel = true;
				BusyBackgroundWorker.CancelAsync();
			}
			else
			{
				DialogResult = Completed ? DialogResult.OK : DialogResult.Cancel;
			}
		}

		private void Busy_Load( object sender, EventArgs e )
		{
			BusyBackgroundWorker.RunWorkerAsync();
		}
	}
}
