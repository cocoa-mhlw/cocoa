/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Configuration;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// AppManagerクラス.
    /// </summary>
    internal static class AppManager
    {
        /// <summary>
        /// アプリケーションファイルの相対パス.
        /// </summary>
        public const string AppPath = "../../../../Covid19Radar.iOS/bin/iPhoneSimulator/Release/Covid19Radar.iOS.app";
        private static readonly string ApkPath = GetPath();
        private static IApp app;
        private static Platform? platform;

        /// <summary>
        /// Platformクエリ.
        /// </summary>
        public static Platform Platform
        {
            get
            {
                if (platform == null)
                {
                    throw new NullReferenceException("'AppManager.Platform' not set.");
                }

                return platform.Value;
            }

            set
            {
                platform = value;
            }
        }

        /// <summary>
        /// アプリケーションのエントリーポイント.
        /// </summary>
        public static IApp App
        {
            get
            {
                if (app == null)
                {
                    throw new NullReferenceException("'AppManager.App' not set. Call 'AppManager.StartApp()' before trying to access it.");
                }

                return app;
            }
        }

        /// <summary>
        /// ローカル環境での実行時にアプリのパスを指定.
        /// </summary>
        /// <returns>アプリのパス.</returns>
        public static string GetPath()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6);
            path = path.Replace("Covid19Radar\\Tests\\Covid19Radar.UITestV2\\bin\\Debug_UITest", "precompiledApps");
            path = path + "/APP_PACKAGE_NAME.APP_PACKAGE_NAME.apk";
            return path;
        }

        /// <summary>
        /// アプリを起動する.
        /// </summary>
        public static void StartApp()
        {
            if (Platform == Platform.Android)
            {
                app = ConfigureApp
                    .Android
                    .ApkFile(ApkPath)
                    .StartApp();
            }

            if (Platform == Platform.iOS)
            {
                app = ConfigureApp
                    .iOS
                    .AppBundle(AppPath)
                    .StartApp();
            }
        }

        /// <summary>
        /// アプリを再起動する.
        /// </summary>
        public static void ReStartApp()
        {
            if (Platform == Platform.Android)
            {
                ConfigureApp.Android.ApkFile(ApkPath).StartApp(AppDataMode.DoNotClear);
            }

            if (Platform == Platform.iOS)
            {
                ConfigureApp.iOS.AppBundle(AppPath).StartApp(AppDataMode.DoNotClear);
            }
        }

        /// <summary>
        /// アプリを再起動する(スプラッシュ画面無し).
        /// </summary>
        public static void ConnectToApp()
        {
            if (Platform == Platform.Android)
            {
                app.Invoke("ConnectToApp");
            }

            if (Platform == Platform.iOS)
            {
                ConfigureApp.iOS.AppBundle(AppPath).StartApp(AppDataMode.DoNotClear);
            }
        }


        /// <summary>
        /// アプリを再起動する(スプラッシュ画面無し).
        /// </summary>
        public static void MoveTaskToBack()
        {
            if (Platform == Platform.Android)
            {
                app.Invoke("MoveTaskToBack");
            }

        }


        /// <summary>
        /// アプリを再起動する(スプラッシュ画面無し).
        /// </summary>
        public static void FinishAndRemoveTask()
        {
            if (Platform == Platform.Android)
            {
                app.Invoke("FinishAndRemoveTask");
            }

            if (Platform == Platform.iOS)
            {
                app.Invoke("FinishAndRemoveTask:", "UITest");
            }

        }


        

        /// <summary>
        /// jsonから任意の値を取得する.
        /// </summary>
        /// <param name="lang">言語.</param>
        /// <param name="value">jsonファイル中の要素を指定するためのキー.</param>
        /// <returns>jsonファイル中の"value"に対応する値.</returns>
        public static JToken Comparison(string lang, string value)
        {
            StreamReader fileName = new StreamReader(lang + ".json");
            string allLine = fileName.ReadToEnd();
            JObject jsonObj = JObject.Parse(allLine);
            return jsonObj[value]["value"];
        }

        /// <summary>
        /// 端末言語取得.
        /// </summary>
        /// <returns>言語コード.</returns>
        public static string GetCurrentCultureBackDoor()
        {
            string cultureText = "en-US";
            if (Platform == Platform.Android)
            {
                cultureText = app.Invoke("GetCurrentCulture").ToString();
            }

            if (Platform == Platform.iOS)
            {
                cultureText = app.Invoke("GetCurrentCulture:", "UITest").ToString();
            }

            if (cultureText == "en")
            {
                cultureText = "en-US";
            }
            else if (cultureText == "ja")
            {
                cultureText = "ja-JP";
            }
            else if (cultureText == "ko")
            {
                cultureText = "en-US";
            }
            else if (cultureText == "ko-KR")
            {
                cultureText = "en-US";
            }

            return cultureText;
        }

        /// <summary>
        /// webビュー内のページのタイトルを取得する.
        /// </summary>
        /// <returns>タイトルテキスト.</returns>
        public static string GetTitleText()
        {
            var title = string.Empty;

            if (Platform == Platform.Android)
            {
                title = app.Query(x => x.Css("h1"))[0].TextContent;
            }

            if (Platform == Platform.iOS)
            {
                title = app.Query(c => c.Class("WKWebView").Css("H1"))[0].TextContent;
            }

            return title;
        }

        /// <summary>
        /// 陽性登録画面でOSごとに文字列比較の分岐を行う.
        /// </summary>
        /// <param name="cultureText">使用する言語.</param>
        /// <returns>ダイアログの文字列.</returns>
        public static string RegistResultBranch(string cultureText)
        {
            string comparisonText = string.Empty;
            if (Platform == Platform.Android)
            {
                // センターに接続できません
                comparisonText = (string)AppManager.Comparison(cultureText, "ExposureNotificationHandler2ErrorMessage");
            }

            if (Platform == Platform.iOS)
            {
                // 登録が完了しました
                comparisonText = (string)AppManager.Comparison(cultureText, "NotifyOtherPageDialogSubmittedTitle");
            }

            return comparisonText;
        }

        /// <summary>
        /// 陽性登録画面でOSごとに文字列比較の分岐を行う.
        /// </summary>
        /// <param name="cultureText">使用する言語.</param>
        /// <returns>ダイアログの文字列.</returns>
        public static string RegistResultBranch2(string cultureText)
        {
            string comparisonText = string.Empty;
            if (Platform == Platform.Android)
            {
                // センターに接続できません
                comparisonText = (string)AppManager.Comparison(cultureText, "ExposureNotificationHandler2ErrorMessage");
            }

            if (Platform == Platform.iOS)
            {
                // 処理番号が誤っているか、有効期限が切れています
                comparisonText = (string)AppManager.Comparison(cultureText, "ExposureNotificationHandler1ErrorMessage");
            }

            return comparisonText;
        }
    }
}
