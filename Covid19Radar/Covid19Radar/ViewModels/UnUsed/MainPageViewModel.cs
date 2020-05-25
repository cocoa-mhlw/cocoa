using System.Collections.Generic;
using System.ComponentModel;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public List<HomeMenuModel> MainMenus { get; private set; }

        public MainPageViewModel(INavigationService navigationService, IStatusBarPlatformSpecific statusBarPlatformSpecific)
            : base(navigationService, statusBarPlatformSpecific)
        {
            Title = "main page";
        }
    }
}
