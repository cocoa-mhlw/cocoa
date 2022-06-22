/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// TutorialPage6クラス.
    /// </summary>
    public class TutorialPage6 : BasePage
    {
        /***********
         * チュートリアルページ_6
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string TutorialPage6Title = "TutorialPage6Title";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openHomePage;
        private readonly Query openHelpMenuPage;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public TutorialPage6()
        {
            if (OnAndroid)
            {
                openHomePage = x => x.Marked(TutorialPage6Title).Class(ButtonRenderer).Index(0);
                openHelpMenuPage = x => x.Marked(TutorialPage6Title).Class(ButtonRenderer).Index(1);
            }

            if (OniOS)
            {
                openHomePage = x => x.Marked(TutorialPage6Title).Class(UIButton).Index(0);
                openHelpMenuPage = x => x.Marked(TutorialPage6Title).Class(UIButton).Index(1);
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(TutorialPage6Title),
            iOS = x => x.Marked(TutorialPage6Title),
        };

        /// <summary>
        /// TutorialPage6のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertTutorialPage6(TimeSpan? timeout = default(TimeSpan?))
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
            AppManager.DismissSpringboardAlerts();
            return new HomePage();
        }

        /// <summary>
        /// HelpMenuPageに遷移する.
        /// </summary>
        /// <returns>HelpMenuPage.</returns>
        public HelpMenuPage OpenHelpMenuPage()
        {
            app.Tap(openHelpMenuPage);
            return new HelpMenuPage();
        }
    }
}
