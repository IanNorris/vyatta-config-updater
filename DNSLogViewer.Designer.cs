namespace vyatta_config_updater
{
	partial class DNSLogViewer
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.Cancel = new System.Windows.Forms.Button();
			this.OK = new System.Windows.Forms.Button();
			this.RecordList = new System.Windows.Forms.ListView();
			this.label1 = new System.Windows.Forms.Label();
			this.Column_Domain = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Column_ReverseDNS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Column_IP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Column_Requester = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Column_IPOwner = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.RecordList, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(6);
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(863, 589);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this.Cancel);
			this.flowLayoutPanel1.Controls.Add(this.OK);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(692, 551);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 29);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// Cancel
			// 
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new System.Drawing.Point(84, 3);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 23);
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			// 
			// OK
			// 
			this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK.Enabled = false;
			this.OK.Location = new System.Drawing.Point(3, 3);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(75, 23);
			this.OK.TabIndex = 1;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new System.EventHandler(this.OK_Click);
			// 
			// RecordList
			// 
			this.RecordList.AllowColumnReorder = true;
			this.RecordList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.RecordList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column_Domain,
            this.Column_ReverseDNS,
            this.Column_IP,
            this.Column_Requester,
            this.Column_IPOwner});
			this.RecordList.FullRowSelect = true;
			this.RecordList.HideSelection = false;
			this.RecordList.Location = new System.Drawing.Point(9, 9);
			this.RecordList.MinimumSize = new System.Drawing.Size(50, 50);
			this.RecordList.MultiSelect = false;
			this.RecordList.Name = "RecordList";
			this.RecordList.Size = new System.Drawing.Size(845, 523);
			this.RecordList.TabIndex = 0;
			this.RecordList.UseCompatibleStateImageBehavior = false;
			this.RecordList.View = System.Windows.Forms.View.Details;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(9, 535);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(845, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "NOTE: Data may contain data collected from background processes.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Column_Domain
			// 
			this.Column_Domain.Text = "Domain";
			this.Column_Domain.Width = 200;
			// 
			// Column_ReverseDNS
			// 
			this.Column_ReverseDNS.Text = "Reverse DNS Name";
			this.Column_ReverseDNS.Width = 200;
			// 
			// Column_IP
			// 
			this.Column_IP.Text = "IP";
			this.Column_IP.Width = 120;
			// 
			// Column_Requester
			// 
			this.Column_Requester.Text = "Requester";
			this.Column_Requester.Width = 120;
			// 
			// Column_IPOwner
			// 
			this.Column_IPOwner.Text = "IP Owner";
			this.Column_IPOwner.Width = 200;
			// 
			// DNSLogViewer
			// 
			this.AcceptButton = this.OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(863, 589);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "DNSLogViewer";
			this.Text = "DNS Log Viewer";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.ListView RecordList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader Column_Domain;
		private System.Windows.Forms.ColumnHeader Column_ReverseDNS;
		private System.Windows.Forms.ColumnHeader Column_IP;
		private System.Windows.Forms.ColumnHeader Column_Requester;
		private System.Windows.Forms.ColumnHeader Column_IPOwner;
	}
}