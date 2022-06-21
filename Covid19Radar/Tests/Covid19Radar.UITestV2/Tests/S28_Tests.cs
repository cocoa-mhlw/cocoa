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
    /// S-28　メンテナンスできない言語リソースが削除されていることシナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ko-KR")]
    public class S28_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S28_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 本テストは使用OSに応じて、対応ケースが変わる
        /// iOS:Case1
        /// Android:Case2.
        /// </summary>
        [Test]
        public void Case01_Test()
        {
            // 前提 : 日本語、英語、中国語以外

            /* S1
             * 【iOS】
             *  OSの設定 > 一般 > 言語と地域
             * 「編集」ボタンを押下し、「使用する言語の優先順序」内の項目を[前提]の言語以外を削除する
             */

            // S2 COCOAアプリを起動

            // S3 アプリ起動時の初期表示を確認
            HomePage home = new HomePage();
            home.AssertHomePage();

            // 言語から比較する単語をjsonから取得
            string comparisonText = (string)AppManager.Comparison("en-US", "HomePageDescription2");

            app.WaitForElement(x => x.Text(comparisonText));
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S4(文字比較) 「登録が完了しました」ポップアップが表示されること
            Assert.AreEqual(message.Text, comparisonText);
        }
    }
}
