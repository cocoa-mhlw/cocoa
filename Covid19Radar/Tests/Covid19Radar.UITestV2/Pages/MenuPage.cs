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
    /// MenuPageページクラス.
    /// </summary>
    public class MenuPage : BasePage
    {
        /***********
         * ハンバーガーメニュー
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string MasterDetailPageTitle = "MasterDetailPageTitle";

        /// <summary>
        /// Androidメニューの行.
        /// </summary>
        private static readonly string ViewCellRendererViewCellContainer = "ViewCellRenderer_ViewCellContainer";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string ImageButtonRenderer = "ImageButtonRenderer";

        /// <summary>
        /// iOSメニューの行.
        /// </summary>
        private static readonly string UITableViewCell = "UITableViewCell";

        /// <summary>
        /// iOSボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openHomePage;
        private readonly Query openSettingsPage;
        private readonly Query openInqueryPage;
        private readonly Query openTermsofservicePage;
        private readonly Query openTermsofservicePageFromHelpPage;
        private readonly Query openPrivacyPolicyPage2;
        private readonly Query openWebAccessibilityPolicyPage;
        private readonly Query openDebugPage;
        private readonly Query backBtn;
        private readonly Query openHelpMenuPage;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public MenuPage()
        {
            if (OnAndroid)
            {
                openHomePage = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(0); // ホーム
                openSettingsPage = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(1); // 設定
                openInqueryPage = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(2); // お問い合わせ
                openHelpMenuPage = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(3); // 使い方
                openTermsofservicePage = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(4); // 利用規約
                openTermsofservicePageFromHelpPage = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(4); // 利用規約 (使い方ページから)
                openPrivacyPolicyPage2 = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(5); // プライバシーポリシー
                openWebAccessibilityPolicyPage = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(6); // ウェブアクセシビリティ方針
                openDebugPage = x => x.Marked(MasterDetailPageTitle).Class(ViewCellRendererViewCellContainer).Index(4); // Debugメニュー
                backBtn = x => x.Marked(MasterDetailPageTitle).Class(ImageButtonRenderer).Index(0); // 戻るボタン
            }

            if (OniOS)
            {
                openHomePage = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(0); // ホーム
                openSettingsPage = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(1); // 設定
                openInqueryPage = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(2); // お問い合わせ
                openHelpMenuPage = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(3); // 使い方
                openTermsofservicePage = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(4); // 利用規約
                openTermsofservicePageFromHelpPage = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(4); // 利用規約 (使い方ページから)
                openPrivacyPolicyPage2 = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(5); // プライバシーポリシー
                openWebAccessibilityPolicyPage = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(6); // ウェブアクセシビリティ方針
                openDebugPage = x => x.Marked(MasterDetailPageTitle).Class(UITableViewCell).Index(4); // Debugメニュー
                backBtn = x => x.Marked(MasterDetailPageTitle).Class(UIButton).Index(0); // 戻るボタン
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(MasterDetailPageTitle),
            iOS = x => x.Marked(MasterDetailPageTitle),
        };

        /// <summary>
        /// MenuPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertMenuPage(TimeSpan? timeout = default(TimeSpan?))
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
            return new HomePage();
        }

        /// <summary>
        /// SettingsPageに遷移する.
        /// </summary>
        /// <returns>SettingsPage.</returns>
        public SettingsPage OpenSettingsPage()
        {
            app.Tap(openSettingsPage);
            return new SettingsPage();
        }

        /// <summary>
        /// InqueryPageに遷移する.
        /// </summary>
        /// <returns>InqueryPage.</returns>
        public InqueryPage OpenInqueryPage()
        {
            app.Tap(openInqueryPage);
            return new InqueryPage();
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

        /// <summary>
        /// TermsofservicePageに遷移する.
        /// </summary>
        /// <returns>TermsofservicePage.</returns>
        public TermsofservicePage OpenTermsofservicePage()
        {
            app.Tap(openTermsofservicePage);
            return new TermsofservicePage();
        }

        /// <summary>
        /// TermsofservicePageFromHelpPageに遷移する.
        /// </summary>
        /// <returns>TermsofservicePageFromHelpPage.</returns>
        public TermsofservicePage OpenTermsofservicePageFromHelpPage()
        {
            app.Tap(openTermsofservicePageFromHelpPage);
            return new TermsofservicePage();
        }

        /// <summary>
        /// PrivacyPolicyPage2に遷移する.
        /// </summary>
        /// <returns>PrivacyPolicyPage2.</returns>
        public PrivacyPolicyPage2 OpenPrivacyPolicyPage2()
        {
            app.Tap(openPrivacyPolicyPage2);
            return new PrivacyPolicyPage2();
        }

        /// <summary>
        /// WebAccessibilityPolicyPageに遷移する.
        /// </summary>
        /// <returns>WebAccessibilityPolicyPage.</returns>
        public WebAccessibilityPolicyPage OpenWebAccessibilityPolicyPage()
        {
            app.Tap(openWebAccessibilityPolicyPage);
            return new WebAccessibilityPolicyPage();
        }

        /// <summary>
        /// DebugPageに遷移する.
        /// </summary>
        /// <returns>DebugPage.</returns>
        public DebugPage OpenDebugPage()
        {
            app.Tap(openDebugPage);
            return new DebugPage();
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
