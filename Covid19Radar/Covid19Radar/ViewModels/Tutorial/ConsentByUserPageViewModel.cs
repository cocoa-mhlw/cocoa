using System.Collections.Generic;
using System.Windows.Input;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ConsentByUserPageViewModel : ViewModelBase
    {
        private UserDataService _userDataService;

        public ICommand OnClickNext { get; }

        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private readonly IPageDialogService _pageDialogService;

        public ConsentByUserPageViewModel(IPageDialogService pageDialogService)
            : base()
        {
            Title = AppResources.TitleConsentByUserPage;
            Url = Resources.AppResources.UrlPrivacyPolicy;

            _pageDialogService = pageDialogService;
            _userDataService = App.Current.Container.Resolve<UserDataService>();
            OnClickNext = new Command(async () =>
             {
                 // Regist user
                 if (!_userDataService.IsExistUserData)
                 {
                     IsBusy = true;
                     UserDataModel userData = await _userDataService.RegistUserAsync();
                     IsBusy = false;

                     if (userData == null)
                     {
                         await _pageDialogService.DisplayAlertAsync("", Resources.AppResources.DialogNetworkConnectionError, Resources.AppResources.DialogButtonOk);
                         return;
                     }
                 }

                 await NavigationService.NavigateAsync("InitSettingPage");
             });

        }
    }
}
