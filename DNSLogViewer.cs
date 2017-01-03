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
		public DNSLogViewer( string LogFilename, RouterData Data )
		{
			InitializeComponent();

			string[] LogData = File.ReadAllLines( LogFilename );

			var Regex = new Regex( @"^\w+\W+\d+\W+[0-9:]+\W+dnsmasq\[\d+\]:\W+(\w+(?:\[\w+\])?)\W+([^ ]+)\W+\w+\W+([0-9.]+|<\w+>)$" );
			
			string ParentQuery = "";
			string QueryFrom = "";
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
						string Owner = "";
						string ASN;
						string Org;

						if( Data.ASNData.GetMatchingASNAndOrgForIP( IP, out ASN, out Org ) )
						{
							Owner = string.Format( "{0} ({1})", Org, ASN );
						}

						//Ignore for now
						ListViewItem Item = new ListViewItem();
						Item.Text = ParentQuery;
						Item.SubItems.Add( Domain );
						Item.SubItems.Add( IP );
						Item.SubItems.Add( QueryFrom );
						Item.SubItems.Add( Owner );
						
						RecordList.Items.Add( Item );
					}
				}
			}
		}

		private void OK_Click( object sender, EventArgs e )
		{

		}
	}
}
