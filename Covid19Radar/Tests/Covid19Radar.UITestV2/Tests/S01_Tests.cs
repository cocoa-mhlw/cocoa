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
    /// S-1　陽性者との接触確認シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S01_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S01_Tests(Platform platform)
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
        /// 濃厚接触0回(日本語).
        /// </summary>
        [Test]
        public void Case01_Test()
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // S1 ホーム画面で、「陽性者との接触を確認する」ボタンを押下
            ExposureCheckPage exposureCheckPage = homePage.OpenExposureCheckPage();
            exposureCheckPage.AssertExposureCheckPage();
        }

        /// <summary>
        /// 濃厚接触1回(日本語).
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case02_Test()
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、ハンバーガーメニューを押下
            MenuPage menuPage = homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // ハンバーガーメニューから、デバッグメニューを押下
            DebugPage debugPage = menuPage.OpenDebugPage();
            debugPage.AssertDebugPage();

            // デバッグメニューで「ContactedNotifyPage」ボタンを押下し、接触あり状態の「過去14日間の接触確認」画面に遷移
            ManageExposureDataPage manageExposureDataPage = debugPage.OpenManageExposureDataPage();
            manageExposureDataPage.AssertManageExposureDataPage();
            manageExposureDataPage.TapManageExposureDataPageHigh();
            manageExposureDataPage.AssertManageExposureDataPage();

            AppManager.ReStartApp();
            homePage.AssertHomePage();

            // S1 ホーム画面で、「陽性者との接触を確認する」ボタンを押下
            ExposureCheckPage exposureCheckPage = homePage.OpenExposureCheckPage();
            exposureCheckPage.AssertExposureCheckPage();
        }
    }
}
