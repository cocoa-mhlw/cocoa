using System.Collections.Generic;
using Covid19Radar.Model;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class ContributorsPageViewModel : ViewModelBase
    {
        public List<ContributorModel> Contributors { get; set; }

        public ContributorsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resx.AppResources.TitleContributorsPage;
            SetData();
        }

        private void SetData()
        {
            Contributors = new List<ContributorModel>
            {
                new ContributorModel { Fullname = "Kazumi Hirose", Description = "Originator" },
                new ContributorModel { Fullname = "Kristina Yasuda", Description = "Public Relations and Activity" },
                new ContributorModel { Fullname = "Tsuyoshi Ushio", Description = "Server side / Azure Functions" },
                new ContributorModel { Fullname = "Yasuaki Matsuda", Description = "Server side / Azure Functions" },
                new ContributorModel { Fullname = "Noriko Matsumoto", Description = "Design UI/UX / Main Site Designer" },
                new ContributorModel { Fullname = "Kazuki Ota", Description = "Create Build pipeline on AppCenter" },
                new ContributorModel { Fullname = "Takeshi Sakurai ", Description = "Client iOS" },
                new ContributorModel { Fullname = "Fumiya Kume", Description = "Client Xamarin" },
                new ContributorModel { Fullname = "Taiki Yoshida", Description = "Power BI / Power Apps" },
                new ContributorModel { Fullname = "Takayuki Hirose", Description = "Device Mac Lending" },
                new ContributorModel { Fullname = "Tomoaki Ueno", Description = "Medical Information and FieAdvisory" },
                new ContributorModel { Fullname = "Takashi Takebayashi", Description = "Translate" }
            };
        }
    }
}
