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
    /// WebAccessibilityPolicyPageクラス.
    /// </summary>
    public class WebAccessibilityPolicyPage : BasePage
    {
        /***********
         * ウェブアクセシビリティ方針
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string WebAccessibilityPolicyPageTitle = "WebAccessibilityPolicyPageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// toolbar.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        private readonly Query backBtn;
        private readonly Query openMenuPage;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public WebAccessibilityPolicyPage()
        {
            if (OnAndroid)
            {
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
                openMenuPage = x => x.Class(AppCompatImageButton).Index(0); // ハンバーガーメニュー
            }

            if (OniOS)
            {
                backBtn = x => x.Class(UIButton).Index(0); // 戻るボタン
                openMenuPage = x => x.Class(UIButton).Index(0); // ハンバーガーメニュー
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(WebAccessibilityPolicyPageTitle),
            iOS = x => x.Marked(WebAccessibilityPolicyPageTitle),
        };

        /// <summary>
        /// WebAccessibilityPolicyPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertWebAccessibilityPolicyPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// 戻るボタンを押下.
        /// </summary>
        public void TapBackBtn()
        {
            app.Tap(backBtn);
        }

        /// <summary>
        /// MenuPageに遷移.
        /// </summary>
        /// <returns>MenuPage.</returns>
        public MenuPage OpenMenuPage()
        {
            app.Tap(openMenuPage);
            return new MenuPage();
        }
    }
}
