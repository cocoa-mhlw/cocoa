using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// TutorialPage3クラス.
    /// </summary>
    public class TutorialPage3 : BasePage
    {
        /***********
         * チュートリアルページ_3
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string TutorialPage3Title = "TutorialPage3Title";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openPrivacyPolicyPage;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public TutorialPage3()
        {
            if (OnAndroid)
            {
                openPrivacyPolicyPage = x => x.Marked(TutorialPage3Title).Class(ButtonRenderer).Index(0);
            }

            if (OniOS)
            {
                openPrivacyPolicyPage = x => x.Marked(TutorialPage3Title).Class(UIButton).Index(0);
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(TutorialPage3Title),
            iOS = x => x.Marked(TutorialPage3Title),
        };

        /// <summary>
        /// TutorialPage3のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertTutorialPage3(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// PrivacyPolicyPageに遷移する.
        /// </summary>
        /// <param name="count">再帰回数.</param>
        /// <returns>PrivacyPolicyPage.</returns>
        public PrivacyPolicyPage OpenPrivacyPolicyPage(int count = 5)
        {
            PrivacyPolicyPage pp = null;
            try
            {
                app.Tap(openPrivacyPolicyPage);
                pp = new PrivacyPolicyPage();
            }
            catch (Exception e)
            {
                if (count > 0)
                {
                    app.Tap("OK");
                    count--;
                    OpenPrivacyPolicyPage(count);
                }
                else
                {
                    throw e;
                }
            }

            return pp;
        }
    }
}
