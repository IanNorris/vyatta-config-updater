﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vyatta_config_updater
{
	public interface BusyWorkInterface
	{
		bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel );
	}
}