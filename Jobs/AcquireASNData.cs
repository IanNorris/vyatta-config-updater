using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using vyatta_config_updater.Routing;
using static vyatta_config_updater.ASNData;

namespace vyatta_config_updater
{
	public class ASNData
	{
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

		

		public bool GetMatchingASNAndOrgForIP( string IP, out string ASN, out string Org, out string NetmaskString )
		{
			UInt32 IPValue = Netmask.IPAsInteger( IP );

			foreach( var Pair in ASNToNetmask )
			{
				foreach( var CurrentNetmask in Pair.Value )
				{
					if( Netmask.IPMatchesNetmask( IPValue, CurrentNetmask ) )
					{
						NetmaskString = CurrentNetmask.NetmaskString;

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
			NetmaskString = "";

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

			char[] Separators = new char[] { '\t' };

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

			if( File.Exists( NetmaskToASN + ".bin" ) )
			{
				using( MemoryStream NetmaskToASNBinMS = new MemoryStream( File.ReadAllBytes( NetmaskToASN + ".bin" ) ) )
				using( BinaryReader NetmaskToASNBin = new BinaryReader( NetmaskToASNBinMS ) )
				{
					while( NetmaskToASNBin.BaseStream.Position != NetmaskToASNBin.BaseStream.Length )
					{
						Int32 ASN = NetmaskToASNBin.ReadInt32();
						uint MaskedAddress = NetmaskToASNBin.ReadUInt32();
						byte MaskedBits = NetmaskToASNBin.ReadByte();
						string NetmaskString = NetmaskToASNBin.ReadString();

						Netmask Value = new Netmask();
						Value.MaskedAddress = MaskedAddress;
						Value.MaskBits = MaskedBits;
						Value.MaskValue = MaskedBits == 32 ? ( ~(UInt32)0 ) : ( ~( ( (UInt32)1 << (int)( 32 - MaskedBits ) ) - 1 ) );
						Value.NetmaskString = NetmaskString;

						List<Netmask> TargetList;
						if( !ASNDataOutput.ASNToNetmask.TryGetValue( ASN, out TargetList ) )
						{
							TargetList = new List<Netmask>();
							ASNDataOutput.ASNToNetmask.Add( ASN, TargetList );
						}

						TargetList.Add( Value );
					}
				}
			}
			else
			{
				string[] NetmaskToASNLines = File.ReadAllLines( NetmaskToASN );

				char[] NetmaskSplitSep = new char[] { '/' };
				char[] IPSep = new char[] { '.' };

				using( FileStream NetmaskToASNBinFS = new FileStream( NetmaskToASN + ".bin", FileMode.CreateNew ) )
				using( BinaryWriter NetmaskToASNBin = new BinaryWriter( NetmaskToASNBinFS ) )
				{
					foreach( var Line in NetmaskToASNLines )
					{
						string[] LineSplit = Line.Split( Separators );
						if( LineSplit.Length != 2 )
						{
							throw new Exception( string.Format( "Failed to parse netmask to ASN data file on line {0}", Line ) );
						}

						Int32 ASN;
						if( !Int32.TryParse( LineSplit[ 1 ], out ASN ) )
						{
							throw new Exception( string.Format( "Failed to parse ASN on line {0}", Line ) );
						}

						List<Netmask> TargetList;
						if( !ASNDataOutput.ASNToNetmask.TryGetValue( ASN, out TargetList ) )
						{
							TargetList = new List<Netmask>();
							ASNDataOutput.ASNToNetmask.Add( ASN, TargetList );
						}

						Netmask? Value = Netmask.GetNetmaskFromString( LineSplit[0] );

						if( Value.HasValue )
						{
							TargetList.Add( Value.Value );

							NetmaskToASNBin.Write( ASN );
							NetmaskToASNBin.Write( Value.Value.MaskedAddress );
							NetmaskToASNBin.Write( (byte)Value.Value.MaskBits );
							NetmaskToASNBin.Write( Value.Value.NetmaskString );
						}
					}
				}
			}

			string[] ASNToOwnerLines = File.ReadAllLines( ASNToOwner );
					

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
