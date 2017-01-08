using System.Collections.Generic;

namespace vyatta_config_updater
{
	public class ChainWorker : BusyWorkInterface
	{
		private List<BusyWorkInterface> Jobs = new List<BusyWorkInterface>();

		public void AddWork( BusyWorkInterface Work )
		{
			Jobs.Add( Work );
		}

		public bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel )
		{
			int JobIndex = 0;
			foreach( var Job in Jobs )
			{
				Util.UpdateStatusDelegate OverrideJobStatusDelegate = ( string NewStatus, int NewProgress ) =>
				{
					float IndividualJobProgress = ((float)NewProgress / 100.0f) / (float)Jobs.Count;
					float OverallJobProgress = (float)JobIndex / (float)Jobs.Count;
					
					IndividualJobProgress /= (float)Jobs.Count;

					float Total = (OverallJobProgress + IndividualJobProgress) * 100.0f;
					int TotalAsInt = (int)Total;

					SetStatus( NewStatus, TotalAsInt );
				};

				if( !Job.DoWork( OverrideJobStatusDelegate, ShouldCancel ) )
				{
					return false;
				}

				if( ShouldCancel() ) { return false; }

				JobIndex++;
			}

			return true;
		}
	}
}
