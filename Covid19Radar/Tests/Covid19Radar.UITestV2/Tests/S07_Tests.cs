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
    /// S-7　アプリ起動シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    [Category("en-US")]
    [Category("zh-CN")]
    [Category("ko-KR")]
    public class S07_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">プラットフォーム.</param>
        public S07_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 通常起動→プラポリ・利用規約改定(起動1回目).
        ///
        /// 本テストは端末の設定言語に応じて、対応ケースが変わる
        /// 日本語:Case1に相当
        /// 英語:Case3に相当
        /// 中国語:Case5に相当
        /// 韓国語:Case7に相当
        /// 本テストは実装の都合上、Case01_2_Test、Case01_3_Testと組み合わせて一つのケースとしている.
        /// </summary>
        [Test]
        public void Case01_1_Test()

        // S-7の遷移確認のみを行うシナリオ
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、ハンバーガーメニューを押下
            MenuPage menuPage = homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // ハンバーガーメニューから、デバッグメニューを押下
            DebugPage debugPage = menuPage.OpenDebugPage();
            debugPage.AssertDebugPage();

            // デバッグメニューで「ReAgreePrivacyPolicyPage」ボタンを押下し、「プライバシーポリシーの改定」画面に遷移
            ReAgreePrivacyPolicyPage reagreePrivacyPolicyPage = debugPage.OpenReAgreePrivacyPolicyPage();
            reagreePrivacyPolicyPage.AssertReAgreePrivacyPolicyPage();

            // 「プライバシーポリシーの改定」画面で「確認しました」ボタンを押下し、ホーム画面に遷移
            reagreePrivacyPolicyPage.OpenHomePage();
            homePage.AssertHomePage();

            // ホーム画面で、ハンバーガーメニューを押下
            homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // ハンバーガーメニューから、デバッグメニューを押下
            menuPage.OpenDebugPage();
            debugPage.AssertDebugPage();

            // デバッグメニューで「ReAgreeTermsOfServicePage」ボタンを押下し、「利用規約の改定」画面に遷移
            ReAgreeTermsOfServicePage reagreeTermsOfService = debugPage.OpenReAgreeTermsOfServicePage();
            reagreeTermsOfService.AssertReAgreeTermsOfServicePage();

            // 「利用規約の改定」画面で「確認しました」ボタンを押下し、ホーム画面に遷移
            reagreeTermsOfService.OpenHomePage();
            homePage.AssertHomePage();
        }

        /// <summary>
        /// S-7におけるプライバシーポリシーの改訂画面の確認を行うシナリオ.
        /// </summary>
        [Test]
        public void Case01_2_Test()
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、ハンバーガーメニューを押下
            MenuPage menuPage = homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // ハンバーガーメニューから、デバッグメニューを押下
            DebugPage debugPage = menuPage.OpenDebugPage();
            debugPage.AssertDebugPage();

            // デバッグメニューで「ReAgreePrivacyPolicyPage」ボタンを押下し、「プライバシーポリシーの改定」画面に遷移
            ReAgreePrivacyPolicyPage reagreePrivacyPolicyPage = debugPage.OpenReAgreePrivacyPolicyPage();
            reagreePrivacyPolicyPage.AssertReAgreePrivacyPolicyPage();

            // 「プライバシーポリシーの改定」画面で「プライバシーポリシーを確認する」リンクを押下し、外部ページに遷移
            reagreePrivacyPolicyPage.OpenPrivacyPolicyLink();

            // 外部ページの立ち上げのために待機する
            Thread.Sleep(5000);

            // スクリーンショットを取得して終了
            app.Screenshot("Browser Check");

            // 以下、Androidで外部ページに遷移した時に、次のテストに進むためにタスクを終了する必要があるための後処理
            if (OnAndroid)
            {
                app.Invoke("FinishAndRemoveTask");
            }
        }

        /// <summary>
        /// S-7における利用規約の改訂画面の確認を行うシナリオ.
        /// </summary>
        [Test]
        public void Case01_3_Test()
        {
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、ハンバーガーメニューを押下
            MenuPage menuPage = homePage.OpenMenuPage();
            menuPage.AssertMenuPage();

            // ハンバーガーメニューから、デバッグメニューを押下
            DebugPage debugPage = menuPage.OpenDebugPage();
            debugPage.AssertDebugPage();

            // デバッグメニューで「ReAgreeTermsOfServicePage」ボタンを押下し、「利用規約の改定」画面に遷移
            ReAgreeTermsOfServicePage reagreeTermsOfService = debugPage.OpenReAgreeTermsOfServicePage();
            reagreeTermsOfService.AssertReAgreeTermsOfServicePage();

            // 「利用規約の改定」画面で「利用規約を確認する」リンクを押下し、外部ページに遷移
            reagreeTermsOfService.OpenTermsOfServiceLink();

            // 外部ページの立ち上げのために待機する
            Thread.Sleep(5000);

            // スクリーンショットを取得して終了
            app.Screenshot("Browser Check");

            // 以下、Androidで外部ページに遷移した時に、次のテストに進むためにタスクを終了する必要があるための後処理
            if (OnAndroid)
            {
                app.Invoke("FinishAndRemoveTask");
            }
        }
    }
}
