using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Shiny.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IConnectivity _connectivityService;
        private IDialogService _dialogService;
        private string _message;

        public DelegateCommand ShowDialogCommand { get; set; }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }


        public MainPageViewModel(INavigationService navigationService, IConnectivity connectivityService, IDialogService dialogService)
            : base(navigationService, connectivityService, dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            _connectivityService = connectivityService;
            _connectivityService.WhenInternetStatusChanged().Subscribe(OnConnectivityChanged);

            OnConnectivityChanged(_connectivityService.IsInternetAvailable());
            Title = "スタート";
        }

        private void OnConnectivityChanged(bool connected)
        {
            if (connected)
            {
                ShowDialogCommand = new DelegateCommand(async () =>
                {
                    var param = new DialogParameters();
                    param.Add("Message", "The internet is connected... We can now do our anti-Social Media... and swipe right, and like our friend's lunch...");
                    _dialogService.ShowDialog("DialogView", param, CloseDialogCallback);
                });

            }
            else
            {
                ShowDialogCommand = new DelegateCommand(async () =>
                {
                    var param = new DialogParameters();
                    param.Add("Message", "Whoops! It seems the internet has gone missing... we're going into withdrawls... please bring it back...");
                    _dialogService.ShowDialog("DialogView", param, CloseDialogCallback);
                });
            }
        }

        private void CloseDialogCallback(IDialogResult obj)
        {
            // throw new NotImplementedException();
        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync("DescriptionPage");
        }));

    }
}
