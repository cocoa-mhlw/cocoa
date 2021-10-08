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
        private static readonly string ANIMATION_LABEL = "ActiveIconAnimation";

        #endregion

        #region Instance Fields

        //private CancellationTokenSource _cancellationTokenSource;
        private CachedImage _homeActiveIconImage;

        #endregion

        #region Constructors

        public HomePage()
        {
            InitializeComponent();

            _homeActiveIconImage = NameScopeExtensions.FindByName<CachedImage>(this, "home_active_icon");
            _homeActiveIconImage.Opacity = 0.25;
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

            var animation = new Animation
            {
                { 0, 0.5, new Animation (v => _homeActiveIconImage.Scale = v, 1, 2) },
                { 0.5, 0.6, new Animation (v => _homeActiveIconImage.Opacity = v, 0.25, 0.0) },
                { 0.58, 1, new Animation (v => _homeActiveIconImage.Scale = v, 2, 1) }, 
            };
            animation.Commit(
                this,
                ANIMATION_LABEL,
                16,
                3000,
                Easing.Linear,
                (v, c) =>
                {
                    _homeActiveIconImage.Scale = 1;
                    _homeActiveIconImage.Opacity = 0.25;
                },
                () => true
            );
        }

        private void StopAnimation()
        {
            this.AbortAnimation(ANIMATION_LABEL);
        }

        #endregion
    }
}