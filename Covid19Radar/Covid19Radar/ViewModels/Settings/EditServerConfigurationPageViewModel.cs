﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Covid19Radar.Repository;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class EditServerConfigurationPageViewModel : ViewModelBase
    {
        private string _userRegisterApiEndpoint;
        public string UserRegisterApiEndpoint
        {
            get { return _userRegisterApiEndpoint; }
            set { SetProperty(ref _userRegisterApiEndpoint, value); }
        }

        private string _inquiryLogApiUrl;
        public string InquiryLogApiUrl
        {
            get { return _inquiryLogApiUrl; }
            set { SetProperty(ref _inquiryLogApiUrl, value); }
        }

        private string _logStorageEndpoint;
        public string LogStorageEndpoint
        {
            get { return _logStorageEndpoint; }
            set { SetProperty(ref _logStorageEndpoint, value); }
        }

        private string _regions;
        public string Regions
        {
            get { return _regions; }
            set { SetProperty(ref _regions, value); }
        }

        private string _diagnosisKeyRegisterApiEndpoint;
        public string DiagnosisKeyRegisterApiEndpoint
        {
            get { return _diagnosisKeyRegisterApiEndpoint; }
            set { SetProperty(ref _diagnosisKeyRegisterApiEndpoint, value); }
        }

        private string _diagnosisKeyRegisterApiUrls;
        public string DiagnosisKeyRegisterApiUrls
        {
            get { return _diagnosisKeyRegisterApiUrls; }
            set { SetProperty(ref _diagnosisKeyRegisterApiUrls, value); }
        }

        private string _diagnosisKeyListProvideServerEndpoint;
        public string DiagnosisKeyListProvideServerEndpoint
        {
            get { return _diagnosisKeyListProvideServerEndpoint; }
            set { SetProperty(ref _diagnosisKeyListProvideServerEndpoint, value); }
        }

        private string _diagnosisKeyListProvideServerUrls;
        public string DiagnosisKeyListProvideServerUrls
        {
            get { return _diagnosisKeyListProvideServerUrls; }
            set { SetProperty(ref _diagnosisKeyListProvideServerUrls, value); }
        }

        private string _exposureConfigurationUrl;
        public string ExposureConfigurationUrl
        {
            get { return _exposureConfigurationUrl; }
            set { SetProperty(ref _exposureConfigurationUrl, value); }
        }

        private string _exposureRiskCalculationConfigurationUrl;
        public string ExposureRiskCalculationConfigurationUrl
        {
            get { return _exposureRiskCalculationConfigurationUrl; }
            set { SetProperty(ref _exposureRiskCalculationConfigurationUrl, value); }
        }

        private string _exposureDataCollectServerEndpoint;
        public string ExposureDataCollectServerEndpoint
        {
            get { return _exposureDataCollectServerEndpoint; }
            set { SetProperty(ref _exposureDataCollectServerEndpoint, value); }
        }

        private string _exposureDataCollectServerUrls;
        public string ExposureDataCollectServerUrls
        {
            get { return _exposureDataCollectServerUrls; }
            set { SetProperty(ref _exposureDataCollectServerUrls, value); }
        }

        private string _eventLogApiEndpoint;
        public string EventLogApiEndpoint
        {
            get { return _eventLogApiEndpoint; }
            set { SetProperty(ref _eventLogApiEndpoint, value); }
        }

        private readonly IServerConfigurationRepository _serverConfigurationRepository;

        public EditServerConfigurationPageViewModel(
            INavigationService navigationService,
            IServerConfigurationRepository serverConfigurationRepository
            ) : base(navigationService)
        {
            Title = "Edit ServerConfiguration";
            _serverConfigurationRepository = serverConfigurationRepository;

            Task.Run(async () =>
            {
                await _serverConfigurationRepository.LoadAsync();

                UserRegisterApiEndpoint = _serverConfigurationRepository.UserRegisterApiEndpoint;
                Regions = string.Join(",", _serverConfigurationRepository.Regions);
                DiagnosisKeyRegisterApiEndpoint = _serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint;
                DiagnosisKeyListProvideServerEndpoint = _serverConfigurationRepository.DiagnosisKeyListProvideServerEndpoint;
                InquiryLogApiUrl = _serverConfigurationRepository.InquiryLogApiUrl;
                LogStorageEndpoint = _serverConfigurationRepository.LogStorageEndpoint;
                ExposureConfigurationUrl = _serverConfigurationRepository.ExposureConfigurationUrl;
                ExposureRiskCalculationConfigurationUrl = _serverConfigurationRepository.ExposureRiskCalculationConfigurationUrl;
                ExposureDataCollectServerEndpoint = _serverConfigurationRepository.ExposureDataCollectServerEndpoint;
                EventLogApiEndpoint = _serverConfigurationRepository.EventLogApiEndpoint;

                UpdateUrls();
            });
        }

        private void UpdateUrls()
        {
            _serverConfigurationRepository.Regions = Regions.Replace(" ", "").Split(",").Distinct().ToArray();
            _serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint = DiagnosisKeyRegisterApiEndpoint;
            _serverConfigurationRepository.DiagnosisKeyListProvideServerEndpoint = DiagnosisKeyListProvideServerEndpoint;
            _serverConfigurationRepository.ExposureDataCollectServerEndpoint = ExposureDataCollectServerEndpoint;
            _serverConfigurationRepository.EventLogApiEndpoint = EventLogApiEndpoint;

            var diagnosisKeyRegisterApiUrls = _serverConfigurationRepository.DiagnosisKeyRegisterApiUrls;
            DiagnosisKeyRegisterApiUrls = string.Join(Environment.NewLine, diagnosisKeyRegisterApiUrls);

            var diagnosisKeyListProvideServerUrls = _serverConfigurationRepository.DiagnosisKeyListProvideServerUrls;
            DiagnosisKeyListProvideServerUrls = string.Join(Environment.NewLine, diagnosisKeyListProvideServerUrls);

            var exposureDataCollectServerUrls = _serverConfigurationRepository.ExposureDataCollectServerUrls;
            ExposureDataCollectServerUrls = string.Join(Environment.NewLine, exposureDataCollectServerUrls);
        }

        public ICommand OnSave => new Command(async () =>
        {
            _serverConfigurationRepository.UserRegisterApiEndpoint = UserRegisterApiEndpoint;
            _serverConfigurationRepository.Regions = Regions
                .Replace(" ", "")
                .Split(",")
                .Distinct()
                .ToArray();
            _serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint = DiagnosisKeyRegisterApiEndpoint;
            _serverConfigurationRepository.DiagnosisKeyListProvideServerEndpoint = DiagnosisKeyListProvideServerEndpoint;
            _serverConfigurationRepository.InquiryLogApiUrl = InquiryLogApiUrl;
            _serverConfigurationRepository.LogStorageEndpoint = LogStorageEndpoint;
            _serverConfigurationRepository.ExposureConfigurationUrl = ExposureConfigurationUrl;
            _serverConfigurationRepository.ExposureRiskCalculationConfigurationUrl = ExposureRiskCalculationConfigurationUrl;
            _serverConfigurationRepository.ExposureDataCollectServerEndpoint = ExposureDataCollectServerEndpoint;
            _serverConfigurationRepository.EventLogApiEndpoint = EventLogApiEndpoint;

            await _serverConfigurationRepository.SaveAsync();

            UpdateUrls();

            await Application.Current.MainPage.DisplayAlert(null, "Exposure Configuration Saved", "OK");
        });
    }
}
