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
            Title = Resources.AppResources.TitleHowItWorks;
            SetData();
        }

        public Command OnClickNext => new Command(async () =>
        {
            await NavigationService.NavigateAsync("ConsentByUserPage");
        });


        private void SetData()
        {
            Steps = new List<StepModel>
            {
                new StepModel
                {
                    Title = Resources.AppResources.DescriptionPageTitleTextStep1,
                    Image = "descriptionStep11.png",
                    Image2 = "descriptionStep12.png",
                    Description = Resources.AppResources.DescriptionPageTextStep1Description,
                    Description2 = Resources.AppResources.DescriptionPageTextStep1Description2,
                    StepNumber = 1
                },
                new StepModel
                {
                    Title = Resources.AppResources.DescriptionPageTitleTextStep2,
                    Image = "descriptionStep21.png",
                    Image2 = "descriptionStep22.png",
                    Description = Resources.AppResources.DescriptionPageTextStep2Description,
                    Description2 = Resources.AppResources.DescriptionPageTextStep2Description2,
                    StepNumber = 2
                },
                new StepModel
                {
                    Title = Resources.AppResources.DescriptionPageTitleTextStep3,
                    Image = "descriptionStep3.png",
                    Description = Resources.AppResources.DescriptionPageTextStep3Description,
                    StepNumber = 3
                },
                new StepModel
                {
                    Title = Resources.AppResources.DescriptionPageTitleTextStep4,
                    Image = "descriptionStep4.png",
                    Description = Resources.AppResources.DescriptionPageTextStep4Description
                }
            };

        }
    }
}