using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InqueryPageViewModel : ViewModelBase
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private List<string> _inqueryItems;

        public List<string> InqueryItems
        {
            get { return _inqueryItems; }
            set { SetProperty(ref _inqueryItems, value); }
        }


        public InqueryPageViewModel() : base()
        {
        }

        public Command OnClickSite => new Command(async () =>
        {
            var uri = "https://corona.go.jp";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });
        public Command OnClickEmail => new Command(async () =>
        {

            try
            {
                List<string> recipients = new List<string>();
                recipients.Add("appsupport@cov19.mhlw.go.jp");
                var message = new EmailMessage
                {
                    Subject = "お問い合わせ",
                    Body = "濃厚接触可能性についてのお問い合わせ",
                    To = recipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        });


    }
}
