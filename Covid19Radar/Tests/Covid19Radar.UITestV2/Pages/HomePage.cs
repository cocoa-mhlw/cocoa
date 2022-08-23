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
    /// HomePageクラス.
    /// </summary>
    public class HomePage : BasePage
    {
        /***********
         * ホームページ
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string HomePageTitle = "HomePageTitle";

        /// <summary>
        /// アプリ上部のツールバー.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ImageButtonRenderer = "ImageButtonRenderer";

        private readonly Query openMenuPage;
        private readonly Query backBtn;
        private readonly Query openNotContactPage;
        private readonly Query openSubmitConsentPage;
        private readonly Query openQuestionMark;
        private readonly Query openSubmitConsentPageENoff;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public HomePage()
        {
            if (OnAndroid)
            {
                // TODO:全ページクラスオブジェクトに定数を使用
                openMenuPage = x => x.Class(AppCompatImageButton).Index(0); // ハンバーガーメニュー
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
                openNotContactPage = x => x.Marked(HomePageTitle).Class(ButtonRenderer).Index(0); // 陽性者との接触結果を確認
                openSubmitConsentPage = x => x.Marked(HomePageTitle).Class(ButtonRenderer).Index(1); // 陽性情報の登録
                openSubmitConsentPageENoff = x => x.Marked(HomePageTitle).Class(ButtonRenderer).Index(2); // 陽性情報の登録(接触通知OFF)
                openQuestionMark = x => x.Marked(HomePageTitle).Class(ImageButtonRenderer).Index(0); // ?マーク
            }

            if (OniOS)
            {
                openMenuPage = x => x.Class(UIButton).Index(3); // ハンバーガーメニュー
                backBtn = x => x.Class(UIButton).Index(3); // 戻るボタン
                openNotContactPage = x => x.Marked(HomePageTitle).Class(UIButton).Index(1); // 陽性者との接触結果を確認
                openSubmitConsentPage = x => x.Marked(HomePageTitle).Class(UIButton).Index(2); // 陽性情報の登録
                openSubmitConsentPageENoff = x => x.Marked(HomePageTitle).Class(UIButton).Index(3); // 陽性情報の登録(接触通知OFF)
                openQuestionMark = x => x.Marked(HomePageTitle).Class(UIButton).Index(0); // ?マーク
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(HomePageTitle),
            iOS = x => x.Marked(HomePageTitle),
        };

        /// <summary>
        /// HomePageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertHomePage(TimeSpan? timeout = default(TimeSpan?))
        {
            if (OniOS)
            {
                (app as Xamarin.UITest.iOS.iOSApp).DismissSpringboardAlerts();
            }
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
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

        /// <summary>
        /// 戻るボタンをタップ.
        /// </summary>
        public void TapBackBtn()
        {
            app.Tap(backBtn);
        }

        /// <summary>
        /// 過去14日間の接触ページに遷移(接触通知ON時に使用).
        /// </summary>
        /// <returns>ExposureCheckPage.</returns>
        public ExposureCheckPage OpenExposureCheckPage()
        {
            app.Tap(openNotContactPage);
            return new ExposureCheckPage();
        }

        /// <summary>
        /// 陽性情報の登録に遷移(接触通知ON時に使用).
        /// </summary>
        /// <returns>SubmitConsentPage.</returns>
        public SubmitConsentPage OpenSubmitConsentPage()
        {
            app.Tap(openSubmitConsentPage);
            return new SubmitConsentPage();
        }

        /// <summary>
        /// 陽性情報の登録に遷移(接触通知OFF時に使用).
        /// </summary>
        /// <returns>SubmitConsentPage.</returns>
        public SubmitConsentPage OpenSubmitConsentPage_ENoff()
        {
            app.Tap(openSubmitConsentPageENoff);
            return new SubmitConsentPage();
        }

        /// <summary>
        /// ?ボタンを押下する.
        /// </summary>
        public void OpenQuestionMark()
        {
            app.Tap(openQuestionMark);
        }
    }
}
