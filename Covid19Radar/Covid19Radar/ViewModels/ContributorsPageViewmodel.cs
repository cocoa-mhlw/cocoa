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
            //            SetData();
        }
/*
        private void SetData()
        {
            Contributors = new List<ContributorModel>
            {
                new ContributorModel { Fullname = "Kazumi Hirose", Role = "Originator" },
                new ContributorModel { Fullname = "Kristina Yasuda", Role = "Public Relations and Activity" },
                new ContributorModel { Fullname = "Tsuyoshi Ushio", Role = "Server side / Azure Functions" },
                new ContributorModel { Fullname = "Yasuaki Matsuda", Role = "Server side / Azure Functions" },
                new ContributorModel { Fullname = "Noriko Matsumoto", Role = "Design UI/UX / Main Site Designer" },
                new ContributorModel { Fullname = "Kazuki Ota", Role = "Create Build pipeline on AppCenter" },
                new ContributorModel { Fullname = "Takeshi Sakurai ", Role = "Client iOS" },
                new ContributorModel { Fullname = "Fumiya Kume", Role = "Client Xamarin" },
                new ContributorModel { Fullname = "Taiki Yoshida", Role = "Power BI / Power Apps" },
                new ContributorModel { Fullname = "Takayuki Hirose", Role = "Device Mac Lending" },
                new ContributorModel { Fullname = "Tomoaki Ueno", Role = "Medical Information and FieAdvisory" },
                new ContributorModel { Fullname = "Takashi Takebayashi", Role = "Translate" }
            };
        }
*/
    }
}
