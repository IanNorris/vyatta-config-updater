using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using vyatta_config_updater.VyattaConfig;
using vyatta_config_updater.VyattaConfig.Routing;

namespace vyatta_config_updater
{
	public class RouterGenerateNewConfig : BusyWorkInterface
	{
		private RouterData Data;

		public RouterGenerateNewConfig( RouterData Data )
		{
			this.Data = Data;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			if( ShouldCancel() ) { return false; }

			SetStatus( "Deleting previous auto-generated rules...", 16 );
			VyattaConfigRouting.DeleteGeneratedStaticRoutes( Data.ConfigRoot );

			if( ShouldCancel() ) { return false; }
			
			SetStatus( "Generating Netflix static routing...", 32 );
			//VyattaConfigRouting.AddStaticRoutesForOrganization( Data.ConfigRoot, "Netflix", Data, "Internet", "Netflix" );
			{
				StaticRoutingData NewRoute = new StaticRoutingData();
				NewRoute.Destination = "Netflix";
				NewRoute.Interface = "eth0";
				NewRoute.Name = "Netflix";
				NewRoute.Type = RoutingType.Organisation;
				NewRoute.Action = StaticRouteAction.ToInterface;

				Data.StaticRoutes.Add( NewRoute );
			}

			if( ShouldCancel() ) { return false; }

			SetStatus( "Generating BBC static routing...", 48 );
			//VyattaConfigRouting.AddStaticRoutesForOrganization( Data.ConfigRoot, "BBC", Data, "Internet", "BBC" );
			{
				StaticRoutingData NewRoute = new StaticRoutingData();
				NewRoute.Destination = "BBC";
				NewRoute.Interface = "eth0";
				NewRoute.Name = "BBC";
				NewRoute.Type = RoutingType.Organisation;
				NewRoute.Action = StaticRouteAction.ToInterface;

				Data.StaticRoutes.Add( NewRoute );
			}

			if( ShouldCancel() ) { return false; }

			SetStatus( "Generating Valve static routing...", 56 );
			//VyattaConfigRouting.AddStaticRoutesForOrganization( Data.ConfigRoot, "Valve", Data, "Internet", "Valve" );
			{
				StaticRoutingData NewRoute = new StaticRoutingData();
				NewRoute.Destination = "Valve";
				NewRoute.Interface = "eth0";
				NewRoute.Name = "Valve";
				NewRoute.Type = RoutingType.Organisation;
				NewRoute.Action = StaticRouteAction.ToInterface;

				Data.StaticRoutes.Add( NewRoute );
			}

			if( ShouldCancel() ) { return false; }

			SetStatus( "Generating Nest static routing...", 80 );
			//VyattaConfigRouting.AddStaticRoutesForOrganization( Data.ConfigRoot, "Nest", Data, "Internet", "Nest" );
			{
				StaticRoutingData NewRoute = new StaticRoutingData();
				NewRoute.Destination = "Nest";
				NewRoute.Interface = "eth0";
				NewRoute.Name = "Nest";
				NewRoute.Type = RoutingType.Organisation;
				NewRoute.Action = StaticRouteAction.ToInterface;

				Data.StaticRoutes.Add( NewRoute );
			}
			
			SetStatus( "Saving new config...", 90 );
			Data.NewConfigLines = VyattaConfigUtil.WriteToStringLines( Data.ConfigRoot );

			return true;
		}
	}
}
