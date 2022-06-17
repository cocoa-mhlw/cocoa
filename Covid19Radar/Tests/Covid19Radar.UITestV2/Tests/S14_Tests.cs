using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// S-14　お問い合わせ-よくある質問シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S14_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S14_Tests(Platform platform)
            : base(platform)
        {
        }


        /// <summary>
        /// tutorial実行.
        /// </summary>
        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            AppManager.StartApp();
            TutorialPageFlow tutorialPageFlow = new TutorialPageFlow();
            tutorialPageFlow.Tutorial();
        }

        /// <summary>
        /// ホーム画面からよくある質問への遷移確認.
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

            // S3 「アプリに関するお問い合わせ」画面で、「よくある質問」ボタンを押下
            inqueryPage.TapOpenFAQBtn();

            // Browserが立ち上がるまで待機
            Thread.Sleep(5000);
        }

        /// <summary>
        /// 後処理.
        /// </summary>
        [TearDown]
        public override void TearDown()
        {
            app.Screenshot("(Manual) Browser Check");
            if (OnAndroid)
            {
                // エミュレーターもしくは実機環境で動作させた場合
                app.Invoke("FinishAndRemoveTask");
            }
        }
    }
}
