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
    /// S-5　陽性情報の登録（使い方-コロナ感染時の対応方法の確認）シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S05_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S05_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 使い方→陽性登録.
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

            // S2 「使い方」画面で、「新型コロナウイルスに感染していると判定されたら」ボタンを押下
            HelpPage3 helpPage3 = helpMenuPage.OpenHelpPage3();
            helpPage3.AssertHelpPage3();

            // S3 「感染していると判定されたら」画面で、「陽性情報を登録」ボタンを押下
            SubmitConsentPage submitConsent = helpPage3.OpenSubmitConsentPage();
            submitConsent.AssertSubmitConsentPage();

            // S4 「陽性登録への同意」画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsent.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S5 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S6 カレンダーに任意の日付を入力(自動で今日の日付が入力される)

            // S7 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S8 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();

            // S9 処理番号入力テキストボックスに処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999910");
            notifyOtherPage.AssertNotifyOtherPage();

            // S10 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            // S11 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);

            // S12 ほかの人に通知するために情報を共有しますか？画面でパターンを参照して選択肢を押下し、「陽性情報の登録」画面に遷移する。
            // 登録後成功か失敗か関数内で実装するのではなく、シナリオコード上で実装
            SubmitDiagnosisKeysCompletePage submitDiagnosisKeysCompletePage = new SubmitDiagnosisKeysCompletePage();
            submitDiagnosisKeysCompletePage.AssertSubmitDiagnosisKeysCompletePage();

            // S13 陽性登録完了画面で「ホームへ」ボタン押下
            submitDiagnosisKeysCompletePage.OpenHomePage();
            homePage.AssertHomePage();
        }
    }
}
