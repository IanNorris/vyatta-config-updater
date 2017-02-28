using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater
{
	public struct InterfaceMapping
	{
		public string Interface;
		public string IPAddress;
		public string Codes;
		public string Description;
		public string Gateway;

		public override string ToString()
		{
			if( Description.Length > 0 )
			{
				if( IPAddress.Length > 0 )
				{
					return $"{Description} ({Interface} - {IPAddress})";
				}
				else
				{
					return $"{Description} ({Interface})";
				}
			}
			else
			{
				if( IPAddress.Length > 0 )
				{
					return $"{Interface} - {IPAddress}";
				}
				else
				{
					return $"{Interface}";
				}
			}
		}
	}
}
