using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig
{
	class VyattaConfigValue : VyattaConfigNode
	{
		string Value;
		bool Quoted;

		public VyattaConfigValue( string Value, bool Quoted )
		{
			this.Value = Value;
			this.Quoted = Quoted;
		}

		public void ToString( StringBuilder Output, int Indent )
		{
			if( Quoted )
			{
				Output.Append( "\"" );
			}

			Output.Append( Value );

			if( Quoted )
			{
				Output.Append( "\"" );
			}
		}
	}
}
