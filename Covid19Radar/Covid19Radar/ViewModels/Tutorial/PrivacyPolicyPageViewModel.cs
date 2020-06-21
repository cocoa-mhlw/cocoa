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
        private readonly UserDataService userDataService;
        private UserDataModel userData;


        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = Resources.AppResources.UrlPrivacyPolicy;

            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }

        /*
        public Command OnClickAgree => new Command(async () =>
        {

            UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextRegistering);
            if (!userDataService.IsExistUserData)
            {
                userData = await userDataService.RegisterUserAsync();
                if (userData == null)
                {
                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.DialogNetworkConnectionError, Resources.AppResources.DialogNetworkConnectionErrorTitle, Resources.AppResources.ButtonOk);
                    return;
                }
            }
            userData.IsOptined = true;
            await userDataService.SetAsync(userData);
            UserDialogs.Instance.HideLoading();
            await NavigationService.NavigateAsync(nameof(DescriptionPage1));
        });
        public Command OnClickNotAgree => new Command(() =>
        {
            // Application close
            Xamarin.Forms.DependencyService.Get<ICloseApplication>().closeApplication();

        });
        */
    }
}
