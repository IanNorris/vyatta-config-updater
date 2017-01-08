using System;
using System.IO;

namespace vyatta_config_updater
{
	partial class Busy
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.Progress = new System.Windows.Forms.ProgressBar();
			this.Status = new System.Windows.Forms.Label();
			this.Cancel = new System.Windows.Forms.Button();
			this.BusyBackgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.Progress, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.Status, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.Cancel, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 8);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(439, 67);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// Progress
			// 
			this.Progress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Progress.Location = new System.Drawing.Point(3, 3);
			this.Progress.MaximumSize = new System.Drawing.Size(0, 20);
			this.Progress.Name = "Progress";
			this.Progress.Size = new System.Drawing.Size(433, 20);
			this.Progress.TabIndex = 1;
			this.Progress.Value = 30;
			// 
			// Status
			// 
			this.Status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Status.AutoSize = true;
			this.Status.Location = new System.Drawing.Point(3, 26);
			this.Status.Name = "Status";
			this.Status.Size = new System.Drawing.Size(433, 13);
			this.Status.TabIndex = 2;
			this.Status.Text = "Connecting...";
			this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Cancel
			// 
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.Cancel.Location = new System.Drawing.Point(361, 42);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 23);
			this.Cancel.TabIndex = 3;
			this.Cancel.TabStop = false;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// BusyBackgroundWorker
			// 
			this.BusyBackgroundWorker.WorkerSupportsCancellation = true;
			this.BusyBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BusyBackgroundWorker_DoWork);
			this.BusyBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BusyBackgroundWorker_Completed);
			// 
			// Busy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(455, 83);
			this.Controls.Add(this.tableLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Busy";
			this.Padding = new System.Windows.Forms.Padding(8);
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Please wait...";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Busy_FormClosing);
			this.Load += new System.EventHandler(this.Busy_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ProgressBar Progress;
		private System.Windows.Forms.Label Status;
		private System.Windows.Forms.Button Cancel;
		private System.ComponentModel.BackgroundWorker BusyBackgroundWorker;
	}
}