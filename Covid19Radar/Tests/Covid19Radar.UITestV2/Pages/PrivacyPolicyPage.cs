using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// PrivacyPolicyPageクラス.
    /// </summary>
    public class PrivacyPolicyPage : BasePage
    {
        /***********
         * プライバシーポリシー (チュートリアル内)
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string PrivacyPolicyPageTitle = "PrivacyPolicyPageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openTutorialPage4;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public PrivacyPolicyPage()
        {
            if (OnAndroid)
            {
                openTutorialPage4 = x => x.Marked(PrivacyPolicyPageTitle).Class(ButtonRenderer).Index(0);
            }

            if (OniOS)
            {
                openTutorialPage4 = x => x.Marked(PrivacyPolicyPageTitle).Class(UIButton).Index(0);
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(PrivacyPolicyPageTitle),
            iOS = x => x.Marked(PrivacyPolicyPageTitle),
        };

        /// <summary>
        /// PrivacyPolicyPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        // メニュー表示確認
        public void AssertPrivacyPolicyPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// TutorialPage4に遷移する.
        /// </summary>
        /// <returns>TutorialPage4.</returns>
        public TutorialPage4 OpenTutorialPage4()
        {
            app.Tap(openTutorialPage4);
            return new TutorialPage4();
        }
    }
}
