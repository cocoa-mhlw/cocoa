using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// SendLogCompletePageクラス.
    /// </summary>
    public class SendLogCompletePage : BasePage
    {
        /***********
         * 動作情報の送信
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string SendLogCompletePageTitle = "SendLogCompletePageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSのボタン.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// Androidラベル.
        /// </summary>
        private static readonly string LabelRenderer = "LabelRenderer";

        private readonly Query openMail;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public SendLogCompletePage()
        {
            if (OnAndroid)
            {
                openMail = x => x.Marked(SendLogCompletePageTitle).Class(ButtonRenderer).Index(0); // メールで動作情報IDを送信する
            }

            if (OniOS)
            {
                openMail = x => x.Class(UIButton).Index(0); // メールで動作情報IDを送信する
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(SendLogCompletePageTitle),
            iOS = x => x.Marked(SendLogCompletePageTitle),
        };

        /// <summary>
        /// SendLogCompletePageのアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertSendLogCompletePage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// メールアプリを開く.
        /// </summary>
        public void OpenMail()
        {
            app.Tap(openMail);
        }
    }
}
