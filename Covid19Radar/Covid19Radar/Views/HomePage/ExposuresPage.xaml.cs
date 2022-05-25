/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExposuresPage : ContentPage
    {
        public ExposuresPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            ExposuresPageTitle.AutomationId = "ExposuresPageTitle";
#endif
        }
    }
}
