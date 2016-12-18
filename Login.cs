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
using Renci.SshNet;
using Renci.SshNet.Common;

namespace vyatta_config_updater
{
	public partial class Login : Form
	{
		const string RegistryKey = @"SOFTWARE\VyattaConfigUpdater";
		bool ProgrammaticClosing = false;

		public Login()
		{
			InitializeComponent();

			Microsoft.Win32.RegistryKey regSettings = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryKey);
			Address.Text = (string)regSettings.GetValue( "Address", "" );
			Username.Text = (string)regSettings.GetValue( "Username", "" );
			SavePassword.Checked = (string)regSettings.GetValue( "SavePassword", "False" ) == "True";

			string EncryptedPassword = (string)regSettings.GetValue("Password");
			if( EncryptedPassword != null && EncryptedPassword.Length > 0 )
			{
				Password.Text = Util.DecryptString( EncryptedPassword );
			}
			
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
			var Busy = new Busy( Address.Text, Username.Text, Password.Text );
			if( Busy.ShowDialog() == DialogResult.OK )
			{
				Microsoft.Win32.RegistryKey regSettings = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryKey);
				regSettings.SetValue( "Address", Address.Text );
				regSettings.SetValue( "Username", Username.Text );
				regSettings.SetValue( "SavePassword", SavePassword.Checked ? "True" : "False" );
				if( SavePassword.Checked )
				{
					regSettings.SetValue( "Password", Util.EncryptString( Password.Text ) );
				}
				else
				{
					regSettings.DeleteValue( "Password", false );
				}
				regSettings.Close();

				var MainForm = new Main( Address.Text, Username.Text, Password.Text, Busy.GetTempPath() );
				Visible = false;
				MainForm.Show();
				ProgrammaticClosing = true;
				Close();
			}
		}
	}
}
