using System.Collections.Generic;
using Covid19Radar.Model;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DescriptionPageViewModel : ViewModelBase
    {
        public List<StepModel> Steps { get; set; }

        public DescriptionPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resx.AppResources.TitleHowItWorks;
            SetData();
        }

        public Command OnClickNext => new Command(() => NavigationService.NavigateAsync("ConsentByUserPage"));

        private void SetData()
        {
            Steps = new List<StepModel>
            {
                new StepModel
                {
                    Title = Resx.AppResources.DescriptionPageTitleTextStep1,
                    Image = "descriptionStep1.png",
                    Description = Resx.AppResources.DescriptionPageTextStep1Description,
                    StepNumber = 1
                },
                new StepModel
                {
                    Title = Resx.AppResources.DescriptionPageTitleTextStep2,
                    Image = "descriptionStep2.png",
                    Description = Resx.AppResources.DescriptionPageTextStep2Description,
                    StepNumber = 2
                },
                new StepModel
                {
                    Title = Resx.AppResources.DescriptionPageTitleTextStep3,
                    Image = "descriptionStep3.png",
                    Description = Resx.AppResources.DescriptionPageTextStep3Description,
                    StepNumber = 3
                },
                new StepModel
                {
                    Title = Resx.AppResources.DescriptionPageTitleTextStep4,
                    Image = "descriptionStep4.png",
                    Description = Resx.AppResources.DescriptionPageTextStep4Description
                }
            };

        }
    }
}
