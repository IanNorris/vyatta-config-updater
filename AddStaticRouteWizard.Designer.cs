namespace vyatta_config_updater
{
	partial class AddStaticRouteWizard
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
			this.Wizard = new AeroWizard.WizardControl();
			this.Page_Name = new AeroWizard.WizardPage();
			this.themedTableLayoutPanel1 = new AeroWizard.ThemedTableLayoutPanel();
			this.themedLabel1 = new AeroWizard.ThemedLabel();
			this.Wizard_Page_Name_NewName = new System.Windows.Forms.TextBox();
			this.Page_Destination = new AeroWizard.WizardPage();
			this.Page_Destination_Layout = new AeroWizard.ThemedTableLayoutPanel();
			this.Destination_Organization = new System.Windows.Forms.RadioButton();
			this.TrafficDestination_ASN_Layout = new System.Windows.Forms.TableLayoutPanel();
			this.ASNSearch = new System.Windows.Forms.TextBox();
			this.ASNSearchResults = new System.Windows.Forms.ListBox();
			this.Destination_ = new System.Windows.Forms.RadioButton();
			this.Page_Destination_ASN_Layout = new System.Windows.Forms.TableLayoutPanel();
			this.Page_Via = new AeroWizard.WizardPage();
			this.Page_Finished = new AeroWizard.WizardPage();
			((System.ComponentModel.ISupportInitialize)(this.Wizard)).BeginInit();
			this.Page_Name.SuspendLayout();
			this.themedTableLayoutPanel1.SuspendLayout();
			this.Page_Destination.SuspendLayout();
			this.Page_Destination_Layout.SuspendLayout();
			this.TrafficDestination_ASN_Layout.SuspendLayout();
			this.SuspendLayout();
			// 
			// Wizard
			// 
			this.Wizard.BackColor = System.Drawing.Color.White;
			this.Wizard.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Wizard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Wizard.Location = new System.Drawing.Point(0, 0);
			this.Wizard.Name = "Wizard";
			this.Wizard.Pages.Add(this.Page_Name);
			this.Wizard.Pages.Add(this.Page_Destination);
			this.Wizard.Pages.Add(this.Page_Via);
			this.Wizard.Pages.Add(this.Page_Finished);
			this.Wizard.Size = new System.Drawing.Size(661, 498);
			this.Wizard.SuppressParentFormCaptionSync = true;
			this.Wizard.SuppressParentFormIconSync = true;
			this.Wizard.TabIndex = 0;
			this.Wizard.Title = "Create a new static route";
			// 
			// Page_Name
			// 
			this.Page_Name.AllowBack = false;
			this.Page_Name.Controls.Add(this.themedTableLayoutPanel1);
			this.Page_Name.Name = "Page_Name";
			this.Page_Name.Size = new System.Drawing.Size(614, 344);
			this.Page_Name.TabIndex = 0;
			this.Page_Name.Text = "Name";
			// 
			// themedTableLayoutPanel1
			// 
			this.themedTableLayoutPanel1.AutoSize = true;
			this.themedTableLayoutPanel1.ColumnCount = 1;
			this.themedTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.themedTableLayoutPanel1.Controls.Add(this.themedLabel1, 0, 0);
			this.themedTableLayoutPanel1.Controls.Add(this.Wizard_Page_Name_NewName, 0, 1);
			this.themedTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.themedTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.themedTableLayoutPanel1.Name = "themedTableLayoutPanel1";
			this.themedTableLayoutPanel1.RowCount = 3;
			this.themedTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.themedTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.themedTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.themedTableLayoutPanel1.Size = new System.Drawing.Size(614, 344);
			this.themedTableLayoutPanel1.TabIndex = 0;
			// 
			// themedLabel1
			// 
			this.themedLabel1.Location = new System.Drawing.Point(3, 0);
			this.themedLabel1.Name = "themedLabel1";
			this.themedLabel1.Size = new System.Drawing.Size(253, 23);
			this.themedLabel1.TabIndex = 0;
			this.themedLabel1.Text = "Enter a name to refer to the new rule:";
			// 
			// Wizard_Page_Name_NewName
			// 
			this.Wizard_Page_Name_NewName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Wizard_Page_Name_NewName.Location = new System.Drawing.Point(3, 26);
			this.Wizard_Page_Name_NewName.Name = "Wizard_Page_Name_NewName";
			this.Wizard_Page_Name_NewName.Size = new System.Drawing.Size(608, 23);
			this.Wizard_Page_Name_NewName.TabIndex = 1;
			// 
			// Page_Destination
			// 
			this.Page_Destination.Controls.Add(this.Page_Destination_Layout);
			this.Page_Destination.Name = "Page_Destination";
			this.Page_Destination.Size = new System.Drawing.Size(614, 350);
			this.Page_Destination.TabIndex = 1;
			this.Page_Destination.Text = "Traffic destination";
			// 
			// Page_Destination_Layout
			// 
			this.Page_Destination_Layout.ColumnCount = 1;
			this.Page_Destination_Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Page_Destination_Layout.Controls.Add(this.Destination_Organization, 0, 0);
			this.Page_Destination_Layout.Controls.Add(this.TrafficDestination_ASN_Layout, 0, 1);
			this.Page_Destination_Layout.Controls.Add(this.Destination_, 0, 2);
			this.Page_Destination_Layout.Controls.Add(this.Page_Destination_ASN_Layout, 0, 3);
			this.Page_Destination_Layout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Page_Destination_Layout.Location = new System.Drawing.Point(0, 0);
			this.Page_Destination_Layout.Name = "Page_Destination_Layout";
			this.Page_Destination_Layout.RowCount = 6;
			this.Page_Destination_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.Page_Destination_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Page_Destination_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.Page_Destination_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.Page_Destination_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.Page_Destination_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.Page_Destination_Layout.Size = new System.Drawing.Size(614, 350);
			this.Page_Destination_Layout.TabIndex = 0;
			// 
			// Destination_Organization
			// 
			this.Destination_Organization.AutoSize = true;
			this.Destination_Organization.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Destination_Organization.Location = new System.Drawing.Point(3, 3);
			this.Destination_Organization.Name = "Destination_Organization";
			this.Destination_Organization.Size = new System.Drawing.Size(608, 19);
			this.Destination_Organization.TabIndex = 0;
			this.Destination_Organization.TabStop = true;
			this.Destination_Organization.Text = "All IPs assigned to an organization:";
			this.Destination_Organization.UseVisualStyleBackColor = true;
			// 
			// TrafficDestination_ASN_Layout
			// 
			this.TrafficDestination_ASN_Layout.ColumnCount = 1;
			this.TrafficDestination_ASN_Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TrafficDestination_ASN_Layout.Controls.Add(this.ASNSearch, 0, 0);
			this.TrafficDestination_ASN_Layout.Controls.Add(this.ASNSearchResults, 0, 1);
			this.TrafficDestination_ASN_Layout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TrafficDestination_ASN_Layout.Location = new System.Drawing.Point(3, 28);
			this.TrafficDestination_ASN_Layout.Name = "TrafficDestination_ASN_Layout";
			this.TrafficDestination_ASN_Layout.Padding = new System.Windows.Forms.Padding(32, 0, 0, 0);
			this.TrafficDestination_ASN_Layout.RowCount = 2;
			this.TrafficDestination_ASN_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TrafficDestination_ASN_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TrafficDestination_ASN_Layout.Size = new System.Drawing.Size(608, 148);
			this.TrafficDestination_ASN_Layout.TabIndex = 1;
			// 
			// ASNSearch
			// 
			this.ASNSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ASNSearch.Location = new System.Drawing.Point(35, 3);
			this.ASNSearch.Name = "ASNSearch";
			this.ASNSearch.Size = new System.Drawing.Size(570, 23);
			this.ASNSearch.TabIndex = 0;
			// 
			// ASNSearchResults
			// 
			this.ASNSearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ASNSearchResults.FormattingEnabled = true;
			this.ASNSearchResults.ItemHeight = 15;
			this.ASNSearchResults.Location = new System.Drawing.Point(35, 32);
			this.ASNSearchResults.Name = "ASNSearchResults";
			this.ASNSearchResults.Size = new System.Drawing.Size(570, 113);
			this.ASNSearchResults.TabIndex = 1;
			// 
			// Destination_
			// 
			this.Destination_.AutoSize = true;
			this.Destination_.Location = new System.Drawing.Point(3, 182);
			this.Destination_.Name = "Destination_";
			this.Destination_.Size = new System.Drawing.Size(156, 19);
			this.Destination_.TabIndex = 2;
			this.Destination_.TabStop = true;
			this.Destination_.Text = "All IPs for a specific ASN:";
			this.Destination_.UseVisualStyleBackColor = true;
			// 
			// Page_Destination_ASN_Layout
			// 
			this.Page_Destination_ASN_Layout.ColumnCount = 1;
			this.Page_Destination_ASN_Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.Page_Destination_ASN_Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.Page_Destination_ASN_Layout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Page_Destination_ASN_Layout.Location = new System.Drawing.Point(3, 207);
			this.Page_Destination_ASN_Layout.Name = "Page_Destination_ASN_Layout";
			this.Page_Destination_ASN_Layout.Padding = new System.Windows.Forms.Padding(32, 0, 0, 0);
			this.Page_Destination_ASN_Layout.RowCount = 2;
			this.Page_Destination_ASN_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.Page_Destination_ASN_Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.Page_Destination_ASN_Layout.Size = new System.Drawing.Size(608, 100);
			this.Page_Destination_ASN_Layout.TabIndex = 3;
			// 
			// Page_Via
			// 
			this.Page_Via.Name = "Page_Via";
			this.Page_Via.Size = new System.Drawing.Size(614, 350);
			this.Page_Via.TabIndex = 2;
			this.Page_Via.Text = "Route traffic to";
			// 
			// Page_Finished
			// 
			this.Page_Finished.IsFinishPage = true;
			this.Page_Finished.Name = "Page_Finished";
			this.Page_Finished.Size = new System.Drawing.Size(614, 350);
			this.Page_Finished.TabIndex = 3;
			this.Page_Finished.Text = "wizardPage4";
			// 
			// AddStaticRouteWizard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(661, 498);
			this.Controls.Add(this.Wizard);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddStaticRouteWizard";
			this.Text = "AddStaticRouteWizard";
			((System.ComponentModel.ISupportInitialize)(this.Wizard)).EndInit();
			this.Page_Name.ResumeLayout(false);
			this.Page_Name.PerformLayout();
			this.themedTableLayoutPanel1.ResumeLayout(false);
			this.themedTableLayoutPanel1.PerformLayout();
			this.Page_Destination.ResumeLayout(false);
			this.Page_Destination_Layout.ResumeLayout(false);
			this.Page_Destination_Layout.PerformLayout();
			this.TrafficDestination_ASN_Layout.ResumeLayout(false);
			this.TrafficDestination_ASN_Layout.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private AeroWizard.WizardControl Wizard;
		private AeroWizard.WizardPage Page_Name;
		private AeroWizard.WizardPage Page_Destination;
		private AeroWizard.WizardPage Page_Via;
		private AeroWizard.WizardPage Page_Finished;
		private AeroWizard.ThemedTableLayoutPanel themedTableLayoutPanel1;
		private AeroWizard.ThemedLabel themedLabel1;
		private System.Windows.Forms.TextBox Wizard_Page_Name_NewName;
		private AeroWizard.ThemedTableLayoutPanel Page_Destination_Layout;
		private System.Windows.Forms.RadioButton Destination_Organization;
		private System.Windows.Forms.TableLayoutPanel TrafficDestination_ASN_Layout;
		private System.Windows.Forms.TextBox ASNSearch;
		private System.Windows.Forms.ListBox ASNSearchResults;
		private System.Windows.Forms.RadioButton Destination_;
		private System.Windows.Forms.TableLayoutPanel Page_Destination_ASN_Layout;
	}
}