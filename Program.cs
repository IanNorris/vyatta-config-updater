﻿using System;
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
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			//var BootForm = new Login();
			var BootForm = new Main( "", "", "", @"X:\ExampleVyattaConfig.txt");
			BootForm.Show();

			Application.Run();
		}
	}
}
