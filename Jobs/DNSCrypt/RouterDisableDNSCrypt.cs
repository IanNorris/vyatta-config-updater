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
	public class RouterDisableDNSCrypt : BusyWorkInterface
	{
		private RouterData Data;
		private Form Parent;

		public RouterDisableDNSCrypt( RouterData Data, Form Parent )
		{
			this.Data = Data;
			this.Parent = Parent;
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			SetStatus( "Connecting to SSH...", 0 );

			using( VyattaShell Shell = new VyattaShell( Data.Address, Data.Username, Data.Password ) )
			{
				SetStatus( "Killing DNSCrypt if running...", 10 );
				RouterEnableDNSCrypt.KillDNSCrypt( Shell );

				SetStatus( "Killing DNSCrypt if running...", 30 );
				
				Shell.RunCommand( "configure" );
				Shell.RunCommand( "delete service dns forwarding options" );

				SetStatus( "Committing changes...", 60 );
				Shell.RunCommand( "commit" );
				Shell.RunCommand( "save" );
				Shell.RunCommand( "exit" );

				SetStatus( "Restarting dnsmasq...", 80 );

				Shell.RunCommand( "sudo /etc/init.d/dnsmasq restart" );
			}
			
			SetStatus( "Completed.", 100 );

			return true;
		}
	}
}
