using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vyatta_config_updater
{
	public partial class Main : Form
	{
		private string Address;
		private string Username;
		private string Password;
		private string OldConfigPath;
		private ASNData ASNData;
		private List<InterfaceMapping> Interfaces;

		public Main( string Address, string Username, string Password, string ConfigPath, ASNData ASNData, List<InterfaceMapping> Interfaces )
		{
			this.Address = Address;
			this.Username = Username;
			this.Password = Password;
			this.OldConfigPath = ConfigPath;
			this.ASNData = ASNData;
			this.Interfaces = Interfaces;

			InitializeComponent();
		}

		private void Upload_Click( object sender, EventArgs e )
		{
			var GenerateConfig = new RouterGenerateNewConfig( OldConfigPath, ASNData, Interfaces );

			Busy BusyWorker = new Busy( GenerateConfig );

			if( BusyWorker.ShowDialog() == DialogResult.OK )
			{
				if( File.ReadAllText( OldConfigPath ) == File.ReadAllText( GenerateConfig.GetNewConfigPath() ) )
				{
					MessageBox.Show( "New config is identical to the old config.\nUpdate will not be performed.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information );
				}
				else
				{
					var Proc = Process.Start( @"C:\Program Files (x86)\WinMerge\WinMergeU.exe", string.Format( "\"{0}\" \"{1}\"", OldConfigPath, GenerateConfig.GetNewConfigPath() ) );
					Proc.WaitForExit();

					if( MessageBox.Show( "Upload new config?", "Are you sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						var Busy = new Busy( new RouterWriteNewConfig( Address, Username, Password, GenerateConfig.GetNewConfigPath() ) );
						if( Busy.ShowDialog() == DialogResult.OK )
						{
							MessageBox.Show( "New config has been uploaded.\nYour router will now reload the config.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information );
						}
						else
						{
							MessageBox.Show( "Error uploading new config..", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error );
						}
					}
				}
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			
			Application.Exit();
		}
	}
}
