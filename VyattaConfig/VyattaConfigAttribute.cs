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

		public void Add( string Value, bool? Quoted = null )
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

		public override string ToString()
		{
			StringBuilder SB = new StringBuilder();
			ToString( SB, 0 );
			return SB.ToString();
		}

		public string GetAttributeString()
		{
			return ToString();
		}

		public VyattaConfigNode GetChild( string Child )
		{
			if( Child == Name )
			{
				return this;
			}

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
			return Name;
		}

		public VyattaConfigValue GetValue( int Index = 0 )
		{
			if( Index < Children.Count )
			{
				return Children[ Index ];
			}
			else
			{
				return null;
			}
		}
	}
}
