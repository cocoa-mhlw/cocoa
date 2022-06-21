/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// S-12　初期化(使い方→設定)シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class S12_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S12_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 使い方→アプリ初期化(日本語).
        /// </summary>
        [Test]
        public void Case01_Test()
        {
            HomePage home = new HomePage();
            home.AssertHomePage();

            MenuPage menuPage = home.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S1 ハンバーガーメニューで、「使い方」ボタンを押下
            HelpMenuPage helpMenuPage = menuPage.OpenHelpMenuPage();
            helpMenuPage.AssertHelpMenuPage();

            // S2 「使い方」画面で、「接触の記録を停止 / 情報を削除するには」ボタンを押下
            HelpPage4 helpPage4 = helpMenuPage.OpenHelpPage4();
            helpPage4.AssertHelpPage4();

            // S3 「記録の停止 / 削除」画面で、「アプリの設定へ」ボタンを押下
            SettingsPage settingsPage = helpPage4.OpenSettingsPage();
            settingsPage.AssertSettingsPage();

            // S4 「設定」画面で、「ライセンス表記」ボタンを押下
            LicenseAgreementPage licenseAgreementPage = settingsPage.OpenLicenseAgreementPage();
            licenseAgreementPage.AssertLicenseAgreementPage();

            // ライセンスページ部分の操作は自動テストではスキップ
            // var h1 = app.Query(c => c.Css("a"));
            // app.ScrollDown("https");
            // app.ScrollDown(x => x.Marked("LicenseAgreementPageTitle").Class("WebView"));

            // Xamarin.UITest.Configuration.AndroidAppConfigurator.WaitTimes.WaitForTimeout (1000);
            // app.Tap(h1.Value);

            // app.Tap(c => c.Marked("PrivacyPolicyPageTitle").Class("ButtonRenderer").Index(0));
            // app.Tap(h1.Value);
            // Console.WriteLine(h1.Html);

            // S13「ライセンス表記」画面から「設定」画面に戻る
            app.Back();

            // S14 「設定」画面で、「アプリを初期化」ボタンを押下
            settingsPage.TapSyokika();
            settingsPage.AssertSettingsPage();

            // S15 「設定」画面で、「アプリの初期化を行います」ポップアップで「キャンセル」ボタンを押下
            settingsPage.TapDialogCancelBtn();
            settingsPage.AssertSettingsPage();

            // S16 「設定」画面で、「アプリを初期化」ボタンを押下
            settingsPage.TapSyokika();
            settingsPage.AssertSettingsPage();

            // S17 「設定」画面で、「アプリの初期化を行います」ポップアップで「OK」ボタンを押下
            settingsPage.TaplDialogOKBtn();
            settingsPage.AssertSettingsPage();

            // S18 初期化
            settingsPage.TaplDialogOKBtn();

            // S19 アプリを起動し、初回ナビゲーションが始まることを確認
            AppManager.ReStartApp();
            TutorialPage1 tutorialPage1 = new TutorialPage1();
            tutorialPage1.AssertTutorialPage1();
        }

        /// <summary>
        /// 使い方→アプリ初期化
        /// 本テストは端末の設定言語に応じて、対応ケースが変わる
        /// 日本語:Case1に相当
        /// 英語:Case2に相当
        /// 中国語:Case3に相当.
        /// </summary>
        [Test]
        [Category("en-US")]
        [Category("zh-CN")]
        [Ignore("Ignore a test")]
        public void Case02_Test()
        {
            HomePage home = new HomePage();
            home.AssertHomePage();

            MenuPage menuPage = home.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S1 ホーム画面で、ハンバーガーメニュー内の「使い方」ボタンを押下
            HelpMenuPage helpMenuPage = menuPage.OpenHelpMenuPage();
            helpMenuPage.AssertHelpMenuPage();

            // S2 「使い方」画面で、「接触の記録を停止 / 情報を削除するには」ボタンを押下
            HelpPage4 helpPage4 = helpMenuPage.OpenHelpPage4();
            helpPage4.AssertHelpPage4();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            // 言語から比較する単語をjsonから取得
            string comparisonText = (string)AppManager.Comparison(cultureText, "HelpPage4Title");

            app.WaitForElement(x => x.Text(comparisonText));
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S8(文字比較) 英語、中国語いずれかの設定言語に合わせて、ページの言語が表示されていることを確認
            Assert.AreEqual(message.Text, comparisonText);
        }
    }
    }
