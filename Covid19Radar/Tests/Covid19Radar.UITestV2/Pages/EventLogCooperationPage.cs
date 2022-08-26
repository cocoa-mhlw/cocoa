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
    public class EventLogCooperationPage : BasePage
    {
        /***********
         * チュートリアルページ_6
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string EventLogCooperationPageTitle = "EventLogCooperationPageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string EventLogCooperationPageScrollView = "EventLogCooperationPageScrollView";

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string EventLogCooperationPageBtn1 = "EventLogCooperationPageBtn1";

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string EventLogCooperationPageBtn2 = "EventLogCooperationPageBtn2";

        private readonly Query openEventLogSettingPage;
        private readonly Query openTutorialPage6;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public EventLogCooperationPage()
        {

        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(EventLogCooperationPageTitle),
            iOS = x => x.Marked(EventLogCooperationPageTitle),
        };

        /// <summary>
        /// TutorialPage6のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertEventLogCooperationPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// HelpMenuPageに遷移する.
        /// </summary>
        /// <returns>HelpMenuPage.</returns>
        public EventLogSettingPage OpenEventLogSettingPage()
        {
            app.ScrollDownTo(EventLogCooperationPageBtn1, EventLogCooperationPageScrollView);
            app.Tap(EventLogCooperationPageBtn1);
            return new EventLogSettingPage();
        }

        /// <summary>
        /// [後で設定する]を押下しTutorial6に遷移する.
        /// </summary>
        /// <returns>HelpMenuPage.</returns>
        public TutorialPage6 OpenTutorialPage6()
        {
            app.ScrollDownTo(EventLogCooperationPageBtn2, EventLogCooperationPageScrollView);
            app.Tap(EventLogCooperationPageBtn2);
            return new TutorialPage6();
        }
    }
}
