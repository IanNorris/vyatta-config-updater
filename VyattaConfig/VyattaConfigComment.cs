using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig
{
	public class VyattaConfigComment : VyattaConfigNode
	{
		string Comment;

		public VyattaConfigComment( string Comment )
		{
			this.Comment = Comment;
		}

		public void ToString( StringBuilder Output, int Indent )
		{
			Output.Append( "/* " + Comment + " */" );
		}
	}
}
