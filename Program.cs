using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vyatta_config_updater
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( string[] Arguments )
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			bool AutoLogin = false;
			foreach( var Arg in Arguments )
			{
				if( String.Compare( Arg, "-AutoLogin", true ) == 0 )
				{
					AutoLogin = true;
				}
			}

			var BootForm = new Login( AutoLogin );
			//var BootForm = new Main( "", "", "", @"X:\ExampleVyattaConfig.txt");
			BootForm.Show();

			Application.Run();
		}
	}
}
