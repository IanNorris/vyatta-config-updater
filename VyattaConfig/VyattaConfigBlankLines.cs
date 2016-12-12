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
	}
}
