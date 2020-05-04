using System.Collections.Generic;
using System.Linq;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using System.Reactive.Linq;
using Reactive.Bindings;
using System.Reactive.Disposables;
using System;
using Reactive.Bindings.Extensions;

namespace Covid19Radar.ViewModels
{
    public class UserStatusPickerItem
    {
        public int UserStatusCode { get; set; }
        public string UserStatusName { get; set; }
    }

    public class UserStatusPageViewModel : ViewModelBase, IDisposable
    {
        private readonly UserDataService _userDataService;

        public List<string> UserStatuses { get; } = Enum.GetNames(typeof(UserStatus)).ToList();
        public ReactiveProperty<UserStatus> SelectedUserStatus { get; set; }
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();
        private UserDataModel _userData;

        public UserStatusPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            _userDataService = App.Current.Container.Resolve<UserDataService>();

            _userData = _userDataService.Get();
            SelectedUserStatus = new ReactiveProperty<UserStatus>(_userData.UserStatus).AddTo(_disposable);
            SelectedUserStatus.ObserveProperty(x => x.Value).Subscribe(OnChangeUserStatus).AddTo(_disposable);
        }

        private void OnChangeUserStatus(UserStatus userStatus)
        {
            _userData.UserStatus = userStatus;
        }

        public Command OnClickSave => (new Command(async () =>
        {
            await _userDataService.SetAsync(_userData);
            await NavigationService.NavigateAsync("HomePage");
        }));

        public void Dispose()
        {
            this._disposable.Dispose();
        }
    }
}
