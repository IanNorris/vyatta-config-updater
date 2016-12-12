using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vyatta_config_updater.VyattaConfig;

namespace vyatta_config_updater
{
	public interface VyattaConfigNode
	{
		string GetName();
		void ToString( StringBuilder Output, int Indent );
		string ToString();
		string GetAttributeString();
		VyattaConfigNode GetChild( string Child );
		VyattaConfigAttribute AddAttribute( string Parent );
		VyattaConfigObject AddObject( string Parent );
		void Delete( string Path );
	}
}
