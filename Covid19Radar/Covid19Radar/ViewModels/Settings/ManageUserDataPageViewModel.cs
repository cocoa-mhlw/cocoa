// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Windows.Input;
using Covid19Radar.Repository;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ManageUserDataPageViewModel : ViewModelBase
    {
        private string _state;
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        private readonly IUserDataRepository _userDataRepository;

        public ManageUserDataPageViewModel(
            INavigationService navigationService,
            IUserDataRepository userDataRepository
            ) : base(navigationService)
        {
            Title = "Manage UserData";
            _userDataRepository = userDataRepository;
        }

        public ICommand OnClickResetDayOfUse => new Command(() => SetDayOfUsec(0));
        public ICommand OnClickSetDayOfUse1 => new Command(() => SetDayOfUsec(1));
        public ICommand OnClickSetDayOfUse14 => new Command(() => SetDayOfUsec(14));
        public ICommand OnClickSetDayOfUse15 => new Command(() => SetDayOfUsec(15));

        private void SetDayOfUsec(int dayOfUse)
        {
            DateTime startDateTime = DateTime.UtcNow - TimeSpan.FromDays(dayOfUse);

            _userDataRepository.SetStartDate(startDateTime);

            State = $"利用開始から{dayOfUse}日に設定しました";
        }
    }
}
