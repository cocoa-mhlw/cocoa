// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using Covid19Radar.Model;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.Views.EndOfService
{
    public partial class TerminationOfUsePage : ContentPage
    {
        public const string NavigationParameterNameSurveyContent = "survey_content";

        public TerminationOfUsePage()
        {
            InitializeComponent();
        }

        public static INavigationParameters BuildNavigationParams(SurveyContent surveyContent = null)
        {
            var navigationParameters = new NavigationParameters();
            if (surveyContent != null)
            {
                navigationParameters.Add(NavigationParameterNameSurveyContent, surveyContent);
            }
            return navigationParameters;
        }
    }
}

