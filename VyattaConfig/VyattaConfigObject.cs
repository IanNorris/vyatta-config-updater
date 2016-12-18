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

		public List<VyattaConfigNode> GetChildren()
		{
			return Children;
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

		public override string ToString()
		{
			StringBuilder SB = new StringBuilder();
			ToString( SB, 0 );
			return SB.ToString();
		}

		public string GetAttributeString()
		{
			return Attribute?.ToString();
		}

		public VyattaConfigNode GetChild( string Child )
		{
			if( Attribute == null )
			{
				foreach( var ChildNode in Children )
				{
					var Result = ChildNode.GetChild( Child );
					if( Result != null )
					{
						return Result;
					}
				}
			}
			else
			{
				if( Child == Attribute.ToString() )
				{
					return this;
				}
				else
				{
					string[] Split = Child.Split( new char[] { ':' }, 2 );
					if( Split.Length != 2 )
					{
						return null;
					}
				
					if( Split[0] != Attribute.ToString() )
					{
						return null;
					}

					foreach( var ChildNode in Children )
					{
						var Result = ChildNode.GetChild( Split[1] );
						if( Result != null )
						{
							return Result;
						}
					}
				}
			}

			return null;
		}

		public VyattaConfigAttribute AddAttribute( string Parent )
		{
			string[] Split = Parent.Split( new char[] { ':' }, 2 );
			
			foreach( var ChildNode in Children )
			{
				if( ChildNode.GetAttributeString() == Split[0] )
				{
					return ChildNode.AddAttribute( Split[1] );
				}
			}

			if( Split.Length == 2 )
			{
				var NewNode = new VyattaConfigObject( new VyattaConfigAttribute(Split[0]) );

				foreach( var Child in Children )
				{
					//If we've already got that attribute
					if( Child.GetName() == Split[1].Split( new char[] { ' ' }, 2 )[0] )
					{
						Children.Remove( Child );
						break;
					}
				}

				Children.Add( NewNode );
				return NewNode.AddAttribute( Split[1] );
			}
			else
			{
				var NewNode = new VyattaConfigAttribute(Split[0]);

				foreach( var Child in Children )
				{
					//If we've already got that attribute
					if( Child.GetName() == Split[0].Split( new char[] { ' ' }, 2 )[0] )
					{
						Children.Remove( Child );
						break;
					}
				}

				Children.Add( NewNode );
				return NewNode;
			}
		}

		public VyattaConfigObject AddObject( string Parent )
		{
			string[] Split = Parent.Split( new char[] { ':' }, 2 );
			
			foreach( var ChildNode in Children )
			{
				if( ChildNode.GetAttributeString() == Split[0] )
				{
					if( Split.Length == 2 )
					{
						return ChildNode.AddObject( Split[1] );
					}
					else
					{
						return ChildNode as VyattaConfigObject;
					}
				}
			}

			var NewNode = new VyattaConfigObject( new VyattaConfigAttribute(Split[0]) );
			Children.Add( NewNode );

			if( Split.Length == 2 )
			{
				return NewNode.AddObject( Split[1] );
			}
			else
			{
				return NewNode;
			}
		}

		public void Delete( string Path )
		{
			string[] Split = Path.Split( new char[] { ':' }, 2 );
						
			foreach( var ChildNode in Children )
			{
				if( ChildNode.GetAttributeString() == Split[0] )
				{
					if( Split.Length == 1 )
					{
						Children.Remove(ChildNode);
						return;
					}

					ChildNode.Delete( Split[1] );
					return;
				}
			}
		}

		public string GetName()
		{
			return Attribute?.GetName();
		}
	}
}
