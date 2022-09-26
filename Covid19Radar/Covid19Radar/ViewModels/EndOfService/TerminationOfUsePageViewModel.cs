// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Covid19Radar.ViewModels.EndOfService
{
    public class TerminationOfUsePageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;

        public TerminationOfUsePageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService
            ) : base(navigationService)
        {
            _loggerService = loggerService;
        }

        public IAsyncCommand OnTerminationButton => new AsyncCommand(async () =>
        {
            _loggerService.StartMethod();

            try
            {
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed termination of use", ex);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        });

    }
}

