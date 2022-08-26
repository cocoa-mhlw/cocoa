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
    /// NotContactPageクラス.
    /// </summary>
    public class NotContactPage : BasePage
    {
        /***********
         * 過去14日間の接触
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string NotContactPageTitle = "NotContactPageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openIntroducePopup;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public NotContactPage()
        {
            if (OnAndroid)
            {
                openIntroducePopup = x => x.Marked(NotContactPageTitle).Class(ButtonRenderer).Index(0); // 処理番号の取得方法
            }

            if (OniOS)
            {
                openIntroducePopup = x => x.Marked(NotContactPageTitle).Class(UIButton).Index(0); // 処理番号の取得方法
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(NotContactPageTitle),
            iOS = x => x.Marked(NotContactPageTitle),
        };

        /// <summary>
        /// NotContactPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertNotContactPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// 「処理番号の取得方法」リンクを押下する.
        /// </summary>
        public void OpenIntroducePopup()
        {
            app.Tap(openIntroducePopup);
        }
    }
}
