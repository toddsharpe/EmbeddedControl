using Ground.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Ground.Models
{
	class DeviceValue : BaseViewModel
	{
		public Int64 Hash { get; set; }
		public string Device { get; set; }
		public TypeCode TypeCode { get; set; }

		private object _value;
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (_value == value)
					return;

				_value = value;
				_ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				  {
					  OnPropertyChanged();
					  OnPropertyChanged(nameof(Display));
				  });
			}
		}

		public string Display => Device + " = " + Value;
	}
}
