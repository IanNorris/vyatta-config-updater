using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig
{
	public class VyattaConfigValue : VyattaConfigNode
	{
		string Value;
		bool Quoted;

		public VyattaConfigValue( string Value, bool? Quoted )
		{
			this.Value = Value;
			this.Quoted = Quoted == null ? Value.Contains( ' ' ) : Quoted.Value;
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

		public override string ToString()
		{
			StringBuilder SB = new StringBuilder();
			ToString( SB, 0 );
			return SB.ToString();
		}

		public string GetAttributeString()
		{
			return null;
		}

		public VyattaConfigNode GetChild( string Child )
		{
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

		public bool IsQuoted()
		{
			return Quoted;
		}

		public string GetValue()
		{
			return Value;
		}

		public string GetName()
		{
			return null;
		}
	}
}
