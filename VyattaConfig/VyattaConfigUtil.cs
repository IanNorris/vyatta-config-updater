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
		public const string SortableDateFormat = "yyyy_MM_dd_HH_mm_ss";

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

			//Just in case we get a file with rogue line endings
			InputBuffer = InputBuffer.Replace( "\r\n", "\n" );

			Errors = "";

			var Parser = new VyattaConfigParser( InputBuffer );
			var Root = new VyattaConfigObject( null );
			Parser.Parse( ref Errors, Root );

			return Root;
		}

		public static string WriteToString( VyattaConfigNode Root )
		{
			StringBuilder SB = new StringBuilder();
			
			Root.ToString( SB, -1 );

			return SB.ToString();
		}

		public static System.String[] WriteToStringLines( VyattaConfigNode Root )
		{
			string Result = WriteToString( Root );
			Result = Result.TrimEnd( new char[] { '\n' } );
			return Result.Split( new char[] { '\n' } );
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
	}
}
