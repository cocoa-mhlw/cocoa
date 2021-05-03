﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DescriptionPageViewModel : ViewModelBase
    {
        public List<StepModel> Steps { get; set; }

        public DescriptionPageViewModel(INavigationService navigationService, IStatusBarPlatformSpecific statusBarPlatformSpecific) : base(navigationService, statusBarPlatformSpecific)
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
                    Description = Resources.AppResources.DescriptionPageTextStep1Description1,
                    Description2 = Resources.AppResources.DescriptionPageTextStep1Description2,
                    StepNumber = 1
                },
                new StepModel
                {
                    Title = Resources.AppResources.DescriptionPageTitleTextStep2,
                    Image = "descriptionStep21.png",
                    Image2 = "descriptionStep22.png",
                    Description = Resources.AppResources.DescriptionPageTextStep2Description1,
                    Description2 = Resources.AppResources.DescriptionPageTextStep2Description2,
                    StepNumber = 2
                },
                new StepModel
                {
                    Title = Resources.AppResources.DescriptionPageTitleTextStep3,
                    Image = "descriptionStep3.png",
                    Description = Resources.AppResources.DescriptionPageTextStep3Description1,
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