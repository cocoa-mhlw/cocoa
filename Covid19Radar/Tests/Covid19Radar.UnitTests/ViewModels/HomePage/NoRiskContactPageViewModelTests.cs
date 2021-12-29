// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Covid19Radar.ViewModels;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Moq;
using Prism.Navigation;
using Xunit;
using Chino;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using Covid19Radar.Resources;
using Newtonsoft.Json;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class NoRiskContactPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;

        public NoRiskContactPageViewModelTests()
        {

            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
        }

        private NoRiskContactPageViewModel CreateViewModel()
        {
            return new NoRiskContactPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object
                );
        }

    }
}
