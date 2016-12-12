using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig
{
	public class VyattaConfigAttribute : VyattaConfigNode
	{
		string Name;
		List<VyattaConfigValue> Children = new List<VyattaConfigValue>();

		public VyattaConfigAttribute( string Name )
		{
			this.Name = Name;
		}

		public void Add( string Value, bool Quoted )
		{
			Children.Add( new VyattaConfigValue( Value, Quoted ) );
		}

		public void ToString( StringBuilder Output, int Indent )
		{
			Output.Append( VyattaConfigUtil.IndentString( Indent ) );

			Output.Append( Name );

			foreach( var Child in Children )
			{
				Output.Append( " " );

				Child.ToString( Output, 0 );
			}
		}
	}
}
