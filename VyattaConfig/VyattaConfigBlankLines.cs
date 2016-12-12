using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig
{
	public class VyattaConfigBlankLines : VyattaConfigNode
	{
		int BlankLines;

		public VyattaConfigBlankLines( int BlankLines )
		{
			this.BlankLines = BlankLines;
		}

		public void ToString( StringBuilder Output, int Indent )
		{
			Output.Append( new string( '\n', BlankLines ) );
		}

		public override string ToString()
		{
			StringBuilder SB = new StringBuilder();
			ToString( SB, 0 );
			return SB.ToString();
		}

		public string GetAttributeString()
		{
			return null;
		}

		public VyattaConfigNode GetChild( string Child )
		{
			return null;
		}

		public VyattaConfigAttribute AddAttribute( string Parent )
		{
			return null;
		}

		public VyattaConfigObject AddObject( string Parent )
		{
			return null;
		}

		public void Delete( string Path )
		{
		}

		public string GetName()
		{
			return null;
		}
	}
}
