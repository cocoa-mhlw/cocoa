using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// SettingsPageクラス.
    /// </summary>
    public class SettingsPage : BasePage
    {
        /***********
         * 設定ページ
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string SettingsPageTitle = "SettingsPageTitle";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// toolbar.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        /// <summary>
        /// LicenseAgreementPageに遷移する.
        /// </summary>
        private static readonly string OpenLicenseAgreementPageTitle = "OpenLicenseAgreementPageTitle";

        /// <summary>
        /// SettingsPageTitleScrollViewのAutomationID.
        /// </summary>
        private static readonly string SettingsPageTitleScrollView = "SettingsPageTitleScrollView";

        /// <summary>
        /// TermsofservicePageに遷移する.
        /// </summary>
        private static readonly string OpenTermsofservicePageTitle = "OpenTermsofservicePageTitle";

        /// <summary>
        /// PrivacyPolicyPageに遷移する.
        /// </summary>
        private static readonly string OpenPrivacyPolicyPageTitle = "OpenPrivacyPolicyPageTitle";

        /// <summary>
        /// WebAccessibilityPolicyPageに遷移する.
        /// </summary>
        private static readonly string OpenWebAccessibilityPolicyPageTitle = "OpenWebAccessibilityPolicyPageTitle";

        private readonly Query backBtn;
        private readonly Query openMenuPage;
        private readonly Query tapSyokika;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public SettingsPage()
        {
            if (OnAndroid)
            {
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
                openMenuPage = x => x.Class(AppCompatImageButton).Index(0); // ハンバーガーメニュー
                tapSyokika = x => x.Marked(SettingsPageTitle).Class(ButtonRenderer).Index(0); // アプリ初期化
            }

            if (OniOS)
            {
                backBtn = x => x.Class(UIButton).Index(3); // 戻るボタン
                openMenuPage = x => x.Class(UIButton).Index(3); // ハンバーガーメニュー
                tapSyokika = x => x.Marked(SettingsPageTitle).Class(UIButton).Index(0); // アプリ初期化
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(SettingsPageTitle),
            iOS = x => x.Marked(SettingsPageTitle),
        };

        /// <summary>
        /// SettingsPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertSettingsPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// LicenseAgreementPageに遷移する.
        /// </summary>
        /// <returns>LicenseAgreementPage.</returns>
        public LicenseAgreementPage OpenLicenseAgreementPage()
        {
            app.ScrollDownTo(OpenLicenseAgreementPageTitle, SettingsPageTitleScrollView);
            app.Tap(OpenLicenseAgreementPageTitle);
            return new LicenseAgreementPage();
        }

        /// <summary>
        /// 戻るボタンを押下する.
        /// </summary>
        public void TapBackBtn()
        {
            app.Tap(backBtn);
        }

        /// <summary>
        /// 「アプリを初期化」ボタンを押下する.
        /// </summary>
        public void TapSyokika()
        {
            app.Tap(tapSyokika);
        }

        /// <summary>
        /// MenuPageに遷移する.
        /// </summary>
        /// <returns>MenuPage.</returns>
        public MenuPage OpenMenuPage()
        {
            app.Tap(openMenuPage);
            return new MenuPage();
        }

        /// <summary>
        /// ダイアログ出現時にOKボタンを押下する.
        /// </summary>
        public void TaplDialogOKBtn()
        {
            app.WaitForElement(x => x.Text("OK"));
            app.Tap("OK");
        }

        /// <summary>
        /// ダイアログ出現時にキャンセルボタンを押下する.
        /// </summary>
        /// <param name="cultureText">端末で言語.</param>
        public void TapDialogCancelBtn(string cultureText = "ja-JP")
        {
            string comparisonText = (string)AppManager.Comparison(cultureText, "ButtonCancel");
            app.WaitForElement(x => x.Text(comparisonText));
            app.Tap(comparisonText); // 陽性情報の登録をしますダイアログ→(「登録」ボタン)
        }

        /// <summary>
        /// TermsofservicePageに遷移する.
        /// </summary>
        /// <returns>LicenseAgreementPage.</returns>
        public TermsofservicePage OpenTermsofservicePage()
        {
            app.ScrollDownTo(OpenTermsofservicePageTitle, SettingsPageTitleScrollView);
            app.Tap(OpenTermsofservicePageTitle);
            return new TermsofservicePage();
        }

        /// <summary>
        /// PrivacyPolicyPageに遷移する.
        /// </summary>
        /// <returns>LicenseAgreementPage.</returns>
        public PrivacyPolicyPage2 OpenPrivacyPolicyPage2()
        {
            app.ScrollDownTo(OpenPrivacyPolicyPageTitle, SettingsPageTitleScrollView);
            app.Tap(OpenPrivacyPolicyPageTitle);
            return new PrivacyPolicyPage2();
        }

        /// <summary>
        /// WebAccessibilityPolicyPageに遷移する.
        /// </summary>
        /// <returns>LicenseAgreementPage.</returns>
        public WebAccessibilityPolicyPage OpenWebAccessibilityPolicyPage()
        {
            app.ScrollDownTo(OpenWebAccessibilityPolicyPageTitle, SettingsPageTitleScrollView);
            app.Tap(OpenWebAccessibilityPolicyPageTitle);
            return new WebAccessibilityPolicyPage();
        }
    }
}
