using System.Collections.Generic;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage3ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public TutorialPage3ViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = AppResources.TitleConsentByUserPage;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }
        public Command OnClickAgree => new Command(async () =>
        {

            UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextRegistering);
            if (!userDataService.IsExistUserData)
            {
                userData = await userDataService.RegisterUserAsync();
                if (userData == null)
                {
                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.DialogNetworkConnectionError, "Connection error", Resources.AppResources.ButtonOk);
                    return;
                }
            }
            userData.IsOptined = true;
            await userDataService.SetAsync(userData);
            UserDialogs.Instance.HideLoading();
            await NavigationService.NavigateAsync(nameof(DescriptionPage1));
        });
    }
}