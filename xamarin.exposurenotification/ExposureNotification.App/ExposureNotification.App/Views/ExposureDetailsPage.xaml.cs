using ExposureNotification.App.ViewModels;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace ExposureNotification.App.Views
{
	public partial class ExposureDetailsPage : ContentPage
	{
		public ExposureDetailsPage()
		{
			InitializeComponent();

			BindingContext = new ExposureDetailsViewModel();
		}
	}
}
