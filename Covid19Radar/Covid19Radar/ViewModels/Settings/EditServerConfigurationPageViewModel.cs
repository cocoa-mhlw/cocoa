/* This Source Code Form is subject to the terms of the Mozilla Public
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

        private string _inquiryLogApiEndpoint;
        public string InquiryLogApiEndpoint
        {
            get { return _inquiryLogApiEndpoint; }
            set { SetProperty(ref _inquiryLogApiEndpoint, value); }
        }

        private string _regions;
        public string Regions
        {
            get { return _regions; }
            set { SetProperty(ref _regions, value); }
        }

        private string _subRegions;
        public string SubRegions
        {
            get { return _subRegions; }
            set { SetProperty(ref _subRegions, value); }
        }

        private bool _withRegionLevel;
        public bool WithRegionLevel
        {
            get { return _withRegionLevel; }
            set { SetProperty(ref _withRegionLevel, value); }
        }

        private string _diagnosisKeyRegisterApiEndpoint;
        public string DiagnosisKeyRegisterApiEndpoint
        {
            get { return _diagnosisKeyRegisterApiEndpoint; }
            set { SetProperty(ref _diagnosisKeyRegisterApiEndpoint, value); }
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
                SubRegions = string.Join(",", _serverConfigurationRepository.SubRegions);
                WithRegionLevel = _serverConfigurationRepository.WithRegionLevel;
                DiagnosisKeyRegisterApiEndpoint = _serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint;
                DiagnosisKeyListProvideServerEndpoint = _serverConfigurationRepository.DiagnosisKeyListProvideServerEndpoint;
                InquiryLogApiEndpoint = _serverConfigurationRepository.InquiryLogApiEndpoint;
                ExposureConfigurationUrl = _serverConfigurationRepository.ExposureConfigurationUrl;
                ExposureDataCollectServerEndpoint = _serverConfigurationRepository.ExposureDataCollectServerEndpoint;

                UpdateUrls();
            });
        }

        private void UpdateUrls()
        {
            _serverConfigurationRepository.Regions = Regions.Replace(" ", "").Split(",").Distinct().ToArray();
            _serverConfigurationRepository.SubRegions = SubRegions.Replace(" ", "").Split(",").Distinct().ToArray();
            _serverConfigurationRepository.WithRegionLevel = WithRegionLevel;
            _serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint = DiagnosisKeyRegisterApiEndpoint;
            _serverConfigurationRepository.DiagnosisKeyListProvideServerEndpoint = DiagnosisKeyListProvideServerEndpoint;
            _serverConfigurationRepository.ExposureDataCollectServerEndpoint = ExposureDataCollectServerEndpoint;

            var diagnosisKeyListProvideServerUrls = _serverConfigurationRepository.GetDiagnosisKeyListProvideServerUrls();
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
            _serverConfigurationRepository.SubRegions = SubRegions
                .Replace(" ", "")
                .Split(",")
                .Distinct()
                .ToArray();
            _serverConfigurationRepository.WithRegionLevel = WithRegionLevel;
            _serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint = DiagnosisKeyRegisterApiEndpoint;
            _serverConfigurationRepository.DiagnosisKeyListProvideServerEndpoint = DiagnosisKeyListProvideServerEndpoint;
            _serverConfigurationRepository.InquiryLogApiEndpoint = InquiryLogApiEndpoint;
            _serverConfigurationRepository.ExposureConfigurationUrl = ExposureConfigurationUrl;
            _serverConfigurationRepository.ExposureDataCollectServerEndpoint = ExposureDataCollectServerEndpoint;

            await _serverConfigurationRepository.SaveAsync();

            UpdateUrls();

            await Application.Current.MainPage.DisplayAlert(null, "Exposure Configuration Saved", "OK");
        });
    }
}
