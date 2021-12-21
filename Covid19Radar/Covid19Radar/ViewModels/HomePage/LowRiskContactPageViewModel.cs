// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class LowRiskContactPageViewModel: ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public LowRiskContactPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            this.loggerService = loggerService;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            loggerService.StartMethod();
            loggerService.EndMethod();
        }

        private string _totalContactMinutes = "直近14日間に合計30分間";
        public string TotalContactMinutes
        {
            get => _totalContactMinutes;
            set => SetProperty(ref _totalContactMinutes, value);
        }
    }
}
