/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// S-24　戻るボタンの機能確認シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S24_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S24_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 戻るボタンを使用した遷移確認.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case01_Test()
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // S1 ホーム画面で、「陽性者との接触を確認する」ボタンを押下
            ExposureCheckPage exposureCheckPage = homePage.OpenExposureCheckPage();
            exposureCheckPage.AssertExposureCheckPage();

            // S2「過去14日間の接触」画面左上の戻るボタンを押下
            exposureCheckPage.TapBackBtn();
            homePage.AssertHomePage();

            // S3 ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S4 「陽性登録への同意」画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S5 「陽性情報の登録」画面で、「処理番号の取得方法」リンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(false);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S6 「処理番号の取得方法」画面左上の戻るボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // S7 「陽性情報の登録」画面左上の戻るボタンを押下
            notifyOtherPage.TapBackBtn();
            submitConsentPage.AssertSubmitConsentPage();

            // S8 「陽性登録への同意」画面左上の戻るボタンを押下
            submitConsentPage.TapBackBtn();
            homePage.AssertHomePage();

            // S9 ホーム画面で、ハンバーガーメニューを押下
            MenuPage menuPage = homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S10 ハンバーガーメニューで、「設定」ボタンを押下
            SettingsPage settingsPage = menuPage.OpenSettingsPage();
            settingsPage.AssertSettingsPage();

            // S11 「設定」画面で、「ライセンス表記」を押下
            LicenseAgreementPage licenseAgreementPage = settingsPage.OpenLicenseAgreementPage();
            licenseAgreementPage.AssertLicenseAgreementPage();

            // S12 「ライセンス表記」画面左上の「設定」ボタンを押下
            licenseAgreementPage.TapBackBtn();
            settingsPage.AssertSettingsPage();

            // S13 「設定」画面で、「利用規約」ボタンを押下
            TermsofservicePage termsofservicePage = settingsPage.OpenTermsofservicePage();
            termsofservicePage.AssertTermsofservicePage();

            // S14 「利用規約」画面左上の「設定」ボタンを押下
            termsofservicePage.TapBackBtn();
            settingsPage.AssertSettingsPage();

            // S15 「設定」画面で、「プライバシーポリシー」ボタンを押下
            PrivacyPolicyPage2 privacyPolicyPage2 = settingsPage.OpenPrivacyPolicyPage2();
            privacyPolicyPage2.AssertPrivacyPolicyPage2();

            // S16 「プライバシーポリシー」画面左上の「設定」ボタンを押下
            privacyPolicyPage2.TapBackBtn();
            settingsPage.AssertSettingsPage();

            // S17 「設定」画面で、「ウェブアクセシビリティ方針」ボタンを押下
            WebAccessibilityPolicyPage webAccessibilityPolicyPage = settingsPage.OpenWebAccessibilityPolicyPage();
            webAccessibilityPolicyPage.AssertWebAccessibilityPolicyPage();

            // S18 「ウェブアクセシビリティ方針」画面左上の「設定」ボタンを押下
            webAccessibilityPolicyPage.TapBackBtn();
            settingsPage.AssertSettingsPage();

            // S19 「設定」画面左上のハンバーガーメニューを押下
            settingsPage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S20 ハンバーガーメニュー左上の戻るボタンを押下
            menuPage.TapBackBtn();
            settingsPage.AssertSettingsPage();

            // S21 「設定」画面左上のハンバーガーメニューを押下
            settingsPage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S22 ハンバーガーメニューで、「お問い合わせ」ボタンを押下
            InqueryPage inqueryPage = menuPage.OpenInqueryPage();
            inqueryPage.AssertInqueryPage();

            // S23 「アプリに関するお問い合わせ」画面で、「動作情報を送信」ボタンを押下
            SendLogConfirmationPage sendLogConfirmationPage = inqueryPage.OpenSendLogConfirmationPage();
            sendLogConfirmationPage.AssertSendLogConfirmationPage();

            // S24 「動作情報の送信について」画面左上の「戻る」ボタンを押下
            sendLogConfirmationPage.TapBackBtn();
            inqueryPage.AssertInqueryPage();

            // S25 「アプリに関するお問い合わせ」画面左上のハンバーガーメニューを押下
            inqueryPage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S26 ハンバーガーメニュー左上の戻るボタンを押下
            menuPage.TapBackBtn();
            inqueryPage.AssertInqueryPage();

            // S27 「アプリに関するお問い合わせ」画面左上のハンバーガーメニューを押下
            inqueryPage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S28 ハンバーガーメニューで、「使い方」ボタンを押下
            HelpMenuPage helpMenuPage = menuPage.OpenHelpMenuPage();
            helpMenuPage.AssertHelpMenuPage();

            // S29 「使い方」画面で、「どのように接触を記録していますか？」ボタンを押下
            HelpPage1 helpPage1 = helpMenuPage.OpenHelpPage1();
            helpPage1.AssertHelpPage1();

            // S30 「接触の記録方法」画面左上の「使い方」ボタンを押下
            helpPage1.TapBackBtn();
            helpMenuPage.AssertHelpMenuPage();

            // S31 「使い方」画面で、「接触の有無はどのように知ることができますか？」ボタンを押下
            HelpPage2 helpPage2 = helpMenuPage.OpenHelpPage2();
            helpPage2.AssertHelpPage2();

            // S32 「接触の確認方法」画面左上の「使い方」ボタンを押下
            helpPage2.TapBackBtn();
            helpMenuPage.AssertHelpMenuPage();

            // S33 「使い方」画面で、「新型コロナウイルスに感染していると判定されたら」ボタンを押下
            HelpPage3 helpPage3 = helpMenuPage.OpenHelpPage3();
            helpPage3.AssertHelpPage3();

            // S34 「感染していると判定されたら」画面で、「陽性情報を登録」ボタンを押下
            helpPage3.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S35 「陽性登録への同意」画面で、「同意して陽性登録する」ボタンを押下
            submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S36 「陽性情報の登録」画面で、「処理番号の取得方法」リンクを押下
            notifyOtherPage.OpenHowToReceiveProcessingNumber(false);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S37 「処理番号の取得方法」画面左上の「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // S38 「陽性情報の登録」画面左上の「戻る」ボタンを押下
            notifyOtherPage.TapBackBtn();
            submitConsentPage.AssertSubmitConsentPage();

            // S39 「陽性登録への同意」画面左上の「戻る」ボタンを押下
            submitConsentPage.TapBackBtn();
            helpPage3.AssertHelpPage3();

            // S40 「感染していると判定されたら」画面左上の「使い方」ボタンを押下
            helpPage3.TapBackBtn();
            helpMenuPage.AssertHelpMenuPage();

            // S41 「使い方」画面で、「接触の記録を停止 / 情報を削除するには」ボタンを押下
            HelpPage4 helpPage4 = helpMenuPage.OpenHelpPage4();
            helpPage4.AssertHelpPage4();

            // S42 「記録の停止/削除」画面で、「アプリの設定へ」ボタンを押下
            helpPage4.OpenSettingsPage();
            settingsPage.AssertSettingsPage();

            // S43 「設定」画面で、「ライセンス表記」ボタンを押下
            settingsPage.OpenLicenseAgreementPage();
            licenseAgreementPage.AssertLicenseAgreementPage();

            // S44 「ライセンス表記」画面左上の「設定」ボタンを押下
            licenseAgreementPage.TapBackBtn();
            settingsPage.AssertSettingsPage();

            // S45 「設定」画面左上の「記録の停止/削除」ボタンを押下
            settingsPage.TapBackBtn();
            helpPage4.AssertHelpPage4();

            // S46 「記録の停止/削除」画面左上の「使い方」ボタンを押下
            helpPage4.TapBackBtn();
            helpMenuPage.AssertHelpMenuPage();

            // S47 「使い方」画面左上のハンバーガーメニューを押下
            helpMenuPage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S48 ハンバーガーメニュー左上の戻るボタンを押下
            menuPage.TapBackBtn();
            helpMenuPage.AssertHelpMenuPage();
        }
    }
}
