using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater.VyattaConfig.Routing
{
	public static class VyattaConfigRouting
	{
		public static void AddStaticRoutesForASN( VyattaConfigObject ConfigRoot, uint ASN )
		{
			//TODO
		}

		public static void AddStaticRoutesForOrganization( VyattaConfigObject ConfigRoot, string OrganizationSubstring )
		{
			//TODO
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
	}
}
