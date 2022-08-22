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
    /// TutorialPage1クラス.
    /// </summary>
    public class TutorialPage1 : BasePage
    {
        /***********
         * チュートリアルページ_1
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string TutorialPage1Title = "TutorialPage1Title";

        /// <summary>
        /// TutorialPage1のScrollView.
        /// </summary>
        private static readonly string TutorialPage1ScrollView = "TutorialPage1ScrollView";

        /// <summary>
        /// TutorialPage2へ遷移するボタン.
        /// </summary>
        private static readonly string TutorialPage2Btn = "TutorialPage2Btn";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openTutorialPage2;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public TutorialPage1()
        {
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(TutorialPage1Title),
            iOS = x => x.Marked(TutorialPage1Title),
        };

        /// <summary>
        /// TutorialPage1のアサーション.
        /// </summary>
        /// <param name="timeout">パラメータ値.</param>
        public void AssertTutorialPage1(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// TutorialPage2に遷移する.
        /// </summary>
        /// <returns>TutorialPage2.</returns>
        public TutorialPage2 OpenTutorialPage2()
        {
            app.ScrollDownTo(TutorialPage2Btn, TutorialPage1ScrollView);
            app.Tap(TutorialPage2Btn);
            return new TutorialPage2();
        }
    }
}
