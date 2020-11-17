using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Ground.Framework
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			bool inverse = false;
			if (parameter != null)
				inverse = bool.Parse(parameter.ToString());

			//Theres probably a way smarter way to do this
			if (!inverse)
				return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
			else
				return (!(bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
			//return value is Visibility && (Visibility)value == Visibility.Visible; - doesnt use parameter
		}
	}
}
