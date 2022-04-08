﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Linq;
using Covid19Radar.Services;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;
using Chino;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;

namespace Covid19Radar.UnitTests.Services { 
    public class ExposureDetectionServiceTests: IDisposable
    {
        #region Instance Properties

        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> loggerService;
        private readonly Mock<ILocalNotificationService> localNotificationService;
        private readonly Mock<IDebugExposureDataCollectServer> exposureDataCollectServer;
        private readonly Mock<IExposureRiskCalculationConfigurationRepository> exposureRiskCalculationConfigurationRepository;
        private readonly Mock<IExposureRiskCalculationService> exposureRiskCalculationService;
        private readonly Mock<IEventLogService> eventLogService;
        private readonly Mock<IDateTimeUtility> dateTimeUtility;
        private readonly Mock<IDeviceInfoUtility> deviceInfoUtility;


        private readonly Mock<IHttpClientService> clientService;
        private readonly Mock<ILocalPathService> localPathService;
        private readonly Mock<IPreferencesService> preferencesService;
        private readonly Mock<ISecureStorageService> secureStorageService;
        private readonly Mock<IServerConfigurationRepository> serverConfigurationRepository;
        #endregion

        #region Constructors

        public ExposureDetectionServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            loggerService = mockRepository.Create<ILoggerService>();
            localNotificationService = mockRepository.Create<ILocalNotificationService>();
            exposureDataCollectServer = mockRepository.Create<IDebugExposureDataCollectServer>();
            exposureRiskCalculationConfigurationRepository = mockRepository.Create<IExposureRiskCalculationConfigurationRepository>();
            exposureRiskCalculationService = mockRepository.Create<IExposureRiskCalculationService>();
            eventLogService = mockRepository.Create<IEventLogService>();

            clientService = mockRepository.Create<IHttpClientService>();
            localPathService = mockRepository.Create<ILocalPathService>();
            preferencesService = mockRepository.Create<IPreferencesService>();
            secureStorageService = mockRepository.Create<ISecureStorageService>();
            serverConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
            dateTimeUtility = mockRepository.Create<IDateTimeUtility>();
            deviceInfoUtility = mockRepository.Create<IDeviceInfoUtility>();


            localPathService.Setup(x => x.ExposureConfigurationDirPath).Returns($"{Path.GetTempPath()}/cocoa/");
        }

        #endregion

        public void Dispose()
        {
            Directory.Delete($"{Path.GetTempPath()}/cocoa/", true);
        }


        #region Other Private Methods

        private ExposureDetectionService CreateService()
        {
            clientService.Setup(x => x.Create())
                .Returns(new HttpClient());

            var exposureConfigurationRepository = new ExposureConfigurationRepository(
                clientService.Object,
                localPathService.Object,
                preferencesService.Object,
                serverConfigurationRepository.Object,
                dateTimeUtility.Object,
                loggerService.Object
                );

            var userDataRepository = new UserDataRepository(
                preferencesService.Object,
                dateTimeUtility.Object,
                loggerService.Object
                );

            var exposureDataRepository = new ExposureDataRepository(
                secureStorageService.Object,
                dateTimeUtility.Object,
                loggerService.Object
                );

            return new ExposureDetectionService(
                loggerService.Object,
                userDataRepository,
                exposureDataRepository,
                localNotificationService.Object,
                exposureRiskCalculationConfigurationRepository.Object,
                exposureRiskCalculationService.Object,
                exposureConfigurationRepository,
                eventLogService.Object,
                exposureDataCollectServer.Object,
                dateTimeUtility.Object,
                deviceInfoUtility.Object
                );
        }
        #endregion

        #region DiagnosisKeysDataMappingApplied
        [Fact]
        public void DiagnosisKeysDataMappingApplied_ConfigurationUpdated()
        {
            // Test Data
            var utcNow = DateTime.UtcNow;

            // Mock Setup
            dateTimeUtility.Setup(x => x.UtcNow).Returns(utcNow);
            preferencesService.
                Setup(x => x.GetValue(It.Is<string>(x => x == "IsDiagnosisKeysDataMappingConfigurationUpdated"), true))
                .Returns(true);


            // Test Case
            var unitUnderTest = CreateService();
            unitUnderTest.DiagnosisKeysDataMappingApplied();


            // Assert
            preferencesService
                .Verify
                (
                    x => x.SetValue(
                        It.Is<string>(x => x == "ExposureConfigurationAppliedEpoch"),
                        It.Is<long>(x => x.Equals(utcNow.ToUnixEpoch()))),
                    Times.Once
                );
            preferencesService
                .Verify
                (
                    x => x.SetValue(It.Is<string>(x => x == "IsDiagnosisKeysDataMappingConfigurationUpdated"), It.Is<bool>(x => x == false)),
                    Times.Once
                );
        }

        [Fact]
        public void DiagnosisKeysDataMappingApplied_ConfigurationNotUpdated()
        {
            // Test Data
            var utcNow = DateTime.UtcNow;

            // Mock Setup
            dateTimeUtility.Setup(x => x.UtcNow).Returns(utcNow);
            preferencesService.
                Setup(x => x.GetValue(It.Is<string>(x => x == "IsDiagnosisKeysDataMappingConfigurationUpdated"), false))
                .Returns(true);


            // Test Case
            var unitUnderTest = CreateService();
            unitUnderTest.DiagnosisKeysDataMappingApplied();


            // Assert
            preferencesService
                .Verify
                (
                    x => x.SetValue(
                        It.Is<string>(x => x == "ExposureConfigurationAppliedEpoch"),
                        It.IsAny<long>()),
                    Times.Never
                );
            preferencesService
                .Verify
                (
                    x => x.SetValue(It.Is<string>(x => x == "IsDiagnosisKeysDataMappingConfigurationUpdated"), It.IsAny<bool>()),
                     Times.Never
                );
        }
        #endregion

        #region ExposureWindowsDetected
        [Fact]
        public async void ExposureDetected_HighRiskExposureDetected()
        {
            // Test Data
            var exposureConfiguration = new ExposureConfiguration();
            var enVersion = 2;

            var dailySummaries = new List<DailySummary>() {
                new DailySummary()
                {
                    DateMillisSinceEpoch = 0,
                    DaySummary = new ExposureSummaryData(),
                    ConfirmedClinicalDiagnosisSummary = new ExposureSummaryData(),
                    ConfirmedTestSummary = new ExposureSummaryData(),
                    RecursiveSummary = new ExposureSummaryData(),
                    SelfReportedSummary = new ExposureSummaryData()
                }
            };

            var exposureWindows = new List<ExposureWindow>()
            {
                new ExposureWindow()
                {
                    CalibrationConfidence = CalibrationConfidence.High,
                    DateMillisSinceEpoch = 0,
                    Infectiousness = Infectiousness.High,
                    ReportType = ReportType.Unknown,
                    ScanInstances = new List<ScanInstance>()
                }
            };

            // Mock Setup
            preferencesService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "IsDiagnosisKeysDataMappingConfigurationUpdated"), false))
                .Returns(true);
            secureStorageService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "DailySummaries"), It.IsAny<string>()))
                .Returns("[]");
            secureStorageService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "ExposureWindows"), It.IsAny<string>()))
                .Returns("[]");
            exposureDataCollectServer
                .Setup(x => x.UploadExposureDataAsync(
                    It.IsAny<ExposureConfiguration>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<DailySummary>>(),
                    It.IsAny<List<ExposureWindow>>()));
            deviceInfoUtility.Setup(x => x.Model).Returns("UnitTest");

            exposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(It.IsAny<bool>()))
                .ReturnsAsync(new V1ExposureRiskCalculationConfiguration());
            exposureRiskCalculationService
                .Setup(x => x.CalcRiskLevel(It.IsAny<DailySummary>(), It.IsAny<List<ExposureWindow>>(), It.IsAny<V1ExposureRiskCalculationConfiguration>()))
                .Returns(RiskLevel.High);


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectedAsync(exposureConfiguration, enVersion, dailySummaries, exposureWindows);


            // Assert
            localNotificationService.Verify(x => x.ShowExposureNotificationAsync(), Times.Once);
        }

        [Fact]
        public async void ExposureDetected_HighRiskExposureNotDetected()
        {
            // Test Data
            var exposureConfiguration = new ExposureConfiguration();
            var enVersion = 2;

            var dailySummaries = new List<DailySummary>() {
                new DailySummary()
                {
                    DateMillisSinceEpoch = 0,
                    DaySummary = new ExposureSummaryData(),
                    ConfirmedClinicalDiagnosisSummary = new ExposureSummaryData(),
                    ConfirmedTestSummary = new ExposureSummaryData(),
                    RecursiveSummary = new ExposureSummaryData(),
                    SelfReportedSummary = new ExposureSummaryData()
                }
            };

            var exposureWindows = new List<ExposureWindow>()
            {
                new ExposureWindow()
                {
                    CalibrationConfidence = CalibrationConfidence.High,
                    DateMillisSinceEpoch = 0,
                    Infectiousness = Infectiousness.High,
                    ReportType = ReportType.Unknown,
                    ScanInstances = new List<ScanInstance>()
                }
            };

            // Mock Setup
            preferencesService.
                Setup(x => x.GetValue(It.Is<string>(x => x == "IsDiagnosisKeysDataMappingConfigurationUpdated"), false))
                .Returns(true);
            secureStorageService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "DailySummaries"), It.IsAny<string>()))
                .Returns("[]");
            secureStorageService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "ExposureWindows"), It.IsAny<string>()))
                .Returns("[]");
            exposureDataCollectServer
                .Setup(x => x.UploadExposureDataAsync(
                    It.IsAny<ExposureConfiguration>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<DailySummary>>(),
                    It.IsAny<List<ExposureWindow>>()));
            deviceInfoUtility.Setup(x => x.Model).Returns("UnitTest");

            exposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(It.IsAny<bool>()))
                .ReturnsAsync(new V1ExposureRiskCalculationConfiguration());

            exposureRiskCalculationService
                .Setup(x => x.CalcRiskLevel(It.IsAny<DailySummary>(), It.IsAny<List<ExposureWindow>>(), It.IsAny<V1ExposureRiskCalculationConfiguration>()))
                .Returns(RiskLevel.Low);

            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectedAsync(exposureConfiguration, enVersion, dailySummaries, exposureWindows);


            // Assert
            localNotificationService.Verify(x => x.ShowExposureNotificationAsync(), Times.Never);
        }

        #endregion

        #region ExposureInformationDetected
        [Fact]
        public async void ExposureDetected_ExposureInformationHighRiskExposureDetected()
        {
            // Test Data
            var exposureConfiguration = new ExposureConfiguration()
            {
                GoogleExposureConfig = new ExposureConfiguration.GoogleExposureConfiguration()
                {
                    MinimumRiskScore = 0
                }
            };
            var exposureSummary = new ExposureSummary()
            {
                 MaximumRiskScore = 1
            };
            var exposureInformantion = new ExposureInformation()
            {
                AttenuationDurationsInMillis = new int[] { 0 },
                AttenuationValue = 0,
                DateMillisSinceEpoch = 0,
                DurationInMillis = 0,
                TotalRiskScore = 2,
                TransmissionRiskLevel = RiskLevel.High
            };
            var exposureInformationList = new List<ExposureInformation>()
            {
                exposureInformantion
            };
            var enVersion = 2;

            // Mock Setup
            exposureDataCollectServer
                .Setup(x => x.UploadExposureDataAsync(
                    It.IsAny<ExposureConfiguration>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ExposureSummary>(),
                    It.IsAny<List<ExposureInformation>>()));
            deviceInfoUtility.Setup(x => x.Model).Returns("UnitTest");

            exposureRiskCalculationService
                .Setup(x => x.CalcRiskLevel(It.IsAny<DailySummary>(), It.IsAny<List<ExposureWindow>>(), It.IsAny<V1ExposureRiskCalculationConfiguration>()))
                .Returns(RiskLevel.High);


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectedAsync(exposureConfiguration, enVersion, exposureSummary, exposureInformationList);

            
            // Assert
            localNotificationService
                .Verify(x => x.ShowExposureNotificationAsync(), Times.Once);

            var expectedSerializedData = JsonConvert.SerializeObject(exposureInformationList.Select(x => new UserExposureInfo(x)));
            secureStorageService
                .Verify(x => x.SetValue<string>("ExposureInformation", It.Is<string>(x => x == expectedSerializedData)), Times.Once);
        }

        [Fact]
        public async void ExposureDetected_ExposureInformationHighRiskExposureNotDetected()
        {
            // Test Data
            var exposureConfiguration = new ExposureConfiguration()
            {
                GoogleExposureConfig = new ExposureConfiguration.GoogleExposureConfiguration()
                {
                    MinimumRiskScore = 3
                }
            };
            var exposureSummary = new ExposureSummary()
            {
                MaximumRiskScore = 1
            };
            var exposureInformantion = new ExposureInformation()
            {
                AttenuationDurationsInMillis = new int[] { 0 },
                AttenuationValue = 0,
                DateMillisSinceEpoch = 0,
                DurationInMillis = 0,
                TotalRiskScore = 2,
                TransmissionRiskLevel = RiskLevel.High
            };
            var exposureInformationList = new List<ExposureInformation>()
            {
                exposureInformantion
            };
            var enVersion = 2;

            // Mock Setup
            exposureDataCollectServer
                .Setup(x => x.UploadExposureDataAsync(
                    It.IsAny<ExposureConfiguration>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ExposureSummary>(),
                    It.IsAny<List<ExposureInformation>>()));
            deviceInfoUtility.Setup(x => x.Model).Returns("UnitTest");


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectedAsync(exposureConfiguration, enVersion, exposureSummary, exposureInformationList);


            // Assert
            localNotificationService
                .Verify(x => x.ShowExposureNotificationAsync(), Times.Never);
            secureStorageService
                .Verify(x => x.SetValue<string>("ExposureInformation", It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
