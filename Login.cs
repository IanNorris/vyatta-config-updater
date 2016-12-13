using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSCP;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace vyatta_config_updater
{
	public partial class Login : Form
	{
		const string RegistryKey = @"SOFTWARE\VyattaConfigUpdater";
		string Fingerprint = "";
		bool ProgrammaticClosing = false;

		public Login()
		{
			InitializeComponent();

			Microsoft.Win32.RegistryKey regSettings = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryKey);
			Address.Text = (string)regSettings.GetValue("Address");
			Username.Text = (string)regSettings.GetValue("Username");
			Fingerprint = (string)regSettings.GetValue("Fingerprint");
			regSettings.Close();

			if( Username.Text.Length > 0 )
			{
				ActiveControl = Password;
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			
			if( !ProgrammaticClosing )
			{
				Application.Exit();
			}
			ProgrammaticClosing = false;
		}

		private void Cancel_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		private void OK_Click( object sender, EventArgs e )
		{
			try
			{
				using( ScpClient client = new ScpClient( Address.Text, Username.Text, Password.Text ) )
				{
					client.Connect();

					string tempConfigPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());

					using( Stream tempFile = new FileStream( tempConfigPath, FileMode.CreateNew ) )
					{
						client.Download( "/config/config.boot", tempFile );
					}

					client.Disconnect();

					var MainForm = new Main( Address.Text, Username.Text, Password.Text, tempConfigPath );
					Visible = false;
					MainForm.Show();
					ProgrammaticClosing = true;
					Close();
				}
			}
			catch( SshException exception )
			{
				MessageBox.Show( exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			catch( IOException exception )
			{
				MessageBox.Show( exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
	}
}
