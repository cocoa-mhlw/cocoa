// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Covid19Radar.Common;
using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels.EndOfService
{
    public class SurveyRequestPageViewModel : ViewModelBase
    {
        public string CheckDetailsButtonReadText => $"{AppResources.SurveyRequestPageCheckDetailsButton} {AppResources.Button}";

        public string SkipAnswerButtonReadText => $"{AppResources.SurveyRequestPageSkipAnswerButton} {AppResources.Button}";

        public SurveyRequestPageViewModel(
            INavigationService navigationService
            ) : base(navigationService)
        {
        }

        public IAsyncCommand CheckDetailsCommand => new AsyncCommand(async () =>
        {
            await Browser.OpenAsync(AppConstants.SurveyDetailsUrl, BrowserLaunchMode.SystemPreferred);
        });
    }
}

