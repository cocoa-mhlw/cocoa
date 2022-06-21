/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Configuration;
using Xamarin.UITest.Queries;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// S-10　使い方-接触記録方法の確認確認シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S10_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S10_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// ホーム画面から「接触の記録方法」画面までの遷移確認.
        /// </summary>
        [Test]
        public void Case01_Test()
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // S1 ホーム画面で、ハンバーガーメニュー内の「使い方」ボタンを押下
            MenuPage menuPage = homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            HelpMenuPage helpMenuPage = menuPage.OpenHelpMenuPage();
            helpMenuPage.AssertHelpMenuPage();

            // S2 「使い方」画面で、「どのように接触を記録していますか？」ボタンを押下
            HelpPage1 helpPage1 = helpMenuPage.OpenHelpPage1();
            helpPage1.AssertHelpPage1();
        }
    }
}
