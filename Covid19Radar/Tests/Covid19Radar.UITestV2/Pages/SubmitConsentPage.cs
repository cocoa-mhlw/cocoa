using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// SubmitVonsenPageクラス.
    /// </summary>
    public class SubmitConsentPage : BasePage
    {
        /***********
         * 陽性登録への同意
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string SubmitConsentPageTitle = "SubmitConsentPageTitle";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// toolbar.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        /// <summary>
        /// NotifyOtherPageに遷移する.
        /// </summary>
        private static readonly string SubmitConsentPageScrollBtn = "SubmitConsentPageScrollBtn";

        /// <summary>
        /// ScrollViewのAutomationID.
        /// </summary>
        private static readonly string SubmitConsentPageScrollView = "SubmitConsentPageScrollView";

        private readonly Query openNotifyOtherPage;
        private readonly Query backBtn;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public SubmitConsentPage()
        {
            if (OnAndroid)
            {
                // (陽性登録への同意画面)
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
            }

            if (OniOS)
            {
                // (陽性登録への同意画面)
                backBtn = x => x.Class(UIButton).Index(1); // 戻るボタン
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(SubmitConsentPageTitle),
            iOS = x => x.Marked(SubmitConsentPageTitle),
        };

        /// <summary>
        /// SubmitConsentPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertSubmitConsentPage(TimeSpan? timeout = default(TimeSpan?))
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
        /// NotifyOtherPageに遷移する.
        /// </summary>
        /// <returns>NotifyOtherPage.</returns>
        public NotifyOtherPage OpenNotifyOtherPage()
        {
            app.ScrollDownTo(SubmitConsentPageScrollBtn, SubmitConsentPageScrollView);
            app.Tap(SubmitConsentPageScrollBtn);
            return new NotifyOtherPage();
        }
    }
}
