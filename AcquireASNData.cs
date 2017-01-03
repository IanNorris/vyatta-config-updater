using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using static vyatta_config_updater.ASNData;

namespace vyatta_config_updater
{
	public class ASNData
	{
		public struct Netmask
		{
			public string NetmaskString;
			public UInt32 MaskBits;
			public UInt32 MaskValue;
			public UInt32 MaskedAddress;
		}

		public Dictionary<string, List<int>> OwnerToASN = new Dictionary<string, List<int>>();
		public Dictionary<int, string> ASNToOwner = new Dictionary<int, string>();
		public Dictionary<int, List<Netmask>> ASNToNetmask = new Dictionary<int, List<Netmask>>();

		public List<int> GetASNByOrganization( string Substring )
		{
			string RegexMatch = string.Format( @"\b{0}\b", Substring );
			
			List<int> ASNs = new List<int>();

			OwnerToASN.Any( 
				Pair =>
			{
				if( Regex.IsMatch( Pair.Key, RegexMatch, RegexOptions.IgnoreCase ) )
				{
					ASNs.AddRange( Pair.Value );
				}
				return false;
			} );

			return ASNs;
		}

		public List<Netmask> GetNetmasksFromOrganization( string Organization )
		{
			List<int> ASNs = GetASNByOrganization( Organization );
			return GetNetmasksFromASNs( ASNs );
		}

		public List<Netmask> GetNetmasksFromASN( int ASN )
		{
			List<int> ASNs = new List<int>() { ASN };
			return GetNetmasksFromASNs( ASNs );
		}

		public List<Netmask> GetNetmasksFromASNs( List<int> ASNs )
		{
			List<Netmask> Result = new List<Netmask>();

			foreach( int ASN in ASNs )
			{
				List<Netmask> NetmasksForASN;

				if( ASNToNetmask.TryGetValue( ASN, out NetmasksForASN ) )
				{
					Result.AddRange( NetmasksForASN );
				}
			}

			return Result;
		}

		public static Netmask? GetNetmaskFromString( string Netmask )
		{
			Netmask Result = new Netmask();

			var NetmaskSplit = Netmask.Split( new char[] { '/' }, 2 );
			var NetmaskSegments = NetmaskSplit[0].Split( new char[] { '.' }, 4 );

			UInt32 Mask = 0;
			if( !UInt32.TryParse( NetmaskSplit[1], out Mask ) )
			{
				return null;
			}
			
			UInt32 NetmaskValue = 0;
			UInt32 MaskValue = Mask == 32 ? (~(UInt32)0) : (~(((UInt32)1 << (int)(32 - Mask))-1));

			byte TempSegment = 0;
			for( int seg = 0; seg < 4; seg++ )
			{
				if( !byte.TryParse( NetmaskSegments[seg], out TempSegment ) )
				{
					return null;
				}

				NetmaskValue = NetmaskValue << 8;
				NetmaskValue |= TempSegment;
			}
			
			Result.NetmaskString = Netmask;
			Result.MaskBits = Mask;
			Result.MaskValue = MaskValue;
			Result.MaskedAddress = NetmaskValue & MaskValue;

			return Result;
		}

		public static UInt32 IPAsInteger( string IP )
		{
			var Segments = IP.Split( new char[] { '.' }, 4 );
						
			UInt32 IPValue = 0;
			
			byte TempSegment = 0;
			for( int seg = 0; seg < 4; seg++ )
			{
				if( !byte.TryParse( Segments[seg], out TempSegment ) )
				{
					return 0;
				}

				IPValue = IPValue << 8;
				IPValue |= TempSegment;
			}

			return IPValue;
		}

		public static bool IPMatchesNetmask( UInt32 IPAsInteger, Netmask Netmask )
		{
			return (IPAsInteger & Netmask.MaskValue) == (Netmask.MaskedAddress & Netmask.MaskValue);
		}

		public bool GetMatchingASNAndOrgForIP( string IP, out string ASN, out string Org )
		{
			UInt32 IPValue = IPAsInteger( IP );

			foreach( var Pair in ASNToNetmask )
			{
				foreach( var Netmask in Pair.Value )
				{
					if( IPMatchesNetmask( IPValue, Netmask ) )
					{
						ASN = Pair.Key.ToString();
						if( !ASNToOwner.TryGetValue( Pair.Key, out Org ) )
						{
							Org = "Unknown";
						}

						return true;
					}
				}
			}

			ASN = "";
			Org = "";

			return false;
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

				ASNDataOutput.ASNToOwner[ Convert.ToInt32( Match.Groups[1].Value ) ] = Match.Groups[2].Value;

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

				List<Netmask> TargetList;
				if( !ASNDataOutput.ASNToNetmask.TryGetValue( Convert.ToInt32( Match.Groups[2].Value ), out TargetList ) )
				{
					TargetList = new List<Netmask>();
					ASNDataOutput.ASNToNetmask.Add( Convert.ToInt32( Match.Groups[2].Value ), TargetList );
				}

				Netmask? Value = GetNetmaskFromString( Match.Groups[1].Value );
				
				if( Value.HasValue )
				{
					TargetList.Add( Value.Value );
				}
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
