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

        private void SetData()
        {
            Steps = new List<StepModel>
            {
                new StepModel
                {
                    Title = Resources.AppResources.DescriptionPageTitleTextStep1,
                    Image = "descriptionStep1.png",
                    Description = Resources.AppResources.DescriptionPageTextStep1Description,
                    StepNumber = 1
                },
                new StepModel
                {
                    Title = Resources.AppResources.DescriptionPageTitleTextStep2,
                    Image = "descriptionStep2.png",
                    Description = Resources.AppResources.DescriptionPageTextStep2Description,
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
