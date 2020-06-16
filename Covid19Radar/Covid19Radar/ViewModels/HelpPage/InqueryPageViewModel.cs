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

        public InqueryPageViewModel() : base()
        {
        }

        public Command OnClickSite1 => new Command(async () =>
        {
            var uri = "https://corona.go.jp/";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });

        public Command OnClickSite2 => new Command(async () =>
        {
            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/cocoa_00138.html";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });

        public Command OnClickSite3 => new Command(async () =>
        {
            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/kenkou_iryou/covid19_qa_kanrenkigyou_00009.html";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });

        public Command OnClickEmail => new Command(async () =>
        {

            try
            {
                List<string> recipients = new List<string>();
                recipients.Add(AppSettings.Instance.SupportEmail);
                var message = new EmailMessage
                {
                    Subject = AppResources.InqueryMailSubject,
                    Body = AppResources.InqueryMailBody,
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
