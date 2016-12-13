using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig
{
	public static class VyattaConfigUtil
	{
		public static string IndentString( int Indent )
		{
			if( Indent > 0 )
			{
				return new string( ' ', Indent * 4 );
			}
			else
			{
				return "";
			}
		}

		public static VyattaConfigObject ReadFromFile( string Filename, ref string Errors )
		{
			var InputBuffer = File.ReadAllText( Filename );

			Errors = "";

			var Parser = new VyattaConfigParser( InputBuffer );
			var Root = new VyattaConfigObject( null );
			Parser.Parse( ref Errors, Root );

			return Root;
		}

		public static void WriteToFile( VyattaConfigNode Root, string TargetFilename )
		{
			StringBuilder SB = new StringBuilder();

			Root.ToString( SB, -1 );

			using (TextWriter FileOut = new StreamWriter( TargetFilename ))
			{
				FileOut.NewLine = "\n";            
				FileOut.Write( SB.ToString() );
			}
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
}
