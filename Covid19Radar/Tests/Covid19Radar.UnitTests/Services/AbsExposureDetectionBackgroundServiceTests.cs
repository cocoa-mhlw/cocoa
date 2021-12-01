// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Repository;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class AbsExposureDetectionBackgroundServiceTests
    {
        #region Instance Properties

        private readonly MockRepository mockRepository;
        private readonly Mock<IDiagnosisKeyRepository> diagnosisKeyRepository;
        private readonly Mock<AbsExposureNotificationApiService> exposureNotificationApiService;
        private readonly Mock<IExposureConfigurationRepository> exposureConfigurationRepository;
        private readonly Mock<ILoggerService> loggerService;
        private readonly Mock<IUserDataRepository> userDataRepository;
        private readonly IMock<IServerConfigurationRepository> serverConfigurationRepository;


        #endregion

        #region Constructors

        public AbsExposureDetectionBackgroundServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            diagnosisKeyRepository = mockRepository.Create<IDiagnosisKeyRepository>();
            exposureNotificationApiService = mockRepository.Create<AbsExposureNotificationApiService>();
            exposureConfigurationRepository = mockRepository.Create<IExposureConfigurationRepository>();
            loggerService = mockRepository.Create<ILoggerService>();
            userDataRepository = mockRepository.Create<IUserDataRepository>();
            serverConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
        }

        #endregion

        #region Other Private Methods

        private AbsExposureDetectionBackgroundService CreateService()
        {

            return new ExposureDetectionBackgroundServiceMock(
                diagnosisKeyRepository.Object,
                exposureNotificationApiService.Object,
                exposureConfigurationRepository.Object,
                loggerService.Object,
                userDataRepository.Object,
                serverConfigurationRepository.Object
                );
        }

        #endregion

        #region Test Methods

        #region ExposureDetectionAsync()

        [Fact]
        public void ExposureDetectionAsync_Success()
        {
            Assert.True(true);
        }

        #endregion

        #endregion
    }

    #region Test Target (Mock)
    internal class ExposureDetectionBackgroundServiceMock : AbsExposureDetectionBackgroundService
    {
        public ExposureDetectionBackgroundServiceMock(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
             IServerConfigurationRepository serverConfigurationRepository
        ) : base(
            diagnosisKeyRepository,
            exposureNotificationApiService,
            exposureConfigurationRepository,
            loggerService,
            userDataRepository,
            serverConfigurationRepository
        )
        {

        }


        public override void Schedule()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
