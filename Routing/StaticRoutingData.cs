using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vyatta_config_updater
{
	public enum RoutingType
	{
		ASN,
		Organisation,
		Netmask,
		NetmaskArray,
	}

	public struct StaticRoutingData
	{
		public string Name;
		public RoutingType Type;
		public string Destination;
		public string Interface;
	}
}
