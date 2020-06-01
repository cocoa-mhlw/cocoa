using System.ComponentModel;
using ExposureNotification.App.ViewModels;
using Xamarin.Forms;

namespace ExposureNotification.App.Views
{
	public partial class NotifyOthersPage : ContentPage
	{
		public NotifyOthersPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			if (BindingContext is ViewModelBase vm)
				vm.NotifyAllProperties();

			base.OnAppearing();
		}
	}
}
