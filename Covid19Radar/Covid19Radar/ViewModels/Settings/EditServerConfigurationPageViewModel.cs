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

        private string _diagnosisKeyRegisterApiBaseEndpoint;
        public string DiagnosisKeyRegisterApiBaseEndpoint
        {
            get { return _diagnosisKeyRegisterApiBaseEndpoint; }
            set { SetProperty(ref _diagnosisKeyRegisterApiBaseEndpoint, value); }
        }

        private string _diagnosisKeyRegisterApiUrls;
        public string DiagnosisKeyRegisterApiUrls
        {
            get { return _diagnosisKeyRegisterApiUrls; }
            set { SetProperty(ref _diagnosisKeyRegisterApiUrls, value); }
        }

        private string _diagnosisKeyListProvideServerBaseEndpoint;
        public string DiagnosisKeyListProvideServerBaseEndpoint
        {
            get { return _diagnosisKeyListProvideServerBaseEndpoint; }
            set { SetProperty(ref _diagnosisKeyListProvideServerBaseEndpoint, value); }
        }

        private string _diagnosisKeyListProvideServerUrls;
        public string DiagnosisKeyListProvideServerUrls
        {
            get { return _diagnosisKeyListProvideServerUrls; }
            set { SetProperty(ref _diagnosisKeyListProvideServerUrls, value); }
        }

        private string _exposureDataCollectServerBaseEndpoint;
        public string ExposureDataCollectServerBaseEndpoint
        {
            get { return _exposureDataCollectServerBaseEndpoint; }
            set { SetProperty(ref _exposureDataCollectServerBaseEndpoint, value); }
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
                DiagnosisKeyRegisterApiBaseEndpoint = _serverConfigurationRepository.DiagnosisKeyRegisterApiBaseEndpoint;
                DiagnosisKeyListProvideServerBaseEndpoint = _serverConfigurationRepository.DiagnosisKeyListProvideServerBaseEndpoint;
                InquiryLogApiEndpoint = _serverConfigurationRepository.InquiryLogApiEndpoint;
                ExposureDataCollectServerBaseEndpoint = _serverConfigurationRepository.ExposureDataCollectServerBaseEndpoint;

                UpdateUrls();
            });
        }

        private void UpdateUrls()
        {
            _serverConfigurationRepository.Regions = Regions.Replace(" ", "").Split(",").Distinct().ToArray();
            _serverConfigurationRepository.DiagnosisKeyRegisterApiBaseEndpoint = DiagnosisKeyRegisterApiBaseEndpoint;
            _serverConfigurationRepository.DiagnosisKeyListProvideServerBaseEndpoint = DiagnosisKeyListProvideServerBaseEndpoint;
            _serverConfigurationRepository.ExposureDataCollectServerBaseEndpoint = ExposureDataCollectServerBaseEndpoint;

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
            _serverConfigurationRepository.DiagnosisKeyRegisterApiBaseEndpoint = DiagnosisKeyRegisterApiBaseEndpoint;
            _serverConfigurationRepository.DiagnosisKeyListProvideServerBaseEndpoint = DiagnosisKeyListProvideServerBaseEndpoint;
            _serverConfigurationRepository.InquiryLogApiEndpoint = InquiryLogApiEndpoint;
            _serverConfigurationRepository.ExposureDataCollectServerBaseEndpoint = ExposureDataCollectServerBaseEndpoint;

            await _serverConfigurationRepository.SaveAsync();

            UpdateUrls();

            await Application.Current.MainPage.DisplayAlert(null, "Exposure Configuration Saved", "OK");
        });
    }
}
