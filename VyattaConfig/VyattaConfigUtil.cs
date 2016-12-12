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
	}
}
