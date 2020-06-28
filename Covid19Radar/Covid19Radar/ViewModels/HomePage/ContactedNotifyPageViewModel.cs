using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;
using Covid19Radar.Resources;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;
using ImTools;
using Acr.UserDialogs;

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        private string _exposureCount;
        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }

        private readonly ExposureNotificationService exposureNotificationService;


        public ContactedNotifyPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            this.exposureNotificationService = exposureNotificationService;
            ExposureCount = exposureNotificationService.GetExposureCount().ToString();
        }
        public Command OnClickByForm => new Command(async () =>
        {
            var uri = AppResources.UrlContactedForm;
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });
        public Command OnClickByPhone => new Command(async () =>
        {
            var uri = AppResources.UrlContactedPhone;
            using (var client = new HttpClient())
            {
                UserDialogs.Instance.ShowLoading();
                try
                {
                    var json = await client.GetStringAsync(uri);
                    var phoneNumber = JObject.Parse(json).Value<string>("phone");
                    Console.WriteLine($"Contacted phone call number = {phoneNumber}");
                    PhoneDialer.Open(phoneNumber);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
        });

    }
}
