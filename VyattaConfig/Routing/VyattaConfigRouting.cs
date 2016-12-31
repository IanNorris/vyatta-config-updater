using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig.Routing
{
	public static class VyattaConfigRouting
	{
		public static void AddStaticRoutesForOrganization( VyattaConfigObject ConfigRoot, string OrganizationSubstring, RouterData Data, string Target, string Name )
		{
			var Netmasks = Data.ASNData.GetNetmasksFromOrganization( OrganizationSubstring );
			
			string Description = string.Format( "VCU-Auto: {0}", Name );

			foreach( var Netmask in Netmasks )
			{
				VyattaConfigRouting.AddStaticRoute( ConfigRoot, Data, Netmask, Target, Description );
			}
		}

		public static void AddStaticRoutesForASN( VyattaConfigObject ConfigRoot, int ASN, RouterData Data, string Target, string Name )
		{
			var Netmasks = Data.ASNData.GetNetmasksFromASN( ASN );
			
			string Description = string.Format( "VCU-Auto: {0}", Name );

			foreach( var Netmask in Netmasks )
			{
				VyattaConfigRouting.AddStaticRoute( ConfigRoot, Data, Netmask, Target, Description );
			}
		}

		public static VyattaConfigObject AddStaticRoute( VyattaConfigObject ConfigRoot, RouterData Data, string Network, string Target, string Description )
		{
			if( Network.Contains( "-" ) )
			{
				Network = VyattaConfigUtil.IPRangeToCIDR( Network );
			}

			foreach( var Int in Data.Interfaces )
			{
				if(	   ( Int.Description == Target || Int.Interface == Target )
					&& Int.Gateway != null )
				{
					Target = Int.Gateway;
					break;
				}
			}


			VyattaConfigObject Route = ConfigRoot.AddObject( string.Format( "protocols:static:route {0}:next-hop {1}", Network, Target ) );
			Route.AddAttribute( "description" ).Add( Description );
			return Route;
		}

		public static List<VyattaConfigObject> GetStaticRoutes( VyattaConfigObject ConfigRoot )
		{
			var StaticRoutesNodes = ConfigRoot.GetChild("protocols:static");
			var StaticRoutes = StaticRoutesNodes as VyattaConfigObject;

			var Results = new List<VyattaConfigObject>();
			if( StaticRoutes != null )
			{
				var Children = StaticRoutes.GetChildren();

				foreach( var Child in Children )
				{
					var CastChild = Child as VyattaConfigObject;
					if( CastChild != null )
					{
						Results.Add( CastChild );
					}
				}
			}

			return Results;
		}

		public static void DeleteStaticRoute( VyattaConfigObject ConfigRoot, RouterData Data, string Network )
		{
			if( Network.Contains( "-" ) )
			{
				Network = VyattaConfigUtil.IPRangeToCIDR( Network );
			}

			ConfigRoot.Delete( string.Format( "protocols:static:route {0}", Network ) );
		}

		public static void DeleteGeneratedStaticRoutes( VyattaConfigObject ConfigRoot, string Name = null )
		{
			var StaticRoutesNodes = ConfigRoot.GetChild("protocols:static");
			var StaticRoutes = StaticRoutesNodes as VyattaConfigObject;

			var Results = new List<VyattaConfigObject>();
			if( StaticRoutes != null )
			{
				var Children = StaticRoutes.GetChildren();
				
				List<VyattaConfigNode> ToRemove = new List<VyattaConfigNode>();

				foreach( var Child in Children )
				{
					var CastChild = Child as VyattaConfigObject;
					if( CastChild != null )
					{
						bool IsAuto = false;

						var SubChildren = CastChild.GetChildren();

						foreach( var SubChild in SubChildren )
						{
							var CastSubChild = SubChild as VyattaConfigObject;
							if( CastSubChild != null )
							{
								
								var SubSubChildren = CastSubChild.GetChildren();

								foreach( var SubSubChild in SubSubChildren )
								{
									var CastSubSubAttribute = SubSubChild as VyattaConfigAttribute;
									if( CastSubSubAttribute != null )
									{
										if( CastSubSubAttribute.GetName() == "description" )
										{
											string Value = CastSubSubAttribute.GetValue(0).GetValue();
											if( Value != null )
											{
												if( Name != null && Value.Contains( "VCU-Auto: " + Name ) )
												{
													IsAuto = true;
													break;
												}
												else if( Value.Contains( "VCU-Auto:" ) )
												{
													IsAuto = true;
													break;
												}
											}
										}
									}
								}

								if( IsAuto )
								{
									break;
								}
							}
						}

						if( IsAuto )
						{
							ToRemove.Add( Child );
						}
					}
				}

				foreach( var Remove in ToRemove )
				{
					Children.Remove( Remove );
				}
			}
		}
	}
}
