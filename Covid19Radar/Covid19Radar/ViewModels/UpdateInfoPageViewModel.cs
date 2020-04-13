using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class UpdateInfoPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public UpdateInfoPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Update Infomation";
        }

        public Command OnClickPrev => (new Command(() =>
        {
            _navigationService.NavigateAsync("HomePage");
        }));


    }
}
