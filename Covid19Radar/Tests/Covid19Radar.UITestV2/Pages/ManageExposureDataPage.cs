using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// DebugPageクラス.
    /// </summary>
    public class ManageExposureDataPage : BasePage
    {
        /***********
         * デバッグページ
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string ManageExposureDataPageTitle = "ManageExposureDataPageTitle";

        /// <summary>
        /// ManageExposureDataPageLowボタン.
        /// </summary>
        private static readonly string ManageExposureDataPageLow = "ManageExposureDataPageLow";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// ManageExposureDataPageHighボタン.
        /// </summary>
        private static readonly string ManageExposureDataPageHigh = "ManageExposureDataPageHigh";

        private readonly Query openMenuPage;
        private TimeSpan ts1 = new TimeSpan(0, 0, 40); // 画面操作中にタイムアウトになることを避けるための変数

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public ManageExposureDataPage()
        {
            {
                openMenuPage = x => x.Class(AppCompatImageButton).Index(0); // ハンバーガーメニュー
            }

            if (OniOS)
            {
                openMenuPage = x => x.Class(UIButton).Index(3); // ハンバーガーメニュー
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(ManageExposureDataPageTitle),
            iOS = x => x.Marked(ManageExposureDataPageTitle),
        };

        /// <summary>
        /// DebugPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertManageExposureDataPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// MenuPage(ハンバーガーメニュー)を開く.
        /// </summary>
        /// <returns>MenuPage.</returns>
        public MenuPage OpenMenuPage()
        {
            app.Tap(openMenuPage);
            return new MenuPage();
        }

        /// <summary>
        /// ManageExposureDataPageLowをTapする.
        /// </summary>
        public void TapManageExposureDataPageLow()
        {
            app.Tap(ManageExposureDataPageLow);
        }

        /// <summary>
        /// ManageExposureDataPageLowをTapする.
        /// </summary>
        public void TapManageExposureDataPageHigh()
        {
            app.Tap(ManageExposureDataPageHigh);
        }
    }
}
