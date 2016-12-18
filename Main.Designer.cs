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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Upload = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Upload
			// 
			this.Upload.Location = new System.Drawing.Point(94, 80);
			this.Upload.Name = "Upload";
			this.Upload.Size = new System.Drawing.Size(75, 23);
			this.Upload.TabIndex = 0;
			this.Upload.Text = "Upload";
			this.Upload.UseVisualStyleBackColor = true;
			this.Upload.Click += new System.EventHandler(this.Upload_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.Upload);
			this.Name = "Main";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Main";
			this.ResumeLayout(false);

		}

		#endregion

		private Button Upload;
	}
}