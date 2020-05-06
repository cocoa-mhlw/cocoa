using System.Collections.Generic;
using Covid19Radar.Model;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class ContributorsPageViewModel : ViewModelBase
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }
        public ContributorsPageViewModel() : base()
        {
            Title = Resources.AppResources.TitleContributorsPage;
            Url = Resources.AppResources.UrlContributor;
        }
    }
}
