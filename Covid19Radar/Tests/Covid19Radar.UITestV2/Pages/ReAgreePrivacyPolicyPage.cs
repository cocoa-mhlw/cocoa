using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// ReAgreePrivacyPolicyPageクラス.
    /// </summary>
    public class ReAgreePrivacyPolicyPage : BasePage
    {
        /***********
         * プライバシーポリシーの改訂ページ
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string ReAgreePrivacyPolicyPageTitle = "ReAgreePrivacyPolicyPageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// Androidラベル.
        /// </summary>
        private static readonly string LabelRenderer = "LabelRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UILabel = "UILabel";

        private readonly Query openHomePage;
        private readonly Query openPrivacyPolicyLink;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public ReAgreePrivacyPolicyPage()
        {
            if (OnAndroid)
            {
                openHomePage = x => x.Class(ButtonRenderer).Index(0);
                openPrivacyPolicyLink = x => x.Class(LabelRenderer).Index(2);
            }

            if (OniOS)
            {
                openHomePage = x => x.Class(UIButton).Index(0);
                openPrivacyPolicyLink = x => x.Class(UILabel).Index(2);
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(ReAgreePrivacyPolicyPageTitle),
            iOS = x => x.Marked(ReAgreePrivacyPolicyPageTitle),
        };

        /// <summary>
        /// AssertReAgreePrivacyPolicyPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertReAgreePrivacyPolicyPage(TimeSpan? timeout = default(TimeSpan?))
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

        /// <summary>
        /// プライバシーポリシー(外部ページ)に遷移する.
        /// </summary>
        public void OpenPrivacyPolicyLink()
        {
            app.Tap(openPrivacyPolicyLink);
        }
    }
}
