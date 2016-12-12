using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig
{
	public class VyattaConfigObject : VyattaConfigNode
	{
		VyattaConfigAttribute Attribute;
		List<VyattaConfigNode> Children = new List<VyattaConfigNode>();

		public VyattaConfigObject( VyattaConfigAttribute Attribute )
		{
			this.Attribute = Attribute;
		}

		public void AddChild( VyattaConfigNode NewNode )
		{
			Children.Add( NewNode );
		}

		public void ToString( StringBuilder Output, int Indent )
		{
			if( Attribute == null )
			{
				foreach( VyattaConfigNode Node in Children )
				{
					Node.ToString( Output, Indent + 1 );
					Output.Append( "\n" );
				}
			}
			else
			{
				Output.Append( VyattaConfigUtil.IndentString( Indent ) );
				Attribute.ToString( Output, 0 );
				Output.Append( " {\n" );
				foreach( VyattaConfigNode Node in Children )
				{
					Node.ToString( Output, Indent + 1 );
					Output.Append( "\n" );
				}
				Output.Append( VyattaConfigUtil.IndentString( Indent ) + "}" );
			}
		}
	}
}
