/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ManageExposureDataPageViewModel : ViewModelBase
    {
        private string _state;
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureRiskCalculationConfigurationRepository _exposureRiskCalculationConfigurationRepository;

        public V1ExposureRiskCalculationConfiguration _config;

        public ManageExposureDataPageViewModel(
            INavigationService navigationService,
            IExposureDataRepository exposureDataRepository,
            IExposureRiskCalculationConfigurationRepository exposureRiskCalculationConfigurationRepository
            ) : base(navigationService)
        {
            Title = "Manage ExposureData";
            _exposureDataRepository = exposureDataRepository;
            _exposureRiskCalculationConfigurationRepository = exposureRiskCalculationConfigurationRepository;
        }

        private async Task ClearExposureDataAsync()
        {
            await _exposureDataRepository.RemoveDailySummariesAsync();
            await _exposureDataRepository.RemoveExposureWindowsAsync();
        }

        public ICommand OnClickClearButton => new Command(async () =>
        {
            await ClearExposureDataAsync();

            State = "接触データを消去しました。";
        });

        public ICommand OnClickGenerateLowRiskExposure => new Command(async () =>
        {
            _config = await _exposureRiskCalculationConfigurationRepository.GetExposureRiskCalculationConfigurationAsync(true);

            var (dailySummaries, exposureWindows) = GenerateDummyLowRiskData(_config);

            await ClearExposureDataAsync();
            await _exposureDataRepository.SetExposureDataAsync(dailySummaries, exposureWindows);

            State = "基準値未満の接触データを登録しました。";
        });

        public ICommand OnClickGenerateHighRiskExposure => new Command(async () =>
        {
            _config = await _exposureRiskCalculationConfigurationRepository.GetExposureRiskCalculationConfigurationAsync(true);
            var (dailySummaries, exposureWindows) = GenerateDummyHighRiskData(_config);

            await ClearExposureDataAsync();
            await _exposureDataRepository.SetExposureDataAsync(dailySummaries, exposureWindows);

            State = "接触データを登録しました。";
        });

        private (List<DailySummary>, List<ExposureWindow>) GenerateDummyLowRiskData(
            V1ExposureRiskCalculationConfiguration config
            )
        {
            var dailySummaries = new List<DailySummary>()
            {
                new DailySummary()
                {
                    DateMillisSinceEpoch = GetDateTimeUnixEpochInMillis(-1),
                    DaySummary = new ExposureSummaryData()
                    {
                        ScoreSum = 1
                    }
                },
                new DailySummary()
                {
                    DateMillisSinceEpoch = GetDateTimeUnixEpochInMillis(-2),
                    DaySummary = new ExposureSummaryData()
                    {
                        ScoreSum = config.DailySummary_DaySummary_ScoreSum.Value - 1
                    }
                },
            };

            int secondsSinceLastScanSumThreshold = (int)Math.Round(config.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Value);

            var exposureWindows = new List<ExposureWindow>()
            {
                new ExposureWindow()
                {
                    DateMillisSinceEpoch = GetDateTimeUnixEpochInMillis(-1),
                    ScanInstances = new List<ScanInstance>()
                    {
                        new ScanInstance()
                        {
                            SecondsSinceLastScan = 1,
                        },
                    }
                },
                new ExposureWindow()
                {
                    DateMillisSinceEpoch = GetDateTimeUnixEpochInMillis(-2),
                    ScanInstances = new List<ScanInstance>()
                    {
                        new ScanInstance()
                        {
                            SecondsSinceLastScan = secondsSinceLastScanSumThreshold - 1,
                        },
                    }
                },
            };

            return (dailySummaries, exposureWindows);
        }

        private (List<DailySummary>, List<ExposureWindow>) GenerateDummyHighRiskData(
            V1ExposureRiskCalculationConfiguration config
            )
        {
            var dailySummaries = new List<DailySummary>()
            {
                new DailySummary()
                {
                    DateMillisSinceEpoch = GetDateTimeUnixEpochInMillis(-1),
                    DaySummary = new ExposureSummaryData()
                    {
                        ScoreSum = config.DailySummary_DaySummary_ScoreSum.Value
                    }
                },
                new DailySummary()
                {
                    DateMillisSinceEpoch = GetDateTimeUnixEpochInMillis(-2),
                    DaySummary = new ExposureSummaryData()
                    {
                        ScoreSum = config.DailySummary_DaySummary_ScoreSum.Value + 1
                    }
                },
            };

            int secondsSinceLastScanSumThreshold = (int)Math.Round(config.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Value);

            var exposureWindows = new List<ExposureWindow>()
            {
                new ExposureWindow()
                {
                    DateMillisSinceEpoch = GetDateTimeUnixEpochInMillis(-1),
                    ScanInstances = new List<ScanInstance>()
                    {
                        new ScanInstance()
                        {
                            SecondsSinceLastScan = secondsSinceLastScanSumThreshold,
                        },
                    }
                },
                new ExposureWindow()
                {
                    DateMillisSinceEpoch = GetDateTimeUnixEpochInMillis(-2),
                    ScanInstances = new List<ScanInstance>()
                    {
                        new ScanInstance()
                        {
                            SecondsSinceLastScan = secondsSinceLastScanSumThreshold + 1,
                        },
                    }
                },
            };

            return (dailySummaries, exposureWindows);
        }

        private static long GetDateTimeUnixEpochInMillis(int offsetDays)
            => DateTime.UtcNow.AddDays(offsetDays).ToUnixEpoch() * 1000;
    }
}
