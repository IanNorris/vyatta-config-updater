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
			this.components = new System.ComponentModel.Container();
			this.MainLayout = new System.Windows.Forms.TableLayoutPanel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.Tabs = new System.Windows.Forms.TabControl();
			this.TabPage_Routing = new System.Windows.Forms.TabPage();
			this.RoutingList = new System.Windows.Forms.ListView();
			this.Routing_Column_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Routing_Column_Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Routing_Column_Destination = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Routing_Column_Interface = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.RougingListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.RoutingList_ContextMenu_Add = new System.Windows.Forms.ToolStripMenuItem();
			this.RoutingList_ContextMenu_Remove = new System.Windows.Forms.ToolStripMenuItem();
			this.TabPage_VPNs = new System.Windows.Forms.TabPage();
			this.ConfigDiff = new Menees.Diffs.Controls.DiffControl();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveConfigToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.makeLiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rollbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addVPNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logDNSQueriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aSNBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enableDNSCryptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MainLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.Tabs.SuspendLayout();
			this.TabPage_Routing.SuspendLayout();
			this.RougingListContextMenu.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// MainLayout
			// 
			this.MainLayout.ColumnCount = 1;
			this.MainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.MainLayout.Controls.Add(this.splitContainer1, 0, 1);
			this.MainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainLayout.Location = new System.Drawing.Point(0, 24);
			this.MainLayout.Name = "MainLayout";
			this.MainLayout.RowCount = 2;
			this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.MainLayout.Size = new System.Drawing.Size(1003, 670);
			this.MainLayout.TabIndex = 2;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.Tabs);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.ConfigDiff);
			this.splitContainer1.Size = new System.Drawing.Size(997, 664);
			this.splitContainer1.SplitterDistance = 184;
			this.splitContainer1.TabIndex = 3;
			// 
			// Tabs
			// 
			this.Tabs.Controls.Add(this.TabPage_Routing);
			this.Tabs.Controls.Add(this.TabPage_VPNs);
			this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Tabs.Location = new System.Drawing.Point(0, 0);
			this.Tabs.Name = "Tabs";
			this.Tabs.SelectedIndex = 0;
			this.Tabs.Size = new System.Drawing.Size(997, 184);
			this.Tabs.TabIndex = 0;
			// 
			// TabPage_Routing
			// 
			this.TabPage_Routing.Controls.Add(this.RoutingList);
			this.TabPage_Routing.Location = new System.Drawing.Point(4, 22);
			this.TabPage_Routing.Name = "TabPage_Routing";
			this.TabPage_Routing.Size = new System.Drawing.Size(989, 158);
			this.TabPage_Routing.TabIndex = 0;
			this.TabPage_Routing.Text = "Routing";
			this.TabPage_Routing.UseVisualStyleBackColor = true;
			// 
			// RoutingList
			// 
			this.RoutingList.AllowColumnReorder = true;
			this.RoutingList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Routing_Column_Name,
            this.Routing_Column_Type,
            this.Routing_Column_Destination,
            this.Routing_Column_Interface});
			this.RoutingList.ContextMenuStrip = this.RougingListContextMenu;
			this.RoutingList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RoutingList.FullRowSelect = true;
			this.RoutingList.LabelEdit = true;
			this.RoutingList.Location = new System.Drawing.Point(0, 0);
			this.RoutingList.Name = "RoutingList";
			this.RoutingList.Size = new System.Drawing.Size(989, 158);
			this.RoutingList.TabIndex = 4;
			this.RoutingList.UseCompatibleStateImageBehavior = false;
			this.RoutingList.View = System.Windows.Forms.View.Details;
			// 
			// Routing_Column_Name
			// 
			this.Routing_Column_Name.Text = "Name";
			this.Routing_Column_Name.Width = 200;
			// 
			// Routing_Column_Type
			// 
			this.Routing_Column_Type.Text = "Type";
			this.Routing_Column_Type.Width = 90;
			// 
			// Routing_Column_Destination
			// 
			this.Routing_Column_Destination.Text = "Destination";
			this.Routing_Column_Destination.Width = 120;
			// 
			// Routing_Column_Interface
			// 
			this.Routing_Column_Interface.Text = "Interface";
			this.Routing_Column_Interface.Width = 120;
			// 
			// RougingListContextMenu
			// 
			this.RougingListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RoutingList_ContextMenu_Add,
            this.RoutingList_ContextMenu_Remove});
			this.RougingListContextMenu.Name = "RougingListContextMenu";
			this.RougingListContextMenu.Size = new System.Drawing.Size(118, 48);
			this.RougingListContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.RougingListContextMenu_Opening);
			// 
			// RoutingList_ContextMenu_Add
			// 
			this.RoutingList_ContextMenu_Add.Name = "RoutingList_ContextMenu_Add";
			this.RoutingList_ContextMenu_Add.Size = new System.Drawing.Size(117, 22);
			this.RoutingList_ContextMenu_Add.Text = "Add...";
			this.RoutingList_ContextMenu_Add.Click += new System.EventHandler(this.RoutingList_ContextMenu_Add_Click);
			// 
			// RoutingList_ContextMenu_Remove
			// 
			this.RoutingList_ContextMenu_Remove.Name = "RoutingList_ContextMenu_Remove";
			this.RoutingList_ContextMenu_Remove.Size = new System.Drawing.Size(117, 22);
			this.RoutingList_ContextMenu_Remove.Text = "Remove";
			this.RoutingList_ContextMenu_Remove.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// TabPage_VPNs
			// 
			this.TabPage_VPNs.Location = new System.Drawing.Point(4, 22);
			this.TabPage_VPNs.Name = "TabPage_VPNs";
			this.TabPage_VPNs.Size = new System.Drawing.Size(989, 158);
			this.TabPage_VPNs.TabIndex = 1;
			this.TabPage_VPNs.Text = "VPNs";
			this.TabPage_VPNs.UseVisualStyleBackColor = true;
			// 
			// ConfigDiff
			// 
			this.ConfigDiff.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConfigDiff.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.ConfigDiff.Location = new System.Drawing.Point(0, 0);
			this.ConfigDiff.Name = "ConfigDiff";
			this.ConfigDiff.ShowToolBar = false;
			this.ConfigDiff.ShowWhiteSpaceInLineDiff = true;
			this.ConfigDiff.Size = new System.Drawing.Size(997, 476);
			this.ConfigDiff.TabIndex = 0;
			this.ConfigDiff.ViewFont = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.setupToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1003, 24);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadConfigToolStripMenuItem,
            this.saveConfigToolStripMenuItem,
            this.saveConfigToolStripMenuItem1,
            this.toolStripSeparator1,
            this.makeLiveToolStripMenuItem,
            this.rollbackToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.newToolStripMenuItem.Text = "&New";
			// 
			// loadConfigToolStripMenuItem
			// 
			this.loadConfigToolStripMenuItem.Name = "loadConfigToolStripMenuItem";
			this.loadConfigToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.loadConfigToolStripMenuItem.Text = "&Load Config...";
			// 
			// saveConfigToolStripMenuItem
			// 
			this.saveConfigToolStripMenuItem.Name = "saveConfigToolStripMenuItem";
			this.saveConfigToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.saveConfigToolStripMenuItem.Text = "S&ave config as...";
			this.saveConfigToolStripMenuItem.Click += new System.EventHandler(this.saveConfigToolStripMenuItem_Click);
			// 
			// saveConfigToolStripMenuItem1
			// 
			this.saveConfigToolStripMenuItem1.Name = "saveConfigToolStripMenuItem1";
			this.saveConfigToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
			this.saveConfigToolStripMenuItem1.Text = "&Save config";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
			// 
			// makeLiveToolStripMenuItem
			// 
			this.makeLiveToolStripMenuItem.Name = "makeLiveToolStripMenuItem";
			this.makeLiveToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.makeLiveToolStripMenuItem.Text = "&Make Live";
			this.makeLiveToolStripMenuItem.Click += new System.EventHandler(this.makeLiveToolStripMenuItem_Click);
			// 
			// rollbackToolStripMenuItem
			// 
			this.rollbackToolStripMenuItem.Name = "rollbackToolStripMenuItem";
			this.rollbackToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.rollbackToolStripMenuItem.Text = "&Rollback...";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(155, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			// 
			// setupToolStripMenuItem
			// 
			this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addVPNToolStripMenuItem,
            this.enableDNSCryptToolStripMenuItem});
			this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
			this.setupToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
			this.setupToolStripMenuItem.Text = "&Setup";
			// 
			// addVPNToolStripMenuItem
			// 
			this.addVPNToolStripMenuItem.Name = "addVPNToolStripMenuItem";
			this.addVPNToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			this.addVPNToolStripMenuItem.Text = "Add &VPN...";
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logDNSQueriesToolStripMenuItem,
            this.aSNBrowserToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
			this.toolsToolStripMenuItem.Text = "&Tools";
			// 
			// logDNSQueriesToolStripMenuItem
			// 
			this.logDNSQueriesToolStripMenuItem.Name = "logDNSQueriesToolStripMenuItem";
			this.logDNSQueriesToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.logDNSQueriesToolStripMenuItem.Text = "&Log DNS Queries...";
			// 
			// aSNBrowserToolStripMenuItem
			// 
			this.aSNBrowserToolStripMenuItem.Name = "aSNBrowserToolStripMenuItem";
			this.aSNBrowserToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.aSNBrowserToolStripMenuItem.Text = "&ASN Browser...";
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.aboutToolStripMenuItem.Text = "&About...";
			// 
			// enableDNSCryptToolStripMenuItem
			// 
			this.enableDNSCryptToolStripMenuItem.Name = "enableDNSCryptToolStripMenuItem";
			this.enableDNSCryptToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			this.enableDNSCryptToolStripMenuItem.Text = "Enable DNSCrypt...";
			this.enableDNSCryptToolStripMenuItem.Click += new System.EventHandler(this.enableDNSCryptToolStripMenuItem_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1003, 694);
			this.Controls.Add(this.MainLayout);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Main";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Main";
			this.Load += new System.EventHandler(this.Main_Load);
			this.MainLayout.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.Tabs.ResumeLayout(false);
			this.TabPage_Routing.ResumeLayout(false);
			this.RougingListContextMenu.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private TableLayoutPanel MainLayout;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem loadConfigToolStripMenuItem;
		private ToolStripMenuItem saveConfigToolStripMenuItem;
		private ToolStripMenuItem setupToolStripMenuItem;
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem newToolStripMenuItem;
		private ToolStripMenuItem saveConfigToolStripMenuItem1;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem toolsToolStripMenuItem;
		private ToolStripMenuItem logDNSQueriesToolStripMenuItem;
		private ToolStripMenuItem makeLiveToolStripMenuItem;
		private ToolStripMenuItem rollbackToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem aSNBrowserToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		private SplitContainer splitContainer1;
		private Menees.Diffs.Controls.DiffControl ConfigDiff;
		private TabControl Tabs;
		private TabPage TabPage_Routing;
		private ListView RoutingList;
		private ColumnHeader Routing_Column_Name;
		private ColumnHeader Routing_Column_Type;
		private ColumnHeader Routing_Column_Destination;
		private ColumnHeader Routing_Column_Interface;
		private ContextMenuStrip RougingListContextMenu;
		private ToolStripMenuItem RoutingList_ContextMenu_Add;
		private ToolStripMenuItem RoutingList_ContextMenu_Remove;
		private TabPage TabPage_VPNs;
		private ToolStripMenuItem addVPNToolStripMenuItem;
		private ToolStripMenuItem enableDNSCryptToolStripMenuItem;
	}
}