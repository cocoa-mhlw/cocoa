using System.Collections.Generic;
using Covid19Radar.Model;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class ContributorsPageViewModel : ViewModelBase
    {
//        public List<ContributorModel> Contributors { get; set; }
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }
        public ContributorsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resx.AppResources.TitleContributorsPage;
            Url = Resx.AppResources.UrlContributor;
        }
    }
}
