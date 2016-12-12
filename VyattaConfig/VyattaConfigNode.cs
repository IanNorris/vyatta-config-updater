using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater
{
	public interface VyattaConfigNode
	{
		void ToString( StringBuilder Output, int Indent );
	}
}
