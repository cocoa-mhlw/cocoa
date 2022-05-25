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
    public class DebugPage : BasePage
    {
        /***********
         * デバッグページ
        ***********/

        /// <summary>
        /// PageAutomationID.
        /// </summary>
        private static readonly string DebugPageTitle = "DebugPageTitle";

        /// <summary>
        /// ReAgreePrivacyPolicyPageボタン.
        /// </summary>
        private static readonly string ReAgreePrivacyPolicyPage = "ReAgreePrivacyPolicyPage";

        /// <summary>
        /// ReAgreeTermsOfServicePageボタン.
        /// </summary>
        private static readonly string ReAgreeTermsOfServicePage = "ReAgreeTermsOfServicePage";

        /// <summary>
        /// ContactedNotifyPageボタン.
        /// </summary>
        private static readonly string ContactedNotifyPage = "ContactedNotifyPage";

        /// <summary>
        /// ManageExposureDataPageボタン.
        /// </summary>
        private static readonly string ManageExposureDataPage = "ManageExposureDataPage";

        /// <summary>
        /// ManageUserDataPageボタン.
        /// </summary>
        private static readonly string ManageUserDataPage = "ManageUserDataPage";

        /// <summary>
        /// Androidのボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// DebugPageのScrollViewAutomationID.
        /// </summary>
        private static readonly string DebugPageScrollView = "DebugPageScrollView";

        private readonly Query openMenuPage;
        private TimeSpan ts1 = new TimeSpan(0, 0, 40); // 画面操作中にタイムアウトになることを避けるための変数

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public DebugPage()
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
            Android = x => x.Marked(DebugPageTitle),
            iOS = x => x.Marked(DebugPageTitle),
        };

        /// <summary>
        /// DebugPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertDebugPage(TimeSpan? timeout = default(TimeSpan?))
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
        /// ReAgreePrivacyPolicyPageを開く.
        /// </summary>
        /// <returns>ReAgreePrivacyPolicyPage.</returns>
        public ReAgreePrivacyPolicyPage OpenReAgreePrivacyPolicyPage()
        {
            app.ScrollDownTo(ReAgreePrivacyPolicyPage, DebugPageScrollView, timeout: ts1);
            app.Tap(ReAgreePrivacyPolicyPage);
            return new ReAgreePrivacyPolicyPage();
        }

        /// <summary>
        /// ReAgreeTermsOfServicePageを開く.
        /// </summary>
        /// <returns>ReAgreeTermsOfServicePage.</returns>
        public ReAgreeTermsOfServicePage OpenReAgreeTermsOfServicePage()
        {
            app.ScrollDownTo(ReAgreeTermsOfServicePage, DebugPageScrollView, timeout: ts1);
            app.Tap(ReAgreeTermsOfServicePage);
            return new ReAgreeTermsOfServicePage();
        }

        /// <summary>
        /// ContactedNotifyPageを開く.
        /// </summary>
        /// <returns>ContactedNotifyPage.</returns>
        public ContactedNotifyPage OpenContactedNotifyPage()
        {
            app.ScrollDownTo(ContactedNotifyPage, DebugPageScrollView, timeout: ts1);
            app.Tap(ContactedNotifyPage);
            return new ContactedNotifyPage();
        }

        /// <summary>
        /// ContactedNotifyPageを開く.
        /// </summary>
        /// <returns>ContactedNotifyPage.</returns>
        public ManageExposureDataPage OpenManageExposureDataPage()
        {
            app.ScrollDownTo(ManageExposureDataPage, DebugPageScrollView, timeout: ts1);
            app.Tap(ManageExposureDataPage);
            return new ManageExposureDataPage();
        }

        /// <summary>
        /// ContactedNotifyPageを開く.
        /// </summary>
        /// <returns>ContactedNotifyPage.</returns>
        public ManageUserDataPage OpenManageUserDataPage()
        {
            app.ScrollDownTo(ManageUserDataPage, DebugPageScrollView, timeout: ts1);
            app.Tap(ManageUserDataPage);
            return new ManageUserDataPage();
        }
    }
}
