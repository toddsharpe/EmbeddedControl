using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Ground.Framework
{
	public class Navigation
	{
		private readonly Frame _frame;
		public Navigation(Frame frame = null)
		{
			_frame = frame ?? Window.Current.Content as Frame;
		}

		public void NavigateTo<T>(object parameter = null) where T : Page
		{
			_frame.Navigate(typeof(T), parameter);
		}

		public bool GoBack()
		{
			if (_frame.CanGoBack)
			{
				_frame.GoBack();
				return true;
			}
			return false;
		}

		public bool CanGoBack => _frame.CanGoBack;

		public void ReplacePage<T>(object parameter, bool replaceAll = false) where T : ViewModelPage
		{
			ViewModelPage currentPage = _frame.Content as ViewModelPage;
			object dataContext = currentPage?.DataContext;

			ReplacePage(typeof(T), parameter, replaceAll);

			IDisposable context = dataContext as IDisposable;
			context?.Dispose();
		}

		public void ReplacePage(Type pageType, object parameter, bool replaceAll = false)
		{
			_frame.Navigate(pageType, parameter);
			if (replaceAll)
				_frame.BackStack.Clear();
			else
				_frame.BackStack.Remove(_frame.BackStack.Last());
		}
	}
}
