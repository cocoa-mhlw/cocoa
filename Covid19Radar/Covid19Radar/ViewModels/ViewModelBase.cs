using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {

        public Command OnClickHelp => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(StartTutorialPage), useModalNavigation: true);
        });

        private readonly UserDataService userDataService;
        private UserDataModel userData;

        // Navigation
        protected INavigationService NavigationService { get; private set; }

        // PageTite
        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, UserDataService userDataService)
        {
            NavigationService = navigationService;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            this.userDataService.UserDataChanged += _userDataChanged;

        }
        private void _userDataChanged(object sender, UserDataModel e)
        {
            userData = this.userDataService.Get();
        }


        public virtual async void Initialize(INavigationParameters parameters)
        {
            if (userData.LastIsEnabled && userData.IsWelcomed)
            {
                if (!await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.StartAsync();
                }
            }
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
