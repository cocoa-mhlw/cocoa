using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class UpdateInfoPageViewModel : ViewModelBase
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }
        public UpdateInfoPageViewModel() : base()
        {
            Title = Resources.AppResources.TitleUpdateInformation;
            Url = Resources.AppResources.UrlUpdate;
        }

        public Command OnClickPrev => (new Command(() =>
        {
            NavigationService.NavigateAsync("HomePage");
        }));


    }
}
