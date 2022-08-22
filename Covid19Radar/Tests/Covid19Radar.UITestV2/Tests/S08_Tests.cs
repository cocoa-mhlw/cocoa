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
    /// S-8　動作状況の確認シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S08_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S08_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// (Blootooth設定、端末のEN、位置情報)全部オン、BG復帰【動作中】.
        /// </summary>
        // TODO:課題No.71に依存するため、解決次第要修正
        [Test]
        [Ignore("Ignore a test")]
        public void Case02_Test()
        {
            // タスクキルしていない状態をここで作る
            HomePage home = new HomePage();
            home.AssertHomePage();
            AppManager.MoveTaskToBack();
            Thread.Sleep(3000);
            app.Screenshot("SplashPage Check");
            AppManager.ConnectToApp();
            app.Screenshot("SplashPage Check");

            // S1 ホーム画面に「動作中」と表示されていること
            home.AssertHomePage();

            // S2 「動作中」の下部の？ボタン押下
            home.OpenQuestionMark();
            home.AssertHomePage();
        }

        /// <summary>
        /// (Blootooth設定、端末のEN、位置情報)全部オン、再起動【動作中】.
        /// </summary>
        [Test]
        public void Case1_Test()
        {
            // S1 ホーム画面に「動作中」と表示されていること
            HomePage home = new HomePage();
            home.AssertHomePage();

            // S2 「動作中」の下部の？ボタン押下
            home.OpenQuestionMark();
            home.AssertHomePage();
        }
    }
}
