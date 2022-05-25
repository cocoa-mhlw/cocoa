using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// TutorialPage2クラス.
    /// </summary>
    public class TutorialPage2 : BasePage
    {
        /***********
         * チュートリアルページ_2
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string TutorialPage2Title = "TutorialPage2Title";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openTutorialPage3;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public TutorialPage2()
        {
            if (OnAndroid)
            {
                openTutorialPage3 = x => x.Marked(TutorialPage2Title).Class(ButtonRenderer).Index(0);
            }

            if (OniOS)
            {
                openTutorialPage3 = x => x.Marked(TutorialPage2Title).Class(UIButton).Index(0);
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(TutorialPage2Title),
            iOS = x => x.Marked(TutorialPage2Title),
        };

        /// <summary>
        /// TutorialPage2のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertTutorialPage2(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// TutorialPage3に遷移する.
        /// </summary>
        /// <returns>TutorialPage3.</returns>
        public TutorialPage3 OpenTutorialPage3()
        {
            app.Tap(openTutorialPage3);
            return new TutorialPage3();
        }
    }
}
