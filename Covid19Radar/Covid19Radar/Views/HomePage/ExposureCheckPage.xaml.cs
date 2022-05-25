// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Covid19Radar.Model;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExposureCheckPage : ContentPage
    {
        public const string ExposureRiskCalculationConfigurationKey = "exposure_risk_calculation_configuration";

        public static INavigationParameters BuildNavigationParams(
            V1ExposureRiskCalculationConfiguration exposureRiskCalculationConfiguration,
            INavigationParameters? baseParam = null
            )
        {
            var param = new NavigationParameters();
            param.CopyFrom(baseParam);

            param.Add(ExposureRiskCalculationConfigurationKey, exposureRiskCalculationConfiguration);

            return param;
        }

        public ExposureCheckPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            ExposureCheckPageTitle.AutomationId = "ExposureCheckPageTitle";
#endif
        }
    }
}
