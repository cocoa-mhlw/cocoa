using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage3ViewModel : ViewModelBase
    {
        public TutorialPage3ViewModel() : base()
        {
            Title = Resources.AppResources.TitleHowItWorks;
        }
    }
}