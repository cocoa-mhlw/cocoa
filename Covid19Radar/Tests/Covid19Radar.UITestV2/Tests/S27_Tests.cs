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
    /// S-27　陽性情報の登録(期限切れの処理番号)シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]

    public class S27_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S27_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 日本語→正しい番号＆期限切れ.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case01_Test()
        {
            HomePage home = new HomePage();
            home.AssertHomePage();

            // S1 ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage openSubmitConsentPage = home.OpenSubmitConsentPage();
            openSubmitConsentPage.AssertSubmitConsentPage();

            // S2 「陽性登録への同意」画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = openSubmitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S3 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S4 カレンダーに任意の日付を入力(自動で今日の日付が入力される)

            // S5 処理番号入力テキストボックスに処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999921");

            // S6 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            // S7 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);

            // 言語から比較する単語をjsonから取得
            string comparisonText = AppManager.RegistResultBranch2(cultureText);

            // メッセージの取得
            app.WaitForElement(x => x.Text(comparisonText)); // 「comparisonText」を含むビューを取得
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S8(文字比較) 「処理番号が誤っているか、有効期限が切れています」のポップアップが表示されること
            Assert.AreEqual(message.Text, comparisonText);
        }
    }
}
