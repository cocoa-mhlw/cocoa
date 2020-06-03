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
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {

        // Navigation
        protected INavigationService NavigationService { get; private set; }
        protected UserDataService UserDataService { get; private set; }
        protected ExposureNotificationService ExposureNotificationService { get; private set; }

        // PageTite
        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase()
        {
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public ViewModelBase(INavigationService navigationService, UserDataService userDataService)
        {
            NavigationService = navigationService;
            UserDataService = userDataService;
        }

        public ViewModelBase(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService)
        {
            NavigationService = navigationService;
            UserDataService = userDataService;
            ExposureNotificationService = exposureNotificationService;
        }


        public virtual void Initialize(INavigationParameters parameters)
        {

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
