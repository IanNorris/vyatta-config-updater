using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vyatta_config_updater.VyattaConfig;
using vyatta_config_updater.VyattaConfig.Routing;

namespace vyatta_config_updater
{
	public class RouterGenerateNewConfig : BusyWorkInterface
	{
		private string OldConfig;
		private string NewConfig;
		private ASNData ASNData;
		private List<InterfaceMapping> Interfaces;

		public RouterGenerateNewConfig( string ConfigPath, ASNData ASNData, List<InterfaceMapping> Interfaces )
		{
			this.OldConfig = ConfigPath;
			this.NewConfig = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
			this.ASNData = ASNData;
			this.Interfaces = Interfaces;
		}

		public string GetNewConfigPath()
		{
			return NewConfig;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			SetStatus( "Parsing existing config...", 0 );

			if( ShouldCancel() ) { return false; }

			string Errors = "";
			VyattaConfigObject Root = VyattaConfigUtil.ReadFromFile( OldConfig, ref Errors );

			if( ShouldCancel() ) { return false; }

			SetStatus( "Deleting previous auto-generated rules...", 16 );
			VyattaConfigRouting.DeleteGeneratedStaticRoutes( Root );

			if( ShouldCancel() ) { return false; }
			
			SetStatus( "Generating Netflix static routing...", 32 );
			VyattaConfigRouting.AddStaticRoutesForOrganization( Root, "Netflix", ASNData, Interfaces, "Internet" );

			if( ShouldCancel() ) { return false; }

			SetStatus( "Generating BBC static routing...", 48 );
			VyattaConfigRouting.AddStaticRoutesForOrganization( Root, "BBC", ASNData, Interfaces, "Internet" );

			if( ShouldCancel() ) { return false; }

			SetStatus( "Generating Valve static routing...", 56 );
			VyattaConfigRouting.AddStaticRoutesForOrganization( Root, "Valve", ASNData, Interfaces, "Internet" );

			if( ShouldCancel() ) { return false; }

			SetStatus( "Generating Nest static routing...", 80 );
			VyattaConfigRouting.AddStaticRoutesForOrganization( Root, "Nest", ASNData, Interfaces, "Internet" );
			
			SetStatus( "Saving new config...", 90 );
			VyattaConfigUtil.WriteToFile( Root, NewConfig );

			if( Errors.Length > 0 )
			{
				MessageBox.Show( "Errors when writing config:\n" + Errors, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return false;
			}

			return true;
		}
	}
}
