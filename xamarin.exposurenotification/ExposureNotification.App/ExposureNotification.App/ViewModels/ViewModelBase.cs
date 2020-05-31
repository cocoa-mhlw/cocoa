using System.Threading.Tasks;
using MvvmHelpers;
using Xamarin.Forms;

namespace ExposureNotification.App.ViewModels
{
	public class ViewModelBase : BaseViewModel
	{
		bool isEnabled;
		bool navigating;

		public void NotifyAllProperties()
			=> OnPropertyChanged(null);

		public bool IsEnabled
		{
			get => isEnabled;
			set => SetProperty(ref isEnabled, value);
		}

		public async Task GoToAsync(string path)
		{
			if (navigating)
				return;
			navigating = true;

			await Shell.Current.GoToAsync(path);

			navigating = false;
		}
	}
}
