using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using Covid19Radar.Renderers;

namespace Covid19Radar.Views
{
    public class NavigatePopoverWebView : WebView
    {
        private string firstNavigateUrl = null;
        public NavigatePopoverWebView() : base()
        {
            Navigating += async (_, e) =>
            {
                if (firstNavigateUrl != null && firstNavigateUrl != e.Url)
                {
                    e.Cancel = true;
                    await Browser.OpenAsync(e.Url);
                }
            };
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(Source) && firstNavigateUrl == null && Source is UrlWebViewSource urlSource)
            {
                firstNavigateUrl = urlSource.Url;
            }
            base.OnPropertyChanged(propertyName);
        }
    }
}
