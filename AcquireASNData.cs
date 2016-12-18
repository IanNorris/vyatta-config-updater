﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace vyatta_config_updater
{
	public class ASNData
	{
		public Dictionary<string, List<int>> OwnerToASN = new Dictionary<string, List<int>>();
		public Dictionary<int, List<string>> ASNToNetmask = new Dictionary<int, List<string>>();

		public List<int> GetASNByOrganization( string Substring )
		{
			Regex Regex = new Regex( string.Format( @".*\b{0}\b.*", Substring.ToLower() ) );
			
			List<int> ASNs = new List<int>();

			OwnerToASN.Any( 
				Pair =>
			{
				if( Regex.IsMatch( Pair.Key.ToLower() ) )
				{
					ASNs.AddRange( Pair.Value );
				}
				return false;
			} );

			return ASNs;
		}

		public List<string> GetNetmasksFromOrganization( string Organization )
		{
			List<int> ASNs = GetASNByOrganization( Organization );
			return GetNetmasksFromASNs( ASNs );
		}

		public List<string> GetNetmasksFromASNs( List<int> ASNs )
		{
			List<string> Result = new List<string>();

			foreach( int ASN in ASNs )
			{
				List<string> NetmasksForASN;

				if( ASNToNetmask.TryGetValue( ASN, out NetmasksForASN ) )
				{
					Result.AddRange( NetmasksForASN );
				}
			}

			return Result;
		}
	}

	public class AcquireASNData : BusyWorkInterface
	{
		const int AgeInMonths = 3;

		AutoResetEvent WaitHandle = new AutoResetEvent(false);

		const int SectionCount = 3;
		int SectionsCompleted = 0;

		string Filename;
		Util.UpdateStatusDelegate SetStatus;
		Util.ShouldCancelDelegate ShouldCancel;
		string AppDataPath;
		bool Aborted = false;

		ASNData ASNDataOutput;

		public AcquireASNData( ASNData Data )
		{
			ASNDataOutput = Data;
			AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\VyattaConfigUpdater\";

			System.IO.Directory.CreateDirectory( AppDataPath );
		}

		bool DownloadFile( string RemoteFilename, string LocalFilename )
		{
			FileInfo FileInfo = new FileInfo( LocalFilename );
			if( FileInfo != null )
			{
				if( FileInfo.LastWriteTime > DateTime.Now.AddMonths(-AgeInMonths) )
				{
					return true;
				}
			}
						
			this.Filename = RemoteFilename;
			WebClient WebClient = new WebClient();
			WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
			WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
			WebClient.DownloadFileAsync(new Uri(Filename), LocalFilename );

			WaitHandle.WaitOne();
			WaitHandle.Reset();

			if( Aborted )
			{
				return false;
			}

			return true;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			this.SetStatus = SetStatus;
			this.ShouldCancel = ShouldCancel;

			SetStatus( "Starting downloads...", 0 );

			string NetmaskToASN = AppDataPath + "NetmaskToASN";
			string ASNToOwner = AppDataPath + "ASNToOwner";

			if( !DownloadFile( @"http://thyme.apnic.net/current/data-raw-table", NetmaskToASN ) )
			{
				return false;
			}
			SectionsCompleted++;

			if( !DownloadFile( @"http://thyme.apnic.net/current/data-used-autnums", ASNToOwner ) )
			{
				return false;
			}
			SectionsCompleted++;

			SetStatus( "Reading ASN data...", ProgressIntForSection(0) );

			string[] ASNToOwnerLines = File.ReadAllLines( ASNToOwner );
			string[] NetmaskToASNLines = File.ReadAllLines( NetmaskToASN );

			Regex ASNToOwnerRegex = new Regex( @"^\s*([0-9]+)\s+(.*)$" );
			foreach( var Line in ASNToOwnerLines )
			{
				Match Match = ASNToOwnerRegex.Match( Line );
				if( !Match.Success )
				{
					throw new Exception( string.Format( "Failed to parse ASN to owner data file on line {0}", Line ) );
				}

				List<int> TargetList;
				if( !ASNDataOutput.OwnerToASN.TryGetValue( Match.Groups[2].Value, out TargetList ) )
				{
					TargetList = new List<int>();
					ASNDataOutput.OwnerToASN.Add( Match.Groups[2].Value, TargetList );
				}

				TargetList.Add( Convert.ToInt32( Match.Groups[1].Value ) );
			}

			Regex NetmaskToASNRegex = new Regex( @"^([0-9.]+/\d+)\s+(\d+)$" );
			foreach( var Line in NetmaskToASNLines )
			{
				Match Match = NetmaskToASNRegex.Match( Line );
				if( !Match.Success )
				{
					throw new Exception( string.Format( "Failed to parse netmask to ASN data file on line {0}", Line ) );
				}

				List<string> TargetList;
				if( !ASNDataOutput.ASNToNetmask.TryGetValue( Convert.ToInt32( Match.Groups[2].Value ), out TargetList ) )
				{
					TargetList = new List<string>();
					ASNDataOutput.ASNToNetmask.Add( Convert.ToInt32( Match.Groups[2].Value ), TargetList );
				}

				TargetList.Add( Match.Groups[1].Value );
			}

			SetStatus( "Processing ASN data...", ProgressIntForSection(20) );


			
			SectionsCompleted++;
	
			SetStatus( "Downloads complete.", 100 );

			return true;
		}

		private int ProgressIntForSection( int Progress )
		{
			float TotalProgress = ((float)SectionsCompleted / (float)SectionCount) + ( (float)Progress / (100.0f * (float)SectionCount) );
			TotalProgress *= 100.0f;

			return (int)TotalProgress;
		}

		private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			if( ShouldCancel() )
			{
				((WebClient)sender).CancelAsync();
				Aborted = true;
				WaitHandle.Set();
			}
			
			SetStatus( string.Format( "Downloading {0} - {1}%...", Filename, e.ProgressPercentage ), ProgressIntForSection( e.ProgressPercentage ) );
		}

		private void Completed(object sender, AsyncCompletedEventArgs e)
		{
			WaitHandle.Set();
		}
	}
}
