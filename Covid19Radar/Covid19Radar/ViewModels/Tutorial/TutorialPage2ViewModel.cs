using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage2ViewModel : ViewModelBase
    {
        public TutorialPage2ViewModel() : base()
        {
            Title = Resources.AppResources.TitleHowItWorks;
        }

    }
}