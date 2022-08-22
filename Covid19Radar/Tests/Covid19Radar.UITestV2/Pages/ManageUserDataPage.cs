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
    /// DebugPageクラス.
    /// </summary>
    public class ManageUserDataPage : BasePage
    {
        /***********
         * デバッグページ
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string ManageUserDataPageTitle = "ManageUserDataPageTitle";

        /// <summary>
        /// ManageExposureDataPageLowボタン.
        /// </summary>
        private static readonly string ManageUserDataPage1day = "ManageUserDataPage1day";

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public ManageUserDataPage()
        {
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(ManageUserDataPageTitle),
            iOS = x => x.Marked(ManageUserDataPageTitle),
        };

        /// <summary>
        /// DebugPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertManageUserDataPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// ManageExposureDataPageLowをTapする.
        /// </summary>
        public void TapManageUserDataPage1day()
        {
            app.Tap(ManageUserDataPage1day);
        }
    }
}
