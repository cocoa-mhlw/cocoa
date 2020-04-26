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
    public class PickerItem
    {
        public int PickerCode { get; set; }
        public string PickerName { get; set; }
    }

    public class UserSettingPageViewModel : ViewModelBase, IDisposable
    {
        private readonly OTPService _otpService;
        private readonly UserDataService _userDataService;
        private string _phoneNumber;

        public List<string> UserStatuses { get; } = Enum.GetNames(typeof(UserStatus)).ToList();
        public ReactiveProperty<UserStatus> SelectedUserStatus { get; }
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                SetProperty(ref _phoneNumber, value);
                RaisePropertyChanged(nameof(IsPhoneNumberValid));
            }
        }

        public bool IsPhoneNumberValid => !string.IsNullOrWhiteSpace(PhoneNumber);

        public UserSettingPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitleStatusSettings;
            _otpService = App.Current.Container.Resolve<OTPService>();
            _userDataService = App.Current.Container.Resolve<UserDataService>();

            SelectedUserStatus = new ReactiveProperty<UserStatus>().AddTo(this.Disposable);
            this.SelectedUserStatus.ObserveProperty(x => x.Value)
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                })
                .AddTo(this.Disposable);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            PhoneNumber = string.Empty;
        }

        public Command OnClickNext => new Command(async () =>
        {
            var user = _userDataService.Get();
            await _otpService.SendOTPAsync(user, PhoneNumberWithoutMask());
            await NavigationService.NavigateAsync($"InputSmsOTPPage?phone_number={PhoneNumber}");
        }, () => IsPhoneNumberValid);

        private string PhoneNumberWithoutMask()
        {
            return new string(("+81").Concat(PhoneNumber.Where(char.IsDigit).Skip(1)).ToArray());
        }
        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
