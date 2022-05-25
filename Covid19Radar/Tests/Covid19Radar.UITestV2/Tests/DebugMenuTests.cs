using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// S-32　v2画面変更箇所の英訳・中国語訳確認シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]

    [Category("ja-JP")]
    [Category("en-US")]
    [Category("zh-CN")]
    public class DebugMenuTests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public DebugMenuTests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 濃厚接触ありの画面確認
        /// 本テストは端末の設定言語に応じて、対応ケースが変わる
        /// 日本語:Case1に相当
        /// 英語:Case2に相当
        /// 中国語:Case3に相当.
        /// </summary>
        [Test]
        public void Case01_Test()
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
            manageExposureDataPage.TapManageExposureDataPageLow();
            manageExposureDataPage.AssertManageExposureDataPage();
            manageExposureDataPage.TapManageExposureDataPageHigh();
            manageExposureDataPage.AssertManageExposureDataPage();

            AppManager.ReStartApp();
            homePage.AssertHomePage();

            // S1 ホーム画面で、「陽性者との接触を確認する」ボタンを押下
            ExposureCheckPage exposureCheckPage = homePage.OpenExposureCheckPage();
            exposureCheckPage.AssertExposureCheckPage();
        }


        /// <summary>
        /// 濃厚接触ありの画面確認
        /// 本テストは端末の設定言語に応じて、対応ケースが変わる
        /// 日本語:Case1に相当
        /// 英語:Case2に相当
        /// 中国語:Case3に相当.
        /// </summary>
        [Test]
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
            ManageUserDataPage manageUserDataPage = debugPage.OpenManageUserDataPage();
            manageUserDataPage.AssertManageUserDataPage();
            manageUserDataPage.TapManageUserDataPage1day();
            manageUserDataPage.AssertManageUserDataPage();

            AppManager.FinishAndRemoveTask();
            AppManager.ReStartApp();
            homePage.AssertHomePage();
        }
    }
}
