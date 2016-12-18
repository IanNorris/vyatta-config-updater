using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using vyatta_config_updater.VyattaConfig;
using vyatta_config_updater.VyattaConfig.Routing;

namespace vyatta_config_updater
{
	partial class Main
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		public Main( string Address, string Username, string Password, string ConfigPath, ASNData ASNData )
		{
			InitializeComponent();

			string Errors = "";
			VyattaConfigObject Root = VyattaConfigUtil.ReadFromFile( ConfigPath, ref Errors );
			
			//23.246.0.0-23.246.31.255:
			//CIDR: 23.246.0.0/19

			VyattaConfigRouting.DeleteStaticRoute( Root, "104.27.200.0/22" );
			
			VyattaConfigRouting.AddStaticRoutesForOrganization( Root, "Netflix", ASNData, "192.168.72.1" );
			VyattaConfigRouting.AddStaticRoutesForOrganization( Root, "BBC", ASNData, "192.168.72.1" );
			VyattaConfigRouting.AddStaticRoutesForOrganization( Root, "Valve", ASNData, "192.168.72.1" );
			VyattaConfigRouting.AddStaticRoutesForOrganization( Root, "Nest", ASNData, "192.168.72.1" );
			
			string tempOutputConfigPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());

			VyattaConfigUtil.WriteToFile( Root, tempOutputConfigPath );

			var Proc = Process.Start( @"C:\Program Files (x86)\WinMerge\WinMergeU.exe", string.Format( "\"{0}\" \"{1}\"", ConfigPath, tempOutputConfigPath ) );
			Proc.WaitForExit();

			if( MessageBox.Show( "Upload new config?", "Are you sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				var Busy = new Busy( new RouterWriteNewConfig( Address, Username, Password, tempOutputConfigPath ) );
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

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			
			Application.Exit();
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Text = "Main";
		}

		#endregion
	}
}