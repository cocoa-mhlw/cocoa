using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

// (陽性情報の登録画面)
namespace CovidRadar.UITestV2
{
    /// <summary>
    /// NotifyOtherPageクラス.
    /// </summary>
    public class NotifyOtherPage : BasePage
    {
        /***********
         * 陽性情報の登録
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string NotifyOtherPageTitle = "NotifyOtherPageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// iOSのラベル.
        /// </summary>
        private static readonly string UILabel = "UILabel";

        /// <summary>
        /// Androidのラベル.
        /// </summary>
        private static readonly string LabelRenderer = "LabelRenderer";

        /// <summary>
        /// 陽性番号入力フォーム.
        /// </summary>
        private static readonly string NotifyOtherPageTitleEntry = "NotifyOtherPageTitleEntry";

        /// <summary>
        /// PageスクロールAutomationID.
        /// </summary>
        private static readonly string NotifyOtherPageTitleScrollView = "NotifyOtherPageTitleScrollView";

        /// <summary>
        /// 登録ボタン.
        /// </summary>
        private static readonly string SubmitConsentPageBtn = "SubmitConsentPageBtn";

        /// <summary>
        /// ラジオボタン.
        /// </summary>
        private static readonly string RadioButtonRenderer = "RadioButtonRenderer";

        /// <summary>
        /// ダイアログID.
        /// </summary>
        private static readonly string Button1 = "button1";

        /// <summary>
        /// toolbar.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        /// <summary>
        /// 陽性番号入力フォーム.
        /// </summary>
        private static readonly string MaterialFormsTextInputLayout = "MaterialFormsTextInputLayout";

        private readonly Query openHowToReceiveProcessingNumberBtnNotCheckRadioBtn;
        private readonly Query openHowToReceiveProcessingNumberBtnCheckedRadioBtn;
        private readonly Query symptomRadioBtn;
        private readonly Query processingNumberForm;
        private readonly Query registerBtn;
        private readonly Query registerConfirmBtn;
        private readonly Query backBtn;
        private readonly Query cancelDialogOKBtn;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public NotifyOtherPage()
        {
            if (OnAndroid)
            {
                openHowToReceiveProcessingNumberBtnNotCheckRadioBtn = x => x.Marked(NotifyOtherPageTitle).Class(LabelRenderer).Index(3); // 処理番号の取得方法(ラジオボタン未選択時)
                openHowToReceiveProcessingNumberBtnCheckedRadioBtn = x => x.Marked(NotifyOtherPageTitle).Class(LabelRenderer).Index(4); // 処理番号の取得方法(ラジオボタン選択時)
                symptomRadioBtn = x => x.Marked(NotifyOtherPageTitle).Class(RadioButtonRenderer).Index(0); // 症状の有無(あり)ラジオボタン
                processingNumberForm = x => x.Marked(NotifyOtherPageTitle).Class(MaterialFormsTextInputLayout).Index(0); // 陽性番号入力フォーム
                registerBtn = x => x.Marked(NotifyOtherPageTitle).Class(ButtonRenderer).Index(0); // 登録するボタン
                registerConfirmBtn = x => x.Id(Button1); // 2種類のボタンに同じIDが振られていることに注意。陽性情報の登録をしますダイアログ→(「登録」ボタン)　　COVID-19接触のログ記録を有効にしてください→(「OK」ボタン)
                cancelDialogOKBtn = x => x.Id(Button1); // 「登録をキャンセルしました」ダイアログでのOKボタン
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
            }

            if (OniOS)
            {
                openHowToReceiveProcessingNumberBtnNotCheckRadioBtn = x => x.Marked(NotifyOtherPageTitle).Class(UILabel).Index(5); // 処理番号の取得方法(ラジオボタン未選択時)
                openHowToReceiveProcessingNumberBtnCheckedRadioBtn = x => x.Marked(NotifyOtherPageTitle).Class(UILabel).Index(6); // 処理番号の取得方法(ラジオボタン選択時)
                symptomRadioBtn = x => x.Marked(NotifyOtherPageTitle).Class(UILabel).Index(2); // 症状の有無(あり)ラジオボタン
                processingNumberForm = x => x.Id(NotifyOtherPageTitleEntry); // 陽性番号入力フォーム
                registerBtn = x => x.Marked(NotifyOtherPageTitle).Class(UIButton).Index(0); // 登録するボタン
                backBtn = x => x.Class(UIButton).Index(1); // 戻るボタン
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(NotifyOtherPageTitle),
            iOS = x => x.Marked(NotifyOtherPageTitle),
        };

        /// <summary>
        /// NotifyOtherPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertNotifyOtherPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// 戻るボタンを押下する.
        /// </summary>
        public void TapBackBtn()
        {
            app.Tap(backBtn);
        }

        /// <summary>
        /// 「処理番号の取得方法」のリンクを押下する.
        /// </summary>
        /// <param name="ischecked">ラジオボタンの選択状態(ラジオボタンの状態によって、要素指定方法の変更をする)
        /// 未選択時:false、　「ある」「ない」いずれか選択時:true.</param>
        /// <returns>HowToReceiveProcessingNumberPage.</returns>
        public HowToReceiveProcessingNumberPage OpenHowToReceiveProcessingNumber(bool ischecked = true)
        {
            if (ischecked == true)
            {
                app.Tap(openHowToReceiveProcessingNumberBtnCheckedRadioBtn);
            }
            else
            {
                app.Tap(openHowToReceiveProcessingNumberBtnNotCheckRadioBtn);
            }

            return new HowToReceiveProcessingNumberPage();
        }

        /// <summary>
        /// 次のような症状がありますか？ラジオボタンにおいて「ある」を選択する.
        /// </summary>
        public void TapYesRadioBtn()
        {
            app.Tap(symptomRadioBtn);
        }

        /// <summary>
        /// 処理番号の入力を行う.
        /// </summary>
        /// <param name="processingNumber">入力する処理番号(8桁の数字).</param>
        public void EnterProcessingNumberForm(string processingNumber = "00000000")
        {
            if (OnAndroid)
            {
                app.ScrollDownTo(processingNumberForm);
            }

            if (OniOS)
            {
                app.ScrollDownTo(NotifyOtherPageTitleEntry, NotifyOtherPageTitleScrollView);
            }

            app.Tap(processingNumberForm);
            app.ClearText(processingNumberForm);
            app.EnterText(processingNumber);
            app.DismissKeyboard();
        }

        /// <summary>
        /// 登録するボタンを押下する.
        /// </summary>
        public void TapRegisterBtn()
        {

            if (OnAndroid)
            {
                app.ScrollDownTo(registerBtn);
            }

            if (OniOS)
            {
                app.ScrollDownTo(SubmitConsentPageBtn, NotifyOtherPageTitleScrollView);
            }

            app.Tap(registerBtn);
        }

        /// <summary>
        /// 陽性情報の登録をしますダイアログにおいて「登録」ボタンを押下する.
        /// </summary>
        /// <param name="cultureText">端末で使用中の言語.</param>
        public void TapRegisterConfirmBtn(string cultureText = "")
        {
            if (OnAndroid)
            {
                app.Tap(registerConfirmBtn);
            }

            if (OniOS)
            {
                string comparisonText = (string)AppManager.Comparison(cultureText, "ButtonRegister");
                app.Tap(comparisonText);
            }
        }

        /// <summary>
        /// 陽性情報の登録をしますダイアログにおいて「キャンセル」ボタンを押下する.
        /// </summary>
        /// <param name="cultureText">端末で使用中の言語.</param>
        public void TapRegisterCancelBtn(string cultureText = "")
        {
            string comparisonText = (string)AppManager.Comparison(cultureText, "ButtonCancel");
            app.Tap(comparisonText);
        }

        /// <summary>
        /// 登録をキャンセルしましたダイアログにおいて「OK」ボタンを押下する.
        /// </summary>
        public void TapCancelDialogOKBtn()
        {
            app.Tap("OK");
        }

        /// <summary>
        /// 登録をキャンセルしましたダイアログにおいて「OK」ボタンを押下する.
        /// </summary>
        public void OpenSubmitDiagnosisKeysCompletePage()
        {
            app.Tap("OK");
        }
    }
}
