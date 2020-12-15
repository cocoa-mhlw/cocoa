using Covid19Radar.Services;
using Prism.Mvvm;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {

        // Navigation
        protected INavigationService NavigationService { get; private set; }
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

        public ViewModelBase(INavigationService navigationService, ExposureNotificationService exposureNotificationService)
        {
            NavigationService = navigationService;
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
