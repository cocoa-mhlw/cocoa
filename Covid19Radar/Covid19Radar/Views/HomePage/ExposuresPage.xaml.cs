/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExposuresPage : ContentPage
    {
        public const string ExposureRiskCalculationConfigurationKey = "exposures_page.exposure_risk_calculation_configuration";

        public static INavigationParameters BuildNavigationParams(
            V1ExposureRiskCalculationConfiguration exposureRiskCalculationConfiguration)
        {
            var param = new NavigationParameters();
            param.Add(ExposureRiskCalculationConfigurationKey, exposureRiskCalculationConfiguration);
            return param;
        }

        public ExposuresPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            ExposuresPageTitle.AutomationId = "ExposuresPageTitle";
#endif
        }
    }
}
