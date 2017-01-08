using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vyatta_config_updater
{
	public partial class AddStaticRouteWizard : Form
	{
		private StaticRoutingData Result = new StaticRoutingData();

		public StaticRoutingData GetResult()
		{
			return Result;
		}

		public AddStaticRouteWizard()
		{
			InitializeComponent();
		}
	}
}
