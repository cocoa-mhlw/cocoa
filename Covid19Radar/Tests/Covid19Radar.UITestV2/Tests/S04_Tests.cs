using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// S-4　陽性情報の登録シナリオ.
    /// </summary>
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    [Category("ja-JP")]
    public class S04_Tests : BaseTestFixture
    {
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="platform">動作OS.</param>
        public S04_Tests(Platform platform)
            : base(platform)
        {
        }

        /// <summary>
        /// 前処理.
        /// </summary>
        [SetUp]
        public override void BeforeEachTest()
        {
            AppManager.StartApp();
            TutorialPageFlow tutorialPageFlow = new TutorialPageFlow();
            tutorialPageFlow.Tutorial();
        }

        /*
        [SetUp]
        public override void BeforeEachTest()
        {
            // SetUpの無効化
        }

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            AppManager.StartApp();
            TutorialPageFlow TutorialPageFlow = new TutorialPageFlow();
            TutorialPageFlow.Tutorial_Actual();
        }

        [TearDown]
        public override void TearDown()
        {
            AppManager.ReStartApp();
        }
       */

        /// <summary>
        /// 処理誤り2回→正番号.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case03_Test()
        {
            // 前提：接触通知設定ON状態
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S1 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S2 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S3 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S4 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            // 処理誤り回数の前提を満たす作業実施
            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            // S5 処理番号入力テキストボックスにパターンを参照して処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999910");

            // S6 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // S7 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // 言語から比較する単語をjsonから取得
            string comparisonText = AppManager.RegistResultBranch(cultureText);

            app.WaitForElement(x => x.Text(comparisonText));
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S8(文字比較) 「登録が完了しました」ポップアップが表示されること
            Assert.AreEqual(message.Text, comparisonText);
        }

        /// <summary>
        /// 処理誤り2回→不正番号.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case04_Test()
        {
            // 前提：接触通知設定ON状態
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S1 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S2 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S3 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S4 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            // 処理誤り回数の前提を満たす作業実施
            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            // S5 処理番号入力テキストボックスにパターンを参照して処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999988");

            // S6 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // S7 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // 言語から比較する単語をjsonから取得
            string comparisonText = AppManager.RegistResultBranch2(cultureText);

            app.WaitForElement(x => x.Text(comparisonText));
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S8(文字比較) 「登録が完了しました」ポップアップが表示されること
            Assert.AreEqual(message.Text, comparisonText);
        }

        /// <summary>
        /// 処理誤り3回→正番号 アプリ強制終了.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case05_Test()
        {
            // 前提：接触通知設定ON状態
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S1 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S2 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S3 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S4 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            // 処理誤り回数の前提を満たす作業実施
            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            // S5 処理番号入力テキストボックスにパターンを参照して処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999910");

            // S6 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // S7 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();

            // 言語から比較する単語をjsonから取得
            string comparisonText = (string)AppManager.Comparison(cultureText, "NotifyOtherPageDiagReturnHomeTitle");

            app.WaitForElement(x => x.Text(comparisonText));
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S8(文字比較) 「登録回数の上限に達しました」ポップアップが表示されること
            Assert.AreEqual(message.Text, comparisonText);

            // S9 「登録回数の上限に達しました」ポップアップの「OK」ボタンを押下
            notifyOtherPage.TapCancelDialogOKBtn();
            homePage.AssertHomePage();
        }

        /// <summary>
        /// 処理誤り3回→不正番号　アプリ強制終了.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case06_Test()
        {
            // 前提：接触通知設定ON状態
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S1 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S2 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S3 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S4 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            // 処理誤り回数の前提を満たす作業実施
            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            // S5 処理番号入力テキストボックスにパターンを参照して処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999988");

            // S6 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // S7 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();

            // 言語から比較する単語をjsonから取得
            string comparisonText = (string)AppManager.Comparison(cultureText, "NotifyOtherPageDiagReturnHomeTitle");

            app.WaitForElement(x => x.Text(comparisonText));
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S8(文字比較) 「登録回数の上限に達しました」ポップアップが表示されること
            Assert.AreEqual(message.Text, comparisonText);

            // S9 「登録回数の上限に達しました」ポップアップの「OK」ボタンを押下
            notifyOtherPage.TapCancelDialogOKBtn();
            homePage.AssertHomePage();
        }

        /// <summary>
        /// 処理誤り4回→正番号.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case07_Test()
        {
            // 前提：接触通知設定ON状態

            // 処理誤り回数 : 4回(リセット後の0回) を満たす作業実施
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            notifyOtherPage.TapYesRadioBtn();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();

            homePage.AssertHomePage();

            // ホーム画面で、「陽性情報の登録」ボタンを押下
            homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S1 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S2 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S3 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S4 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // S5 処理番号入力テキストボックスにパターンを参照して処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999910");

            // S6 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // S7 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();

            // 言語から比較する単語をjsonから取得
            string comparisonText = AppManager.RegistResultBranch(cultureText);

            app.WaitForElement(x => x.Text(comparisonText));
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S8(文字比較) 「登録が完了しました」ポップアップが表示されること
            Assert.AreEqual(message.Text, comparisonText);
        }

        /// <summary>
        /// 処理誤り4回→不正番号.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case08_Test()
        {
            // 前提：接触通知設定ON状態

            // 処理誤り回数 : 4回(リセット後の0回) を満たす作業実施
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            notifyOtherPage.TapYesRadioBtn();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.TapCancelDialogOKBtn();

            notifyOtherPage.EnterProcessingNumberForm("99999988");
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();
            notifyOtherPage.TapCancelDialogOKBtn();

            homePage.AssertHomePage();

            // ホーム画面で、「陽性情報の登録」ボタンを押下
            homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S1 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S2 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S3 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S4 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // S5 処理番号入力テキストボックスにパターンを参照して処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999988");

            // S6 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // S7 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();

            // 言語から比較する単語をjsonから取得
            string comparisonText = AppManager.RegistResultBranch2(cultureText);

            app.WaitForElement(x => x.Text(comparisonText));
            var message = app.Query(x => x.Text(comparisonText))[0];

            // S8(文字比較) 「処理番号が誤っているか、有効期限が切れています」ポップアップが表示されること
            Assert.AreEqual(message.Text, comparisonText);
        }

        /*App Centerでは、接触通知OFF設定の前提条件を設定できないため、本ケースは手動実行
        [Test]
        public void Case10_Test()
        {
            // 前提：接触通知設定OFF状態

            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // S1 ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage_ENoff();
            submitConsentPage.AssertSubmitConsentPage();

            // S2 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S3 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S5 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S6 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();

            // S7 処理番号入力テキストボックスに処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("12345678");

            // S8 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // S9 「登録します」ポップアップの「登録」を押下、接触通知がOFFになっているので登録できない旨のエラーが表示される
            notifyOtherPage.TapRegisterConfirmBtn()
        }
        */

        /* 通知共有選択肢：「共有しない」を選ぶ動作が実装できないため、本ケースは手動実行とする
        [Test]
        public void Case11_Test()
        {
            // 前提：接触通知設定ON状態 → クリア
            // 前提：機内モードON状態 → クリア

            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // S1 ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S2 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S3 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S5 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S6 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();

            // S7 処理番号入力テキストボックスに処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999988"); // リクエスト(processNumber)桁エラーに対応する入力

            // S8 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();

            // S9 「登録します」ポップアップの「登録」を押下
            notifyOtherPage.TapRegisterConfirmBtn();

            app.Repl();
        }
        */

        /// <summary>
        /// 陽性情報の登録をしますポップアップでキャンセル押下.
        /// </summary>
        [Test]
        [Ignore("Ignore a test")]
        public void Case12_Test()
        {
            // 前提：接触通知設定ON状態 → クリア
            // 前提：機内モードON状態 → クリア
            HomePage homePage = new HomePage();
            homePage.AssertHomePage();

            // S1 ホーム画面で、「陽性情報の登録」ボタンを押下
            SubmitConsentPage submitConsentPage = homePage.OpenSubmitConsentPage();
            submitConsentPage.AssertSubmitConsentPage();

            // S2 陽性情報の同意画面で、「同意して陽性登録する」ボタンを押下
            NotifyOtherPage notifyOtherPage = submitConsentPage.OpenNotifyOtherPage();
            notifyOtherPage.AssertNotifyOtherPage();

            // S3 陽性情報の登録画面で、「次のような症状がありますか？」のラジオボタン「ある」を押下
            notifyOtherPage.TapYesRadioBtn();

            // S5 処理番号の取得方法のリンクを押下
            HowToReceiveProcessingNumberPage howToReceiveProcessingNumberPage = notifyOtherPage.OpenHowToReceiveProcessingNumber(true);
            howToReceiveProcessingNumberPage.AssertHowToReceiveProcessingNumberPage();

            // S6 処理番号の取得方法画面で「戻る」ボタンを押下
            howToReceiveProcessingNumberPage.TapBackBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // S7 処理番号入力テキストボックスに処理番号を入力
            notifyOtherPage.EnterProcessingNumberForm("99999910"); // 正しい処理番号

            // S8 「登録する」ボタンを押下
            notifyOtherPage.TapRegisterBtn();
            notifyOtherPage.AssertNotifyOtherPage();

            // 端末言語取得
            var cultureText = AppManager.GetCurrentCultureBackDoor();

            // S8 「登録します」ポップアップの「キャンセル」を押下
            notifyOtherPage.TapRegisterCancelBtn(cultureText);
            notifyOtherPage.AssertNotifyOtherPage();

            // S8 「登録をキャンセルしました」ポップアップの「OK」を押下
            notifyOtherPage.TapCancelDialogOKBtn();
            notifyOtherPage.AssertNotifyOtherPage();
        }
    }
}
