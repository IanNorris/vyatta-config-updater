using System;
using System.Windows.Data;

namespace vyatta_config_updater.Utilities
{
	public class InvertBool : IValueConverter
	{
		public object Convert( object Value, Type TargetType, object Parameter, System.Globalization.CultureInfo Culture )
		{
			if( TargetType != typeof(bool) )
			{
				throw new NotSupportedException();
			}

			return !(bool)Value;
		}

		public object ConvertBack( object Value, Type TargetType, object Parameter, System.Globalization.CultureInfo Culture )
		{
			throw new NotSupportedException();
		}
	}
}
