using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// S-11　使い方-接触有無の確認方法シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S11_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S11_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// ホーム画面から接触の確認方法画面までの遷移確認.
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

            // S2 「使い方」画面で、「接触の有無はどのように知ることができますか？」ボタンを押下
            HelpPage2 helpPage2 = helpMenuPage.OpenHelpPage2();
            helpPage2.AssertHelpPage2();
        }
    }
}
