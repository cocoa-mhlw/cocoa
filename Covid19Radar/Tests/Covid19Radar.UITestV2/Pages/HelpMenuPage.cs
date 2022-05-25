using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// HelpMenuPageクラス.
    /// </summary>
    public class HelpMenuPage : BasePage
    {
        /***********
         * 使い方メニュー
        ***********/

        /// <summary>
        /// PageAutomationID.
        /// </summary>
        private static readonly string HelpMenuPageTitle = "HelpMenuPageTitle";

        /// <summary>
        /// Android Label.
        /// </summary>
        private static readonly string LabelRenderer = "LabelRenderer";

        /// <summary>
        /// AndroidImageボタン.
        /// </summary>
        private static readonly string AppCompatImageButton = "AppCompatImageButton";

        /// <summary>
        /// アプリ上部のツールバー.
        /// </summary>
        private static readonly string Toolbar = "toolbar";

        /// <summary>
        /// iOSのラベル.
        /// </summary>
        private static readonly string UILabel = "UILabel";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        private readonly Query openHelpPage1;
        private readonly Query openHelpPage2;
        private readonly Query openHelpPage3;
        private readonly Query openHelpPage4;
        private readonly Query backBtn;
        private readonly Query openMenuPage;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public HelpMenuPage()
        {
            if (OnAndroid)
            {
                openHelpPage1 = x => x.Marked(HelpMenuPageTitle).Class(LabelRenderer).Index(0); // どのようにして接触を記録していますか？
                openHelpPage2 = x => x.Marked(HelpMenuPageTitle).Class(LabelRenderer).Index(1); // 接触の有無はどのように知ることができますか？
                openHelpPage3 = x => x.Marked(HelpMenuPageTitle).Class(LabelRenderer).Index(2); // 新型コロナウイルスに感染していると判定されたら
                openHelpPage4 = x => x.Marked(HelpMenuPageTitle).Class(LabelRenderer).Index(3); // 接触の記録を停止/情報を削除するには
                backBtn = x => x.Id(Toolbar).Class(AppCompatImageButton).Index(0); // 戻るボタン
                openMenuPage = x => x.Class(AppCompatImageButton).Index(0); // ハンバーガーメニュー
            }

            if (OniOS)
            {
                openHelpPage1 = x => x.Marked(HelpMenuPageTitle).Class(UILabel).Index(0); // どのようにして接触を記録していますか？
                openHelpPage2 = x => x.Marked(HelpMenuPageTitle).Class(UILabel).Index(1); // 接触の有無はどのように知ることができますか？
                openHelpPage3 = x => x.Marked(HelpMenuPageTitle).Class(UILabel).Index(2); // 新型コロナウイルスに感染していると判定されたら
                openHelpPage4 = x => x.Marked(HelpMenuPageTitle).Class(UILabel).Index(3); // 接触の記録を停止/情報を削除するには
                backBtn = x => x.Class(UIButton).Index(0); // 戻るボタン
                openMenuPage = x => x.Class(UIButton).Index(0); // ハンバーガーメニュー
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(HelpMenuPageTitle),
            iOS = x => x.Marked(HelpMenuPageTitle),
        };

        /// <summary>
        /// HelpMenuPageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertHelpMenuPage(TimeSpan? timeout = default(TimeSpan?))
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
        /// MenuPage(ハンバーガーメニュー)を開く.
        /// </summary>
        /// <returns>MenuPage.</returns>
        public MenuPage OpenMenuPage()
        {
            app.Tap(openMenuPage);
            return new MenuPage();
        }

        /// <summary>
        /// HelpPage1に遷移する.
        /// </summary>
        /// <returns>HelpPage1.</returns>
        public HelpPage1 OpenHelpPage1()
        {
            app.Tap(openHelpPage1);
            return new HelpPage1();
        }

        /// <summary>
        /// HelpPage2に遷移する.
        /// </summary>
        /// <returns>HelpPage2.</returns>
        public HelpPage2 OpenHelpPage2()
        {
            app.Tap(openHelpPage2);
            return new HelpPage2();
        }

        /// <summary>
        /// HelpPage3に遷移する.
        /// </summary>
        /// <returns>HelpPage3.</returns>
        public HelpPage3 OpenHelpPage3()
        {
            app.Tap(openHelpPage3);
            return new HelpPage3();
        }

        /// <summary>
        /// HelpPage4に遷移する.
        /// </summary>
        /// <returns>HelpPage4.</returns>
        public HelpPage4 OpenHelpPage4()
        {
            app.Tap(openHelpPage4);
            return new HelpPage4();
        }
    }
}
