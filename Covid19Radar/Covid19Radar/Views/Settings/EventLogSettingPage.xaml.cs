// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.ViewModels;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventLogSettingPage : ContentPage
    {
        public enum TransitionReason
        {
            Tutorial,
            Splash,
            Setting
        };

        public const string TransitionReasonKey = "transition_reason";

        public static INavigationParameters BuildNavigationParams(TransitionReason transitionReason)
        {
            var param = new NavigationParameters();
            param.Add(TransitionReasonKey, transitionReason);
            return param;
        }

        public EventLogSettingPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            var backButtonEnabled = true;
            if (BindingContext is EventLogSettingPageViewModel vm)
            {
                backButtonEnabled = vm.BackButtonEnabled;
            }
            if (backButtonEnabled)
            {
                return base.OnBackButtonPressed();
            }
            else
            {
                return false;
            }
        }
    }
}

