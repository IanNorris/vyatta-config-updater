using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vyatta_config_updater
{
	public partial class DNSLogViewer : Form
	{
		private ListViewColumnSorter ColumnSorter;

		private string GetASNStringFromIP( RouterData Data, string IP, out string Netmask )
		{
			string ASN;
			string Org;

			if( Data.ASNData.GetMatchingASNAndOrgForIP( IP, out ASN, out Org, out Netmask ) )
			{
				return string.Format( "{0} ({1})", Org, ASN );
			}

			return "Unknown";
		}

		public DNSLogViewer( string LogFilename, RouterData Data )
		{
			InitializeComponent();

			HashSet<string> ExistingItems = new HashSet<string>();

			ColumnSorter = new ListViewColumnSorter();
			RecordList.ListViewItemSorter = ColumnSorter;

			string[] LogData = File.ReadAllLines( LogFilename );

			var Regex = new Regex( @"^\w+\W+\d+\W+[0-9:]+\W+dnsmasq\[\d+\]:\W+(\w+(?:\[\w+\])?)\W+([^ ]+)\W+\w+\W+([0-9.]+|<\w+>)$" );
			
			string ParentQuery = "";
			string QueryFrom = "";
			List<string> CurrentIPs = new List<string>();
			List<string> Owners = new List<string>();
			List<string> Netmasks = new List<string>();
			foreach( var Line in LogData )
			{
				Match Match = Regex.Match( Line );
				if( !Match.Success )
				{
					continue;
				}

				var Type = Match.Groups[1].Value;
				var Domain = Match.Groups[2].Value;
				var IP = Match.Groups[3].Value;

				if( Type == "query" || Type == "query[A]" )
				{
					if( CurrentIPs.Count > 0 )
					{
						ListViewItem Item = new ListViewItem();
						Item.Text = ParentQuery;

						string IPs = "";
						bool first = true;
						foreach( string s in CurrentIPs )
						{
							if( first )
							{
								first = false;
							}
							else
							{
								IPs += ", ";
							}
							IPs += s;
						}

						string OwnersString = "";
						first = true;
						foreach( string s in Owners )
						{
							if( first )
							{
								first = false;
							}
							else
							{
								OwnersString += ", ";
							}
							OwnersString += s;
						}

						string NetmasksString = "";
						first = true;
						foreach( string s in Netmasks )
						{
							if( first )
							{
								first = false;
							}
							else
							{
								NetmasksString += ", ";
							}
							NetmasksString += s;
						}

						Item.SubItems.Add( IPs );
						Item.SubItems.Add( QueryFrom );
						Item.SubItems.Add( OwnersString );
						Item.SubItems.Add( NetmasksString );
						
						RecordList.Items.Add( Item );

						Owners.Clear();
						CurrentIPs.Clear();
						Netmasks.Clear();
					}

					ParentQuery = Domain;
					QueryFrom = IP;
				}
				else if( Type == "forwarded" )
				{
					//Ignore for now
				}
				else if( Type == "reply" || Type == "cached" )
				{
					if( IP != "<CNAME>" )
					{
						if( !CurrentIPs.Contains( IP ) )
						{
							CurrentIPs.Add( IP );
						}

						string Netmask = "";
						string Owner = GetASNStringFromIP( Data, IP, out Netmask );
						if( !Owners.Contains( Owner ) )
						{
							Owners.Add( Owner );
						}

						if( !Netmasks.Contains( Netmask) )
						{
							Netmasks.Add( Netmask );
						}
					}
				}
			}

			if( ParentQuery != "" )
			{
				ListViewItem Item = new ListViewItem();
				Item.Text = ParentQuery;

				string IPs = "";
				bool first = true;
				foreach( string s in CurrentIPs )
				{
					if( first )
					{
						first = false;
					}
					else
					{
						IPs += ", ";
					}
					IPs += s;
				}

				string OwnersString = "";
				first = true;
				foreach( string s in Owners )
				{
					if( first )
					{
						first = false;
					}
					else
					{
						OwnersString += ", ";
					}
					OwnersString += s;
				}

				string NetmasksString = "";
				first = true;
				foreach( string s in Netmasks )
				{
					if( first )
					{
						first = false;
					}
					else
					{
						NetmasksString += ", ";
					}
					NetmasksString += s;
				}

				Item.SubItems.Add( IPs );
				Item.SubItems.Add( QueryFrom );
				Item.SubItems.Add( OwnersString );
				Item.SubItems.Add( NetmasksString );
						
				RecordList.Items.Add( Item );
			}
		}

		private void OK_Click( object sender, EventArgs e )
		{

		}

		private void RecordList_ColumnClick( object sender, ColumnClickEventArgs e )
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
			this.RecordList.Sort();
		}
	}
}
