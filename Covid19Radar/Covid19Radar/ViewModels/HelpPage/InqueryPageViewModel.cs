using System;
using System.Collections.Generic;
using Covid19Radar.Resources;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;

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
            // Device Model (ex.SMG-950U, iPhone10,6)
            var device = DeviceInfo.Model;

            // Manufacture (ex.Samsung)
            var manufacturer = DeviceInfo.Manufacturer;

            // OS ver (ex. 7.0)
            var version = DeviceInfo.VersionString;

            // Platform (ex. Android)
            var platform = DeviceInfo.Platform;

            var device_info = "DEVICE_INFO : " + AppSettings.Instance.AppVersion + "," + device + "(" + manufacturer + ")," + platform + "," + version;

            try
            {
                List<string> recipients = new List<string>();
                recipients.Add(AppSettings.Instance.SupportEmail);
                var message = new EmailMessage
                {
                    Subject = AppResources.InqueryMailSubject,
                    Body = device_info + "\r\n" + AppResources.InqueryMailBody.Replace("\\r\\n", "\r\n"),
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
