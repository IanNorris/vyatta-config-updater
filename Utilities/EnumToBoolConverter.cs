using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace vyatta_config_updater.Utilities
{
	class EnumToBoolConverter : IValueConverter
	{
		public object Convert( object Value, Type TargetType, object Parameter, System.Globalization.CultureInfo Culture )
		{
			return Value.Equals( Parameter );
		}

		public object ConvertBack( object Value, Type TargetType, object Parameter, System.Globalization.CultureInfo Culture )
		{
			if( Value.Equals( true ) )
			{
				return Parameter;
			}
			else
			{
				return Binding.DoNothing;
			}
		}
	}
}
