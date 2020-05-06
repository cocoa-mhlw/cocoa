using System.Collections.Generic;
using System.ComponentModel;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Prism.Navigation.TabbedPages;
using Prism.Navigation.Xaml;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public MainPageViewModel() : base()
        {
            Title = "main page";
            NavigationService.SelectTabAsync("home");
        }
    }
}
