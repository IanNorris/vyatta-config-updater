using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vyatta_config_updater
{
	public partial class DNSCryptResolverPicker : Form
	{
		private string[] IgnoreColumns = { "Name", "Full name", "Coordinates", "Namecoin", "Resolver address", "Provider name", "Provider public key", "Provider public key TXT record" };

		private ListViewColumnSorter ColumnSorter;
		private string PickedResolver;

		public string GetPickedResolver()
		{
			return PickedResolver;
		}

		public DNSCryptResolverPicker( string ResolverFilename )
		{
			InitializeComponent();
			ColumnSorter = new ListViewColumnSorter();
			ResolverList.ListViewItemSorter = ColumnSorter;
			
			int NameColumn = -1;
			int FullNameColumn = -1;
			int URLColumn = -1;
			List<int> IgnoreColumnIndices = new List<int>();

			var Trim = new char[] { '\"', ' ' };
			var CSVSplitRegex = new Regex( ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))" );

			var NameHeader = new ColumnHeader();
			NameHeader.Text = "Name";
			ResolverList.Columns.Add( NameHeader );

			int Index = 0;
			string[] Lines = System.IO.File.ReadAllLines( ResolverFilename );
			foreach( var Line in Lines )
			{
				string[] Elements = CSVSplitRegex.Split( Line );
				int ElementsCount = Elements.Count();
				for( int i = 0; i < ElementsCount; i++ )
				{
					Elements[i] = Elements[i].Trim( Trim );
				}

				if( Index == 0 )
				{
					int ColIndex = 0;
					foreach( string Elem in Elements )
					{
						if( Elem.Equals( "Name" ) )
						{
							NameColumn = ColIndex;
						}

						if( Elem.Equals( "Full name" ) )
						{
							FullNameColumn = ColIndex;
						}

						if( Elem.Equals( "URL" ) )
						{
							URLColumn = ColIndex;
						}

						bool ShouldIgnore = false;
						foreach( string Ignore in IgnoreColumns )
						{
							if( Elem.StartsWith( Ignore ) )
							{
								ShouldIgnore = true;
								IgnoreColumnIndices.Add( ColIndex );
								break;
							}
						}

						if( !ShouldIgnore )
						{
							var Header = new ColumnHeader();
							Header.Text = Elem;

							ResolverList.Columns.Add( Header );
						}

						ColIndex++;
					}
				}
				else
				{
					int URLIndex = -1;
					int ColIndex = 0;
					List<string> RemainingColData = new List<string>();
					foreach( string Elem in Elements )
					{
						if( ColIndex == URLColumn )
						{
							URLIndex = RemainingColData.Count;
						}

						if( IgnoreColumnIndices.IndexOf( ColIndex ) == -1 )
						{
							RemainingColData.Add( Elem );
						}
						
						ColIndex++;
					}

					var Item = new ListViewItem();
					Item.Text = Elements[ FullNameColumn ];

					int DataIndex = 0;
					foreach( var Data in RemainingColData )
					{
						if( DataIndex == URLIndex )
						{
							var SubItem = new ListViewItem.ListViewSubItem();
							SubItem.ForeColor = Color.Blue;
							SubItem.Text = RemainingColData[DataIndex];
							SubItem.Font = new Font( SubItem.Font, FontStyle.Underline );

							Item.SubItems.Add( SubItem );
						}
						else
						{
							Item.SubItems.Add( RemainingColData[DataIndex] );
						}

						DataIndex++;
					}

					Item.UseItemStyleForSubItems = false;
					Item.SubItems.AddRange( RemainingColData.ToArray() );
					Item.Tag = Elements[ NameColumn ];

					ResolverList.Items.Add( Item );
				}

				Index++;
			}

			ResolverList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		private void DNSCryptResolverPicker_Load( object sender, EventArgs e )
		{

		}

		private void ResolverList_SelectedIndexChanged( object sender, EventArgs e )
		{
			OK.Enabled = ResolverList.SelectedItems.Count > 0;
		}

		private void ResolverList_MouseDown( object sender, MouseEventArgs e )
		{
			var MouseEvent = e as MouseEventArgs;

			var Result = ResolverList.HitTest( MouseEvent.X, MouseEvent.Y );
			if( Result.SubItem != null )
			{
				if( Result.SubItem.Text.StartsWith( "http" ) )
				{
					System.Diagnostics.Process.Start( Result.SubItem.Text );
				}
			}
		}

		private void ResolverList_MouseMove( object sender, MouseEventArgs e )
		{
			var MouseEvent = e as MouseEventArgs;

			var Result = ResolverList.HitTest( MouseEvent.X, MouseEvent.Y );
			if( Result.SubItem != null )
			{
				if( Result.SubItem.Text.StartsWith( "http://" ) || Result.SubItem.Text.StartsWith( "https://" ) )
				{
					ResolverList.Cursor = Cursors.Hand;
				}
				else
				{
					ResolverList.Cursor = Cursors.Default;
				}
			}
		}

		private void ResolverList_ColumnClick( object sender, ColumnClickEventArgs e )
		{
			if ( e.Column == ColumnSorter.SortColumn )
			{
				// Reverse the current sort direction for this column.
				if (ColumnSorter.Order == SortOrder.Ascending)
				{
					ColumnSorter.Order = SortOrder.Descending;
				}
				else
				{
					ColumnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				ColumnSorter.SortColumn = e.Column;
				ColumnSorter.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			this.ResolverList.Sort();
		}

		private void OK_Click( object sender, EventArgs e )
		{
			PickedResolver = (string)ResolverList.SelectedItems[0].Tag;
		}
	}
}
