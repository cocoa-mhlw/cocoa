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
    public class ExposuresPageViewModel : ViewModelBase
    {
        public ExposuresPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Resources.AppResources.TitleHeadsup;
        }

        public Command OnClickPrev => (new Command(() =>
        {
            NavigationService.NavigateAsync("HomePage");
        }));
    }
}
