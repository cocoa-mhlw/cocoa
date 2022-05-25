using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// ContantedNotifyクラス.
    /// </summary>
    public class SubmitDiagnosisKeysCompletePage : BasePage
    {
        /***********
         * 過去14日間の接触
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string SubmitDiagnosisKeysCompletePageTitle = "SubmitDiagnosisKeysCompletePageTitle";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openHomePage;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public SubmitDiagnosisKeysCompletePage()
        {
            if (OnAndroid)
            {
                openHomePage = x => x.Marked(SubmitDiagnosisKeysCompletePageTitle).Class(ButtonRenderer).Index(0); // 動作状況を送信ボタン
            }

            if (OniOS)
            {
                openHomePage = x => x.Marked(SubmitDiagnosisKeysCompletePageTitle).Class(UIButton).Index(0); // 動作状況を送信ボタン
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(SubmitDiagnosisKeysCompletePageTitle),
            iOS = x => x.Marked(SubmitDiagnosisKeysCompletePageTitle),
        };

        /// <summary>
        /// DiagnosisKeysCompletePageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertSubmitDiagnosisKeysCompletePage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// HomePageに遷移する.
        /// </summary>
        /// <returns>HomePage.</returns>
        public HomePage OpenHomePage()
        {
            app.Tap(openHomePage);
            return new HomePage();
        }
    }
}
