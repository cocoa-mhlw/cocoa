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
    /// S-16　お問い合わせ-動作情報の送信シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S16_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S16_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// お問い合わせ-動作情報の送信(機内モードOFF).
        /// </summary>
        [Test]
        public void Case01_Test()
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // S1 ホーム画面で、ハンバーガーメニューを押下
            MenuPage menuPage = homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // S2 ハンバーガーメニューで、「お問い合わせ」ボタンを押下
            InqueryPage inqueryPage = menuPage.OpenInqueryPage();
            inqueryPage.AssertInqueryPage();

            // S3 「アプリに関するお問い合わせ」画面で、「動作情報を送信」ボタンを押下
            SendLogConfirmationPage sendLogConfirmationPage = inqueryPage.OpenSendLogConfirmationPage();
            sendLogConfirmationPage.AssertSendLogConfirmationPage();

            // S4「動作情報の送信について」画面で、「同意して送信する」ボタンを押下
            SendLogCompletePage sendLogCompletePage = sendLogConfirmationPage.OpenSubmitConsentPage();
            sendLogCompletePage.AssertSendLogCompletePage();

            // S5 「動作情報を送信しました～」画面で、「メールで動作情報IDを送信する」ボタンを押下
            sendLogCompletePage.OpenMail();

            // メールアプリが立ち上がるまで待機
            Thread.Sleep(3000);
        }

        /// <summary>
        /// 遷移確認のためのスクリーンショット取得.
        /// </summary>
        [TearDown]
        public override void TearDown()
        {
            app.Screenshot("Mail Check");
        }
    }
}
