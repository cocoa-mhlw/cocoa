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
    [Category("ja-JP")]
    public class S36_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S36_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 使い方→アプリ初期化(日本語).
        /// </summary>
        [Test]
        public void Case01_Test()
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、ハンバーガーメニューを押下
            MenuPage menuPage = homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S2 ハンバーガーメニューで、「設定」ボタンを押下
            SettingsPage settingsPage = menuPage.OpenSettingsPage();
            settingsPage.AssertSettingsPage();

            // S3 「設定」画面で、「アプリを初期化」ボタンを押下
            LicenseAgreementPage licenseAgreementPage = settingsPage.OpenLicenseAgreementPage();
            licenseAgreementPage.AssertLicenseAgreementPage();

            // Browserが立ち上がるまで待機
            Thread.Sleep(5000);
        }

        /// <summary>
        /// 遷移を確認するためのスクリーンショット取得と、アプリを確実に終了するための処理.
        /// </summary>
        [TearDown]
        public override void TearDown()
        {
            app.Screenshot("(Manual) Browser Check");
            if (OnAndroid)
            {
                app.Invoke("FinishAndRemoveTask");
            }
        }
    }
    }
