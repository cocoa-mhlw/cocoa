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
    public class DescriptionPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public string TextHowToWorks { get; set; }
        public string TextAppDescription { get; set; }
        public string ButtonRegister { get; set; }

        public DescriptionPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = Resx.AppResources.TitleAppDescription;
            TextHowToWorks = Resx.AppResources.TextHowToWorks;
            TextAppDescription = Resx.AppResources.TextAppDescription;
            ButtonRegister = Resx.AppResources.ButtonRegister;
        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync("ConsentByUserPage");
        }));


    }
}
