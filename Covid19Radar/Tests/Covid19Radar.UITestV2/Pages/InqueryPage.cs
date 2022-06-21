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
    /// InqueryPageクラス.
    /// </summary>
    public class InqueryPage : BasePage
    {
        /***********
         * アプリに関するお問い合わせ
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string InqueryPageTitle = "InqueryPageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// 接触確認アプリに関する情報リンクのAutomationID.
        /// </summary>
        private static readonly string AppImfoLink = "InqueryPageTitleOpenLink";

        private readonly Query opensendLogConfirmationPage;
        private readonly Query openMenuPage;
        private readonly Query openFAQBtn;
        private readonly Query openMail;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public InqueryPage()
        {
            if (OnAndroid)
            {
                opensendLogConfirmationPage = x => x.Marked(InqueryPageTitle).Class(ButtonRenderer).Index(2); // 動作状況を送信ボタン
                openMenuPage = x => x.Class(AppCompatImageButton).Index(0); // ハンバーガーメニュー
                openFAQBtn = x => x.Marked(InqueryPageTitle).Class(ButtonRenderer).Index(0); // よくある質問ボタン
                openMail = x => x.Marked(InqueryPageTitle).Class(ButtonRenderer).Index(1); // よくある質問ボタン
            }

            if (OniOS)
            {
                opensendLogConfirmationPage = x => x.Marked(InqueryPageTitle).Class(UIButton).Index(2); // 動作状況を送信ボタン
                openMenuPage = x => x.Class(UIButton).Index(3); // ハンバーガーメニュー
                openFAQBtn = x => x.Marked(InqueryPageTitle).Class(UIButton).Index(0); // よくある質問ボタン
                openMail = x => x.Marked(InqueryPageTitle).Class(UIButton).Index(1); // よくある質問ボタン
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(InqueryPageTitle),
            iOS = x => x.Marked(InqueryPageTitle),
        };

        /// <summary>
        /// InqueryPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertInqueryPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// SendLogConfirmationPageに遷移する.
        /// </summary>
        /// <returns>SendLogConfirmationPage.</returns>
        public SendLogConfirmationPage OpenSendLogConfirmationPage()
        {
            app.Tap(opensendLogConfirmationPage);
            return new SendLogConfirmationPage();
        }

        /// <summary>
        /// 「よくある質問」リンクを押下する.
        /// </summary>
        public void TapOpenFAQBtn()
        {
            app.Tap(openFAQBtn);
        }

        /// <summary>
        /// 「接触確認アプリに関する情報」リンクを押下する.
        /// </summary>
        public void TapAppImfoLink()
        {
            app.Tap(AppImfoLink);
        }

        /// <summary>
        /// 「メールでお問い合わせ」ボタンを押下する.
        /// </summary>
        public void OpenMail()
        {
            app.Tap(openMail);
        }

        /// <summary>
        /// MenuPage(ハンバーガーメニュー)を開く.
        /// </summary>
        /// <returns>MenuPage.</returns>
        public MenuPage OpenMenuPage()
        {
            app.Tap(openMenuPage);
            return new MenuPage();
        }
    }
}
