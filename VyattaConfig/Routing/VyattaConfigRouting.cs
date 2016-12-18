using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig.Routing
{
	public static class VyattaConfigRouting
	{
		public static void AddStaticRoutesForOrganization( VyattaConfigObject ConfigRoot, string OrganizationSubstring, ASNData ASNData, string TargetIP )
		{
			var Netmasks = ASNData.GetNetmasksFromOrganization( OrganizationSubstring );
			
			string Description = string.Format( "VCU-Auto: {0}", OrganizationSubstring );

			foreach( var Netmask in Netmasks )
			{
				VyattaConfigRouting.AddStaticRoute( ConfigRoot, Netmask, TargetIP, Description );
			}
		}

		public static VyattaConfigObject AddStaticRoute( VyattaConfigObject ConfigRoot, string Network, string TargetIP, string Description )
		{
			if( Network.Contains( "-" ) )
			{
				Network = VyattaConfigUtil.IPRangeToCIDR( Network );
			}

			VyattaConfigObject Route = ConfigRoot.AddObject( string.Format( "protocols:static:route {0}:next-hop {1}", Network, TargetIP ) );
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

		public static void DeleteStaticRoute( VyattaConfigObject ConfigRoot, string Network )
		{
			if( Network.Contains( "-" ) )
			{
				Network = VyattaConfigUtil.IPRangeToCIDR( Network );
			}

			ConfigRoot.Delete( string.Format( "protocols:static:route {0}", Network ) );
		}

		public static void DeleteGeneratedStaticRoutes( VyattaConfigObject ConfigRoot )
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
												if( Value.Contains( "VCU-Auto:" ) )
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
