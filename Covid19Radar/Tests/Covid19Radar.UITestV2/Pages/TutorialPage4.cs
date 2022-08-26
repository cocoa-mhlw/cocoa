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
    /// TutorialPage4クラス.
    /// </summary>
    public class TutorialPage4 : BasePage
    {
        /***********
         * チュートリアルページ_4
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string TutorialPage4Title = "TutorialPage4Title";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// あとで設定するボタン.
        /// </summary>
        private static readonly string OpenEventLogCooperationPageButton2 = "OpenEventLogCooperationPageButton2";

        /// <summary>
        /// 4ScrollViewのAutomationID.
        /// </summary>
        private static readonly string TutorialPage4ScrollView = "TutorialPage4ScrollView";

        /// <summary>
        /// 有効にするボタン.
        /// </summary>
        private static readonly string OpenEventLogCooperationPageButton = "OpenEventLogCooperationPageButton";

        private readonly Query openEventLogCooperationPage;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public TutorialPage4()
        {
            if (OnAndroid)
            {
                openEventLogCooperationPage = x => x.Marked(TutorialPage4Title).Class(ButtonRenderer).Index(0);
            }

            if (OniOS)
            {
                openEventLogCooperationPage = x => x.Marked(TutorialPage4Title).Class(UIButton).Index(0);
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(TutorialPage4Title),
            iOS = x => x.Marked(TutorialPage4Title),
        };

        /// <summary>
        /// TutorialPage4のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertTutorialPage4(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// 「有効にする」ボタンを押下し、EventLogCooperationPageに遷移する.
        /// </summary>
        /// <returns>TutorialPage6.</returns>
        public EventLogCooperationPage OpenEventLogCooperationPage()
        {
            app.ScrollDownTo(OpenEventLogCooperationPageButton, TutorialPage4ScrollView);
            app.Tap(OpenEventLogCooperationPageButton);
            return new EventLogCooperationPage();
        }

        /// <summary>
        /// 「あとで設定する」ボタンを押下し、EventLogCooperationPageに遷移する.
        /// </summary>
        /// <returns>TutorialPage6.</returns>
        public EventLogCooperationPage OpenEventLogCooperationPageBluetoothOff()
        {
            app.ScrollDownTo(OpenEventLogCooperationPageButton2, TutorialPage4ScrollView);
            app.Tap(OpenEventLogCooperationPageButton2);
            return new EventLogCooperationPage();
        }
    }
}
