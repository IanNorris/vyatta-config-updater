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

namespace vyatta_config_updater
{
	public partial class Login : Form
	{
		const string RegistryKey = @"SOFTWARE\VyattaConfigUpdater";
		string Fingerprint = "";

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

		private void Cancel_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		private void OK_Click( object sender, EventArgs e )
		{
			SessionOptions options = new SessionOptions
			{
				Protocol = Protocol.Scp,
				HostName = Address.Text,
				UserName = Username.Text,
				Password = Password.Text
			};

			using( Session session = new Session() )
			{
				if( Fingerprint == null || Fingerprint.Length == 0 )
				{
					Fingerprint = session.ScanFingerprint( options );
				}
				else
				{
					string NewFingerprint = session.ScanFingerprint( options );
					if( Fingerprint != NewFingerprint )
					{
						var Result = MessageBox.Show( "Host fingerprint has changed from:\n" + Fingerprint + "\nTo:\n" + NewFingerprint + "\n\nDo you wish to continue?", "Fingerprint changed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning );
						if( Result == DialogResult.No )
						{
							return;
						}

						Fingerprint = NewFingerprint;
					}
				}

				Microsoft.Win32.RegistryKey regSettings = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryKey);
				regSettings.SetValue("Address", Address.Text);
				regSettings.SetValue("Username", Username.Text);
				regSettings.SetValue("Fingerprint", Fingerprint);
				regSettings.Close();
			
				options.SshHostKeyFingerprint = Fingerprint;

				try
				{
					session.Open( options );
				}
				catch( SessionException exception )
				{
					MessageBox.Show( exception.Message, "Error connecting", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}

				string tempConfigPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
				
				try
				{
					TransferOperationResult result = session.GetFiles( "/config/config.boot", tempConfigPath, false );
					result.Check();
				}
				catch( SessionException exception )
				{
					MessageBox.Show( exception.Message, "Error connecting", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
			}
		}
	}
}
