/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        #region Static Fields

        private static readonly int ANIMATION_INTERVAL_IN_MILLIS = 1000;
        private static readonly double ANIMATION_SCALE_VALUE = 1.25;
        private static readonly uint ANIMATION_SCALEIN_DURATION_IN_MILLIS = 1500;
        private static readonly uint ANIMATION_SCALEOUT_DURATION_IN_MILLIS = 200;

        #endregion

        #region Instance Fields

        private CancellationTokenSource _cancellationTokenSource;
        private CachedImage _homeActiveIconImage;

        #endregion

        #region Constructors

        public HomePage()
        {
            InitializeComponent();

            _homeActiveIconImage = NameScopeExtensions.FindByName<CachedImage>(this, "home_active_icon");
        }

        #endregion

        #region Override Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            StartAnimation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            StopAnimation();
        }

        #endregion

        #region Other Private Methods

        private void StartAnimation()
        {
            StopAnimation();

            _cancellationTokenSource = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _ = _homeActiveIconImage.FadeTo(0.25, length: 0);
                    await _homeActiveIconImage.RelScaleTo(ANIMATION_SCALE_VALUE,
                        length: ANIMATION_SCALEIN_DURATION_IN_MILLIS,
                        easing: Easing.Linear
                        );
                    await _homeActiveIconImage.FadeTo(0.0);
                    await _homeActiveIconImage.RelScaleTo(-ANIMATION_SCALE_VALUE,
                        length: ANIMATION_SCALEOUT_DURATION_IN_MILLIS,
                        easing: Easing.Linear
                        );

                    await Task.Delay(ANIMATION_INTERVAL_IN_MILLIS);
                }
            },
            _cancellationTokenSource.Token);
        }

        private void StopAnimation()
        {
            if (_cancellationTokenSource is null)
            {
                return;
            }
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
        }

        #endregion
    }
}