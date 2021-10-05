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

        private string _diagnosisKeyListProvideServerBaseEndpoint;
        public string DiagnosisKeyListProvideServerBaseEndpoint
        {
            get { return _diagnosisKeyListProvideServerBaseEndpoint; }
            set { SetProperty(ref _diagnosisKeyListProvideServerBaseEndpoint, value); }
        }

        private string _inquiryLogApiEndpoint;
        public string InquiryLogApiEndpoint
        {
            get { return _inquiryLogApiEndpoint; }
            set { SetProperty(ref _inquiryLogApiEndpoint, value); }
        }

        private string _exposureDataCollectServerEndpoint;
        public string ExposureDataCollectServerEndpoint
        {
            get { return _exposureDataCollectServerEndpoint; }
            set { SetProperty(ref _exposureDataCollectServerEndpoint, value); }
        }

        private string _diagnosisKeyListProvideServerEndpoints;
        public string DiagnosisKeyListProvideServerEndpoints
        {
            get { return _diagnosisKeyListProvideServerEndpoints; }
            set { SetProperty(ref _diagnosisKeyListProvideServerEndpoints, value); }
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
                DiagnosisKeyListProvideServerBaseEndpoint = _serverConfigurationRepository.DiagnosisKeyListProvideServerBaseEndpoint;
                InquiryLogApiEndpoint = _serverConfigurationRepository.InquiryLogApiEndpoint;
                ExposureDataCollectServerEndpoint = _serverConfigurationRepository.ExposureDataCollectServerEndpoint;

                UpdateDiagnosisKeyListProvideServerEndpoints();
            });
        }

        private void UpdateDiagnosisKeyListProvideServerEndpoints()
        {
            var regions = Regions.Replace(" ", "").Split(",");
            _serverConfigurationRepository.Regions = Regions.Replace(" ", "").Split(",");
            _serverConfigurationRepository.DiagnosisKeyListProvideServerBaseEndpoint = DiagnosisKeyListProvideServerBaseEndpoint;

            var diagnosisKeyListProvideServerEndpoints = regions
                .Select(region => _serverConfigurationRepository.GetDiagnosisKeyListProvideServerUrl(region))
                .Select(url => $"  * {url}");

            DiagnosisKeyListProvideServerEndpoints = string.Join(Environment.NewLine, diagnosisKeyListProvideServerEndpoints);
        }

        public ICommand OnSave => new Command(async () =>
        {
            UpdateDiagnosisKeyListProvideServerEndpoints();

            _serverConfigurationRepository.UserRegisterApiEndpoint = UserRegisterApiEndpoint;
            _serverConfigurationRepository.Regions = Regions.Replace(" ", "").Split(",");

            _serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint = DiagnosisKeyRegisterApiEndpoint;
            _serverConfigurationRepository.DiagnosisKeyListProvideServerBaseEndpoint = DiagnosisKeyListProvideServerBaseEndpoint;
            _serverConfigurationRepository.InquiryLogApiEndpoint = InquiryLogApiEndpoint;
            _serverConfigurationRepository.ExposureDataCollectServerEndpoint = ExposureDataCollectServerEndpoint;

            await _serverConfigurationRepository.SaveAsync();

            await Application.Current.MainPage.DisplayAlert(null, "Exposure Configuration Saved", "OK");
        });
    }
}
