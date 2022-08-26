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
    /// TutorialPage6クラス.
    /// </summary>
    public class EventLogSettingPage : BasePage
    {
        /***********
         * チュートリアルページ_6
        ***********/

        /// <summary>
        /// AutomationID.
        /// </summary>
        private static readonly string EventLogSettingPageTitle = "EventLogSettingPageTitle";

        /// <summary>
        /// Androidボタン.
        /// </summary>
        private static readonly string ButtonRenderer = "ButtonRenderer";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string UIButton = "UIButton";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string EventLogSettingPageCheckBox = "EventLogSettingPageCheckBox";

        /// <summary>
        /// iOSラベル.
        /// </summary>
        private static readonly string EventLogSettingPageBtn = "EventLogSettingPageBtn";

        private readonly Query openTutorialPage6;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public EventLogSettingPage()
        {
            if (OnAndroid)
            {
                openTutorialPage6 = x => x.Marked(EventLogSettingPageTitle).Class(ButtonRenderer).Index(0);
            }

            if (OniOS)
            {
                openTutorialPage6 = x => x.Marked(EventLogSettingPageTitle).Class(UIButton).Index(0);
            }
        }

        /// <summary>
        /// ページオブジェクトクエリ.
        /// </summary>
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(EventLogSettingPageTitle),
            iOS = x => x.Marked(EventLogSettingPageTitle),
        };

        /// <summary>
        /// TutorialPage6のアサーション.
        /// </summary>
        /// <param name="timeout">タイムアウト値.</param>
        public void AssertEventLogSettingPage(TimeSpan? timeout = default(TimeSpan?))
        {
            app.Screenshot(this.GetType().Name.ToString());
            AssertOnPage(timeout);
        }

        /// <summary>
        /// [設定を保存]を押下しTutorial6に遷移する.
        /// </summary>
        public void OpenTutorialPage6()
        {
            app.Tap(EventLogSettingPageBtn);
        }

        /// <summary>
        /// [接触通知の表示]チェックボックスをチェック
        /// </summary>
        public void TapCheckBox()
        {
            app.Tap(EventLogSettingPageCheckBox);
        }

        /// <summary>
        /// 登録をキャンセルしましたダイアログにおいて「OK」ボタンを押下する.
        /// </summary>
        /// <returns>TutorialPage6Page.</returns>
        public TutorialPage6 TaplDialogOKBtn()
        {
            app.Tap("OK");
            return new TutorialPage6();
        }
    }
}
