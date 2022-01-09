/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class ExposurePageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly Mock<IExposureDataRepository> mockExposureDataRepository;

        public ExposurePageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockExposureDataRepository = mockRepository.Create<IExposureDataRepository>();
        }

        private ExposuresPageViewModel CreateViewModel()
        {
            return new ExposuresPageViewModel(
                mockNavigationService.Object,
                mockUserDataRepository.Object,
                mockExposureDataRepository.Object
                );
        }

        [Fact]
        public async void ExposureSummaryGroupingTest1()
        {
            var date = DateTime.UtcNow.Date;

            List<UserExposureInfo> dummyList = new List<UserExposureInfo>()
            {
                new UserExposureInfo(date, TimeSpan.FromMinutes(15), 99, 99, RiskLevel.High),
                new UserExposureInfo(date, TimeSpan.FromMinutes(20), 98, 98, RiskLevel.Medium),
            };
            List<ExposureWindow> dummyExposureWindowList = new List<ExposureWindow>();

            mockExposureDataRepository.Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay)).Returns(dummyList);
            mockExposureDataRepository.Setup(x => x.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay)).Returns(Task.FromResult(dummyExposureWindowList));

            var vm = CreateViewModel();
            await vm.InitExposures();

            Assert.Single(vm.Exposures);
        }

        [Fact]
        public async void ExposureSummaryGroupingTest2()
        {
            var date1 = DateTime.UtcNow.Date;
            var date2 = DateTime.UtcNow.Date - TimeSpan.FromDays(1);

            List<UserExposureInfo> testList = new List<UserExposureInfo>()
            {
                new UserExposureInfo(date1, TimeSpan.FromMinutes(15), 99, 99, RiskLevel.High),
                new UserExposureInfo(date2, TimeSpan.FromMinutes(20), 98, 98, RiskLevel.Medium),
            };
            List<ExposureWindow> dummyExposureWindowList = new List<ExposureWindow>();

            mockExposureDataRepository.Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay)).Returns(testList);
            mockExposureDataRepository.Setup(x => x.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay)).Returns(Task.FromResult(dummyExposureWindowList));

            var vm = CreateViewModel();
            await vm.InitExposures();

            Assert.Equal(2, vm.Exposures.Count);
        }

        [Fact]
        public async void ExposureUnitOnceTest()
        {
            var date = DateTime.UtcNow.Date;
            List<UserExposureInfo> testList = new List<UserExposureInfo>()
            {
                new UserExposureInfo(date, TimeSpan.FromMinutes(15), 99, 99, RiskLevel.High),
            };
            List<ExposureWindow> dummyExposureWindowList = new List<ExposureWindow>();

            mockExposureDataRepository.Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay)).Returns(testList);
            mockExposureDataRepository.Setup(x => x.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay)).Returns(Task.FromResult(dummyExposureWindowList));

            var vm = CreateViewModel();
            await vm.InitExposures();

            Assert.Single(vm.Exposures);

            Assert.Equal(AppResources.ExposuresPageExposureUnitPluralOnce, vm.Exposures[0].ExposurePluralizeCount);
        }

        [Fact]
        public async void ExposureUnitPluralTest()
        {
            var date = DateTime.UtcNow.Date;

            List<UserExposureInfo> testList = new List<UserExposureInfo>()
            {
                new UserExposureInfo(date, TimeSpan.FromMinutes(15), 99, 99, RiskLevel.High),
                new UserExposureInfo(date, TimeSpan.FromMinutes(20), 98, 98, RiskLevel.Medium)
            };
            List<ExposureWindow> dummyExposureWindowList = new List<ExposureWindow>();

            mockExposureDataRepository.Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay)).Returns(testList);
            mockExposureDataRepository.Setup(x => x.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay)).Returns(Task.FromResult(dummyExposureWindowList));

            var vm = CreateViewModel();
            await vm.InitExposures();

            Assert.Single(vm.Exposures);

            Assert.Equal(string.Format(AppResources.ExposuresPageExposureUnitPlural, 2), vm.Exposures[0].ExposurePluralizeCount);
        }
    }
}
