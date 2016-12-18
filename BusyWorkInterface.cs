namespace vyatta_config_updater
{
	public interface BusyWorkInterface
	{
		bool DoWork( Util.UpdateStatusDelegate SetStatus, Util.ShouldCancelDelegate ShouldCancel );
	}
}
