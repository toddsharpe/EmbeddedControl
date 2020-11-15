using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Ground.Framework
{
	public class ViewModelPage : Page
	{
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			this.DataContext = e.Parameter;

			if (e.NavigationMode == NavigationMode.New)
			{
				if (e.Parameter is ILoadable loadable)
					await loadable.LoadAsync();
			}
		}
	}
}
