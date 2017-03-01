using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace vyatta_config_updater.Routing
{
	public struct Netmask
	{
		public string NetmaskString;
		public UInt32 MaskBits;
		public UInt32 MaskValue;
		public UInt32 MaskedAddress;
		public UInt32 ASN; //Only gets set by ASNData

		public UInt32 IPCount
		{
			get
			{
				UInt32 RangeMax = ~0U - 1;

				UInt32 MinValue = MaskedAddress & MaskValue;
				UInt32 MaxValue = (MaskedAddress & MaskValue) | ~MaskValue;

				MinValue &= RangeMax;
				MaxValue &= RangeMax;

				return Math.Max(MaxValue-MinValue,1);
			}
		}

		public static UInt32 NetmaskValueFromGenmask( string Genmask )
		{
			var NetmaskSegments = Genmask.Split( new char[] { '.' }, 4 );

			UInt32 NetmaskValue = 0;

			byte TempSegment = 0;
			for( int seg = 0; seg < 4; seg++ )
			{
				if( !byte.TryParse( NetmaskSegments[seg], out TempSegment ) )
				{
					return 0;
				}

				NetmaskValue = NetmaskValue << 8;
				NetmaskValue |= TempSegment;
			}

			return NetmaskValue;
		}

		public static uint MaskFromGenmask( string Genmask )
		{
			UInt32 NetmaskValue = NetmaskValueFromGenmask( Genmask );

			UInt32 ComparisonMask = 0;
			uint BitsSet = 0;
			bool AllSet = true;
			bool Reset = false;
			for( uint i = 0; i < 32; i++ )
			{
				ComparisonMask |= (UInt32)(1 << (int)(31 - i));

				if( (NetmaskValue & ComparisonMask) == ComparisonMask )
				{
					BitsSet = i+1;

					if( !AllSet )
					{
						Reset = true;
					}
				}
				else
				{
					AllSet = false;
				}
			}

			if( Reset && !AllSet )
			{
				throw new Exception( string.Format( "Genmask {0} cannot be converted to a mask as set bits are not contiguous.", Genmask ) );
			}

			return BitsSet;
		}

		public static Netmask? GetNetmaskFromIPAndGenmask( string IP, string Genmask )
		{
			Netmask Result = new Netmask();
			
			var IPSegments = IP.Split( new char[] { '.' }, 4 );

			uint Mask = MaskFromGenmask( Genmask );
						
			UInt32 NetmaskValue = 0;
			UInt32 MaskValue = Mask == 32 ? (~(UInt32)0) : (~(((UInt32)1 << (int)(32 - Mask))-1));

			byte TempSegment = 0;
			for( int seg = 0; seg < 4; seg++ )
			{
				if( !byte.TryParse( IPSegments[seg], out TempSegment ) )
				{
					return null;
				}

				NetmaskValue = NetmaskValue << 8;
				NetmaskValue |= TempSegment;
			}
			
			Result.MaskedAddress = NetmaskValue & MaskValue;
			Result.NetmaskString = string.Format( "{0}.{1}.{2}.{3}/{4}", 
				(Result.MaskedAddress >> 24) & 0xFF, 
				(Result.MaskedAddress >> 16) & 0xFF, 
				(Result.MaskedAddress >> 8 ) & 0xFF, 
				Result.MaskedAddress		 & 0xFF,
				Mask );
			Result.MaskBits = Mask;
			Result.MaskValue = MaskValue;

			return Result;
		}

		public static bool IsValidIP( string IP )
		{
			var Segments = IP.Split( new char[] { '.' }, 4 );
			if( Segments.Length != 4 )
			{
				return false;
			}
			
			byte TempSegment = 0;
			for( int seg = 0; seg < 4; seg++ )
			{
				if( !byte.TryParse( Segments[seg], out TempSegment ) )
				{
					return false;
				}
			}

			return true;
		}

		public static Netmask? GetNetmaskFromString( string Netmask )
		{
			Netmask Result = new Netmask();

			var NetmaskSplit = Netmask.Split( new char[] { '/' }, 2 );

			if( NetmaskSplit.Length != 2 )
			{
				return null;
			}

			var NetmaskSegments = NetmaskSplit[0].Split( new char[] { '.' }, 4 );
			if( NetmaskSegments.Length != 4 )
			{
				return null;
			}

			UInt32 Mask = 0;
			if( !UInt32.TryParse( NetmaskSplit[1], out Mask ) )
			{
				return null;
			}
			
			UInt32 NetmaskValue = 0;
			UInt32 MaskValue = Mask == 0 ? 0 : (~(((UInt32)1 << (int)(32 - Mask))-1));

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
			if( Segments.Length != 4)
			{
				return 0;
			}
						
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

		public static UInt32 NumberOfSetBits(UInt32 i)
		{
			i = i - ((i >> 1) & 0x55555555);
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
			return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
		}

		public static string IPRangeToCIDR( string Range )
		{
			string[] Split = Range.Split( new char[] { '-' }, 2 );
			if( Split.Length != 2 )
			{
				return null;
			}

			long MaskBits = 0;

			string[] IP1Segments = Split[0].Split( new char[] { '.' }, 4 );
			string[] IP2Segments = Split[1].Split( new char[] { '.' }, 4 );
			for( int index = 0; index < 4; index++ )
			{
				int Segment = int.Parse(IP2Segments[index]) - int.Parse(IP1Segments[index]);
				MaskBits |= (long)(Segment << (4-index));
			}

			UInt32 Mask = 32 - NumberOfSetBits((UInt32)MaskBits);

			return string.Format( "{0}/{1}", Split[0], Mask );
		}
	}

	public struct Route
	{
		public string Destination;
		public string Gateway;
		public string Genmask;
		public string Interface;

		public Netmask? CalculatedNetmask
		{
			get {
				return Netmask.GetNetmaskFromIPAndGenmask( Destination, Genmask );
			}
		}
	}

	public class IPRoute
	{
		private static Regex GatewayRegex = new Regex( @"([\w.]+)\W+([\d.]+|\*)\W+([\d.]+)\W+\w+\W+\d+\W+\d+\W+\d+\W+(\w+)" );

		private static List<Route> GetRoutes( VyattaShell Shell )
		{
			string ShowRoutes = Shell.RunCommand( "sudo route -n" );

			string[] RouteLines = ShowRoutes.Split( new char[] { '\n' } );

			List<Route> Routes = new List<Route>();

			foreach( string Line in RouteLines )
			{
				Match Match = GatewayRegex.Match( Line );
				if( Match.Success )
				{
					Route Route = new Route();
					Route.Destination	= Match.Groups[1].Value;
					Route.Gateway		= Match.Groups[2].Value;
					Route.Genmask		= Match.Groups[3].Value;
					Route.Interface		= Match.Groups[4].Value;

					Routes.Add( Route );
				}
			}

			return Routes;
		}

		public static Dictionary<string, string> GetDefaultGateways( VyattaShell Shell )
		{
			var Routes = GetRoutes( Shell );
			
			Dictionary<string, string> Gateways = new Dictionary<string, string>();			
			foreach( Route Route in Routes )
			{
				//Only match gateways to the internet
				if( (Route.Destination == "default" || Route.Destination == "0.0.0.0") &&  Route.Gateway != "0.0.0.0" )
				{
					if( Route.Genmask == "0.0.0.0" || Route.Genmask == "128.0.0.0" )
					{
						Gateways[Route.Interface] = Route.Gateway;
					}
				}
			}

			return Gateways;
		}

		public static List<Route> GetRoutesForInterface( VyattaShell Shell, bool DefaultOnly, string Interface = null )
		{
			var Routes = GetRoutes( Shell );

			List<Route> NewRoutes = new List<Route>();
					
			foreach( Route Route in Routes )
			{
				//Only match gateways to the internet
				if( (Interface == null || Route.Interface == Interface)  &&  (!DefaultOnly || Route.Gateway != "0.0.0.0") )
				{
					if( !DefaultOnly || (Route.Genmask == "0.0.0.0" || Route.Genmask == "128.0.0.0") )
					{
						NewRoutes.Add( Route );
					}
				}
			}

			return NewRoutes;
		}

		public static bool AddRoutes( VyattaShell Shell, List<Route> Routes )
		{
			foreach( Route Route in Routes )
			{
				string Result = Shell.RunCommand( string.Format( "ip route add {0} via {1} dev {2}", Route.CalculatedNetmask.Value, Route.Gateway, Route.Interface ) );

				//TODO: Check Result
			}

			return true;
		}

		public static bool DeleteRoutes( VyattaShell Shell, List<Route> Routes )
		{
			foreach( Route Route in Routes )
			{
				string Result = Shell.RunCommand( string.Format( "ip route del {0} via {1} dev {2}", Route.CalculatedNetmask.Value, Route.Gateway, Route.Interface ) );

				//TODO: Check Result
			}

			return true;
		}
	}
}
