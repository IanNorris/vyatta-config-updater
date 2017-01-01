using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;
using vyatta_config_updater.VyattaConfig;

namespace vyatta_config_updater
{
	public class RouterWriteNewConfig : BusyWorkInterface
	{
		private RouterData Data;

		public RouterWriteNewConfig( RouterData Data )
		{
			this.Data = Data;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			SetStatus( "Connecting to SSH...", 0 );

			using( VyattaShell Shell = new VyattaShell( Data.Address, Data.Username, Data.Password ) )
			{
				SetStatus( "Creating backup...", 5 );
				Shell.RunCommand( "mkdir /config/vcu" );
				Shell.RunCommand( "cp /config/config.boot /config/vcu/previous.boot");
				Shell.RunCommand( "cp /config/current.vcu /config/vcu/previous.vcu");
				Shell.RunCommand( string.Format( "cp /config/config.boot /config/vcu/history_{0}.boot", DateTime.Now.ToString(VyattaConfigUtil.SortableDateFormat) ) );
				Shell.RunCommand( string.Format( "cp /config/current.vcu /config/vcu/history_{0}.vcu", DateTime.Now.ToString(VyattaConfigUtil.SortableDateFormat) ) );

				SetStatus( "Entering configure mode...", 10 );
				Shell.RunCommand( "configure" );

				if( !ShouldCancel() )
				{
					using( ScpClient ScpClient = new ScpClient( Data.Address, Data.Username, Data.Password ) )
					{
						string TempConfigPath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
						Data.NewConfigLines = VyattaConfigUtil.WriteToStringLines( Data.ConfigRoot );
						VyattaConfigUtil.WriteToFile( Data.ConfigRoot, TempConfigPath );

						string TempTemplatePath = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid().ToString());
						VyattaConfigUtil.WriteToFile( Data.TemplateRoot, TempTemplatePath );

						ScpClient.Connect();

						SetStatus( "Uploading new config...", 15 );

						if( !ShouldCancel() )
						{
							using( Stream configFile = new FileStream( TempConfigPath, FileMode.Open ) )
							{
								ScpClient.Upload( configFile, "/config/config.boot" );
							}
						}

						//Probably shouldn't cancel after this point it might leave things in a weird state...
						
						using( Stream templateFile = new FileStream( TempTemplatePath, FileMode.Open ) )
						{
							ScpClient.Upload( templateFile, "/config/current.vcu" );
						}

						SetStatus( "Disconnecting SCP...", 20 );

						ScpClient.Disconnect();
					}
				}

				SetStatus( "Loading new config...", 25 );

				Shell.RunCommand( "load" );

				SetStatus( "Comparing config...", 35 );
				Shell.RunCommand( "compare" );

				SetStatus( "Committing new config (this will take a while)...", 45 );
				Shell.RunCommand( "commit" );

				SetStatus( "Committing new config (this will take a while)...", 95 );
				Shell.RunCommand( "exit" );

				SetStatus( "Disconnecting from SSH...", 98 );
			}
			
			SetStatus( "Completed.", 100 );

			return true;
		}
	}
}
