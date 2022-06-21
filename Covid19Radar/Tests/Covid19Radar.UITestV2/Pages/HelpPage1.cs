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
    /// HelpPage1クラス.
    /// </summary>
    public class HelpPage1 : BasePage
    {
        /***********
         * どのようにして接触を記録していますか？
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string HelpPage1Title = "HelpPage1Title";

        /// <summary>
        /// アプリ上部のツールバー.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        /// <summary>
        /// Androidのボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query backBtn;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public HelpPage1()
        {
            if (OnAndroid)
            {
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
            }

            if (OniOS)
            {
                backBtn = x => x.Class(UIButton).Index(0); // 戻るボタン
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(HelpPage1Title),
            iOS = x => x.Marked(HelpPage1Title),
        };

        /// <summary>
        /// HelpPage1のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertHelpPage1(TimeSpan? timeout = default(TimeSpan?))
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
    }
}
