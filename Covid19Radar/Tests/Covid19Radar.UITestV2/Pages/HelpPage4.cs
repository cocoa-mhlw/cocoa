using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// HelpPage4クラス.
    /// </summary>
    public class HelpPage4 : BasePage
    {
        /***********
         * 接触の記録を停止/情報を削除するには
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string HelpPage4Title = "HelpPage4Title";

        /// <summary>
        /// アプリ上部のツールバー.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query backBtn;
        private readonly Query openSettingsPage;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public HelpPage4()
        {
            if (OnAndroid)
            {
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
                openSettingsPage = x => x.Marked(HelpPage4Title).Class(ButtonRenderer).Index(0); // 「アプリの設定へ」ボタン
            }

            if (OniOS)
            {
                backBtn = x => x.Class(UIButton).Index(1); // 戻るボタン
                openSettingsPage = x => x.Marked(HelpPage4Title).Class(UIButton).Index(0); // 「アプリの設定へ」ボタン
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(HelpPage4Title),
            iOS = x => x.Marked(HelpPage4Title),
        };

        /// <summary>
        /// HelpPage4のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertHelpPage4(TimeSpan? timeout = default(TimeSpan?))
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

        /// <summary>
        /// SettingPageに遷移する.
        /// </summary>
        /// <returns>SettingPage.</returns>
        public SettingsPage OpenSettingsPage()
        {
            app.Tap(openSettingsPage);
            return new SettingsPage();
        }
    }
}
