using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// HelpPage3クラス.
    /// </summary>
    public class HelpPage3 : BasePage
    {
        /***********
         * 新型コロナウイルスに感染していると判定されたら
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string HelpPage3Title = "HelpPage3Title";

        /// <summary>
        /// アプリ上部のツールバー.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string HelpPage3ScrollView = "HelpPage3ScrollView";

        /// <summary>
        /// 陽性情報を登録ボタン.
        /// </summary>
        private static readonly string HelpPage3Btn = "HelpPage3Btn";

        private readonly Query backBtn;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public HelpPage3()
        {
            if (OnAndroid)
            {
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
            }

            if (OniOS)
            {
                backBtn = x => x.Class(UIButton).Index(1); // 戻るボタン
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(HelpPage3Title),
            iOS = x => x.Marked(HelpPage3Title),
        };

        /// <summary>
        /// HelpPage3のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertHelpPage3(TimeSpan? timeout = default(TimeSpan?))
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
        /// SubmitConsentPageに遷移する.
        /// </summary>
        /// <returns>SubmitConsentPage.</returns>
        public SubmitConsentPage OpenSubmitConsentPage()
        {
            app.ScrollDownTo(HelpPage3Btn, HelpPage3ScrollView);
            app.Tap(HelpPage3Btn);
            return new SubmitConsentPage();
        }
    }
}
