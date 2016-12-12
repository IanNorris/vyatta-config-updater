using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig
{
	public class VyattaConfigParser
	{
		public string Input;

		public VyattaConfigParser( string Input )
		{
			this.Input = Input;
		}

		public void ParseError( ref string Errors, string CurrentPosition, string Error )
		{
			string Temp = Input.Substring( 0, Input.Length - CurrentPosition.Length );
			int LineNum = 0;
			int Offset = 0;
			while( (Offset = Temp.IndexOf( "\n")) != -1 )
			{
				LineNum++;
				Temp = Temp.Substring( Offset+1 );
			}
			int CharPos = Temp.Length;

			Errors += string.Format( "({0}:{1}): {2}\n", LineNum+1, CharPos+1, Error );
		}

		public string Parse( ref string Errors, VyattaConfigObject Node, string Input = null )
		{
			if( Node == null )
			{
				ParseError( ref Errors, "", "Cannot pass an empty root node." );
				return null;
			}

			if( Input == null )
			{
				Input = this.Input;
			}

			while( true )
			{
				Input = Input.TrimStart( ' ' );

				if( Input.Length == 0 )
				{
					break;
				}

				int NewLineOffset = 0;
				int NewLineCount = 0;
				while( NewLineOffset < Input.Length && Input[NewLineOffset] == '\n' )
				{
					NewLineCount++;
					NewLineOffset++;
				}
				if( NewLineCount > 0 )
				{
					Node.AddChild( new VyattaConfigBlankLines( NewLineCount ) );
					Input = Input.Substring(NewLineCount);
				}

				if( Input.Length > 2 && Input[0] == '/' && Input[1] == '*' )
				{
					int End = Input.IndexOf( " */" );
					if( End == -1 )
					{
						ParseError( ref Errors, Input, "End of comment not found" );
					}
					Node.AddChild( new VyattaConfigComment( Input.Substring( 3, End - 3 ) ) );
					Input = Input.Substring( End + 3 );

					if( Input.Length > 0 )
					{
						if( Input[0] == '\n' )
						{
							Input = Input.Substring(1);
						}
					}

					continue;
				}

				if( Input.Length == 0 )
				{
					break;
				}

				if( Input[0] == '}' )
				{
					Input = Input.Substring(1);
					if( Input.Length > 0 && Input[0] == '\n' )
					{
						Input = Input.Substring(1);
					}
					return Input;
				}

				//First element is always an attribute
				string[] InputSplit = Input.Split( new char[] { ' ' }, 2 );

				bool FoundNewLine = false;
				if( InputSplit[0].IndexOf( '\n' ) != -1 )
				{
					InputSplit = Input.Split( new char[] { ' ', '\n' }, 2 );
					FoundNewLine = true;
				}

				var NewAttrib = new VyattaConfigAttribute( InputSplit[0] );

				if( InputSplit.Length != 2 || InputSplit[1].Length == 0 )
				{
					ParseError( ref Errors, Input, "Unexpected end of file." );
					return Input;
				}

				Input = InputSplit[1];
				Input = Input.TrimStart( new char[] { ' ' } );

				if( Input.Length == 0 )
				{
					break;
				}
				
				while( true )
				{
					if( Input[0] == '{' )
					{
						Input = Input.Substring(1);
						Input = Input.TrimStart( new char[] { ' ', '\n' } );

						var NewChild = new VyattaConfigObject( NewAttrib );

						Node.AddChild( NewChild );
						Input = Parse( ref Errors, NewChild, Input );
						Input = Input.TrimStart( new char[] { ' ' } );
						if( Input.Length > 0 && Input[0] == '\n' )
						{
							Input = Input.Substring(1);
						}
						
						break;
					}
					else if( Input[0] == '}' )
					{
						Input = Input.Substring(1);
						if( Input.Length > 0 && Input[0] == '\n' )
						{
							Input = Input.Substring(1);
						}
						Node.AddChild( NewAttrib );
						return Input;
					}
					else if( Input[0] == '\"' )
					{
						int Found = Input.IndexOf( "\"", 1 );
						if( Found == -1 )
						{
							ParseError( ref Errors, Input, "Unexpected end of quoted string." );
							return Input;
						}

						NewAttrib.Add( Input.Substring( 1, Found - 1 ), true );
						Input = Input.Substring( Found + 1 );
					}
					else if( Input[0] == '\n' )
					{
						Input = Input.Substring( 1 );
						Input = Input.TrimStart( new char[] { ' ', '\n' } );
						Node.AddChild( NewAttrib );
						break;
					}
					else
					{
						InputSplit = Input.Split( new char[] { ' ' }, 2 );
						if( InputSplit[0].IndexOf( '\n' ) != -1 )
						{
							InputSplit = Input.Split( new char[] { ' ', '\n' }, 2 );
							FoundNewLine = true;
						}
						if( InputSplit.Length == 2 )
						{
							bool Term = false;
							if( InputSplit[0].Last() == '\n' )
							{
								InputSplit[0] = InputSplit[0].TrimEnd( new char[] { '\n' } );
								Term = true;
							}

							NewAttrib.Add( InputSplit[0], false );
							Input = InputSplit[1];

							if( Term )
							{
								Node.AddChild( NewAttrib );
								break;
							}
						}
						else
						{
							break;
						}
					}

					if( FoundNewLine )
					{
						Node.AddChild( NewAttrib );
						break;
					}
				}
			}

			return Input;
		}
	}
}
