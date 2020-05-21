using System.Collections.Generic;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPageViewModel : ViewModelBase
    {
        private UserDataService _userDataService;

        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = AppResources.TitleConsentByUserPage;
            Url = Resources.AppResources.UrlPrivacyPolicy;

            _userDataService = App.Current.Container.Resolve<UserDataService>();
        }

        public Command OnClickAgree => new Command(async () =>
        {
            if (!LocalStateManager.Instance.IsWelcomed)
            {
                LocalStateManager.Instance.IsWelcomed = true;
                LocalStateManager.Save();
            }

            UserDialogs.Instance.ShowLoading("Waiting for register");
            if (!_userDataService.IsExistUserData)
            {
                // TODO Create and Get Secure API access token key per AES256 user from Azure Func Side
                /*
                 UserDataModel userData = await _userDataService.RegisterUserAsync();
                if (userData == null)
                {
                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.DialogNetworkConnectionError, "Connection error", Resources.AppResources.DialogButtonOk);
                    return;
                }
                */
            }
            UserDialogs.Instance.HideLoading();
            await NavigationService.NavigateAsync(nameof(DescriptionPage1));
        });
        public Command OnClickNotAgree => new Command(() =>
        {
            // Application close
            Xamarin.Forms.DependencyService.Get<ICloseApplication>().closeApplication();

        });

    }
}
