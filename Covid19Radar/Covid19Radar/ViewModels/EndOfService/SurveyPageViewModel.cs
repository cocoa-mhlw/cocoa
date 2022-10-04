// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views.EndOfService;
using Prism.Navigation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Covid19Radar.ViewModels.EndOfService
{
    public class SurveyPageViewModel : ViewModelBase
    {
        private void IsTerminationOfUsePageButtonEnabledChanged()
            => OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsTerminationOfUsePageButtonEnabled)));

        public DateTime MimimumStartDate
            => AppConstants.COCOA_FIRST_RELEASE_DATE.ToLocalTime();

        private int _selectedIndexQ1;
        public int SelectedIndexQ1
        {
            get => _selectedIndexQ1;
            set
            {
                if (SetProperty(ref _selectedIndexQ1, value))
                {
                    IsTerminationOfUsePageButtonEnabledChanged();
                }
            }
        }

        private int _selectedIndexQ2;
        public int SelectedIndexQ2
        {
            get => _selectedIndexQ2;
            set
            {
                if (SetProperty(ref _selectedIndexQ2, value))
                {
                    IsTerminationOfUsePageButtonEnabledChanged();
                }
            }
        }

        private SurveyAnswerPickerItem _selectedItemQ1;
        public SurveyAnswerPickerItem SelectedItemQ1
        {
            get => _selectedItemQ1;
            set => SetProperty(ref _selectedItemQ1, value);
        }

        private SurveyAnswerPickerItem _selectedItemQ2;
        public SurveyAnswerPickerItem SelectedItemQ2
        {
            get => _selectedItemQ2;
            set => SetProperty(ref _selectedItemQ2, value);
        }

        private DateTime _q3Answer;
        public DateTime Q3Answer
        {
            get => _q3Answer;
            set
            {
                if (SetProperty(ref _q3Answer, value))
                {
                    IsTerminationOfUsePageButtonEnabledChanged();
                }
            }
        }

        private bool _isExposureDataProvision;
        public bool IsExposureDataProvision
        {
            get => _isExposureDataProvision;
            set => SetProperty(ref _isExposureDataProvision, value);
        }

        public bool IsTerminationOfUsePageButtonEnabled
            => _selectedIndexQ1 > 0 && _selectedIndexQ2 > 0 && _q3Answer != null;

        private readonly IUserDataRepository _userDataRepository;
        private readonly ISurveyService _surveyService;

        public SurveyPageViewModel(
            INavigationService navigationService,
            ISurveyService surveyService,
            IUserDataRepository userDataRepository
            ) : base(navigationService)
        {
            _surveyService = surveyService;
            _userDataRepository = userDataRepository;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            // Initial value
            SelectedIndexQ1 = 0;
            SelectedIndexQ2 = 0;

            DateTime startDate = _userDataRepository.GetStartDate();
            Q3Answer = startDate.ToLocalTime();

            IsExposureDataProvision = false;
        }

        public IAsyncCommand OnToTerminationOfUsePageButton => new AsyncCommand(async () =>
        {
            SurveyContent surveyContent = await _surveyService.BuildSurveyContent(
                SelectedItemQ1.Value,
                SelectedItemQ2.Value,
                Q3Answer,
                IsExposureDataProvision);

            var navigationParameters = TerminationOfUsePage.BuildNavigationParams(surveyContent);
            await NavigationService.NavigateAsync(nameof(TerminationOfUsePage), navigationParameters);
        });
    }
}

