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
using Menees;
using Menees.Diffs;
using Menees.Windows.Forms;
using vyatta_config_updater.VyattaConfig;

namespace vyatta_config_updater
{
	public partial class Main : Form
	{
		private RouterData Data;

		public Main( RouterData Data )
		{
			this.Data = Data;
			
			InitializeComponent();

			RoutingList.Items.Clear();

			foreach( var Item in Data.StaticRoutes )
			{
				RoutingList.Items.Add( CreateLVIFromStaticRoutingRule(Item) );
			}
			RoutingList.Refresh();

			Data.StaticRoutes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(
				delegate( object Sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs EventArgs )
				{
					if( EventArgs.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add )
					{
						foreach( var ItemObj in EventArgs.NewItems )
						{
							StaticRoutingData Item = (StaticRoutingData)ItemObj;

							RoutingList.Items.Add( CreateLVIFromStaticRoutingRule(Item) );
						}
					}
					else if( EventArgs.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove )
					{
						foreach( var ItemObj in EventArgs.OldItems )
						{
							StaticRoutingData Item = (StaticRoutingData)ItemObj;

							foreach( var ListItemObj in RoutingList.Items )
							{
								var ListViewItem = ListItemObj as ListViewItem;
								if( ListViewItem.Tag.Equals(Item) )
								{
									RoutingList.Items.Remove( ListViewItem );
									break;
								}
							}
						}
					}

					RefreshDiff();
				}
			);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			
			Application.Exit();
		}

		private void saveConfigToolStripMenuItem_Click( object sender, EventArgs e )
		{

		}

		private void RefreshDiff()
		{
			Data.NewConfigLines = VyattaConfigUtil.WriteToStringLines( Data.ConfigRoot );

			var Diff = new Menees.Diffs.TextDiff( HashType.HashCode, false, false, 0, false );
			var EditScript = Diff.Execute( Data.OldConfigLines, Data.NewConfigLines );

			ConfigDiff.SetData( Data.OldConfigLines, Data.NewConfigLines, EditScript, "Existing router config", "New router config", false, true, false );
			ConfigDiff.GoToFirstDiff();
		}

		private void makeLiveToolStripMenuItem_Click( object sender, EventArgs e )
		{
			/*var GenerateConfig = new RouterGenerateNewConfig( OldConfigPath, Data );

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
						var Busy = new Busy( new RouterWriteNewConfig( Data ) );
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
			}*/

			var Busy = new Busy( new RouterWriteNewConfig( Data ) );
			if( Busy.ShowDialog() == DialogResult.OK )
			{
				MessageBox.Show( "New config has been uploaded.\nYour router will now reload the config.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
			else
			{
				MessageBox.Show( "Error uploading new config..", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void Main_Load( object sender, EventArgs e )
		{
			RefreshDiff();
		}

		private ListViewItem CreateLVIFromStaticRoutingRule( StaticRoutingData Route, ListViewItem ExistingItem = null )
		{
			ListViewItem Item = ExistingItem != null ? ExistingItem : new ListViewItem();
			Item.SubItems.Clear();
			Item.Tag = Route;
			Item.Text = Route.Name;
			Item.SubItems.Add( Route.Type.ToString() );
			Item.SubItems.Add( Route.Destination );
			Item.SubItems.Add( Route.Interface );

			return Item;
		}

		private void removeToolStripMenuItem_Click( object sender, EventArgs e )
		{
			foreach( var ItemObj in RoutingList.SelectedItems )
			{
				var Item = ItemObj as ListViewItem;
				if( Item != null )
				{
					var Route = (StaticRoutingData)Item.Tag;
					Data.StaticRoutes.Remove( Route );
				}
			}
		}

		private void RougingListContextMenu_Opening( object sender, CancelEventArgs e )
		{
			RoutingList_ContextMenu_Remove.Enabled = RoutingList.SelectedItems.Count > 0;
		}

		private void RoutingList_ContextMenu_Add_Click( object sender, EventArgs e )
		{
			AddStaticRouteWizard Wizard = new AddStaticRouteWizard();
			if( Wizard.ShowDialog() == DialogResult.OK )
			{
				Data.StaticRoutes.Add( Wizard.GetResult() );
			}
		}

		private void enableDNSCryptToolStripMenuItem_Click( object sender, EventArgs e )
		{
			if( MessageBox.Show( 
				"You are about to enable DNSCrypt that will encrypt your DNS traffic\n" + 
				"so that it cannot be snooped on by your ISP.\n" + 
				"This process takes a while and could possibly go wrong\n" + 
				"leaving your router in an unknown state.\n" +
				"Are you sure you wish to continue?", 
				"Confirm enabling DNS crypt?", 
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				var Command = new RouterEnableDNSCrypt( Data, this );

				Busy BusyWorker = new Busy( Command );

				if( BusyWorker.ShowDialog() == DialogResult.OK )
				{
					MessageBox.Show( "DNSCrypt has been enabled!\nIt is recommended that you now confirm this\nby using DNSLeakTest.com", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information );
				}
			}
		}

		private void logDNSQueriesToolStripMenuItem_Click_1( object sender, EventArgs e )
		{
			var Command = new RouterLogDNS( Data );

			Busy BusyWorker = new Busy( Command );

			if( BusyWorker.ShowDialog() == DialogResult.OK )
			{
				var ResultForm = new DNSLogViewer( Command.GetLogPath(), Data );
				ResultForm.ShowDialog();
			}
		}

		private void openDNSLogToolStripMenuItem_Click( object sender, EventArgs e )
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = "All files (*.*)|*.*";
			openFile.FilterIndex = 1;
			openFile.RestoreDirectory = true;

			if( openFile.ShowDialog() == DialogResult.OK )
			{
				var ResultForm = new DNSLogViewer( openFile.FileName, Data );
				ResultForm.ShowDialog();
			}
		}
	}
}
