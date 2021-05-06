/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;
using Xamarin.Forms;
using SysColor = System.Drawing.Color;

namespace Covid19Radar.ViewModels
{
    /// <summary>
    ///  <see cref="Covid19Radar.Views.NewsPage"/>のイベントを実行します。
    /// </summary>
    public class NewsPageViewModel : ViewModelBase
    {
        private static readonly Uri                  CoronaGoJPUrl    = new Uri(AppResources.CoronaGoJPUrl);
        private static readonly Uri                  StopCOVID19JPUrl = new Uri(AppResources.StopCOVID19JPUrl);
        private static readonly Uri                  WikipediaUrl     = new Uri(AppResources.WikipediaUrl);
        private        readonly ILoggerService       _logger;
        private                 string?              _g_search;
        private                 BrowserLaunchOptions _blo;

        /// <summary>
        ///  Google 検索する内容を取得または設定します。
        /// </summary>
        public string? GSearch
        {
            get => _g_search;
            set => this.SetProperty(ref _g_search, value);
        }

        /// <summary>
        ///  ブラウザの起動設定を取得または設定します。
        /// </summary>
        public BrowserLaunchOptions BrowserLaunchOptions
        {
            get => _blo;
            set => this.SetProperty(ref _blo, value);
        }

        /// <summary>
        ///  「最新情報をGoogleで検索」ボタンが押下された時に実行される処理を取得します。
        /// </summary>
        public virtual ICommand OnSearch { get; }

        /// <summary>
        ///  「内閣官房のデータ」ボタンが押下された時に実行される処理を取得します。
        /// </summary>
        public virtual ICommand OnClick_ShowCoronaGoJP { get; }

        /// <summary>
        ///  「対策ダッシュボード」ボタンが押下された時に実行される処理を取得します。
        /// </summary>
        public virtual ICommand OnClick_ShowStopCOVID19JP { get; }

        /// <summary>
        ///  「ウィキペディアの情報」ボタンが押下された時に実行される処理を取得します。
        /// </summary>
        public virtual ICommand OnClick_ShowWikipedia { get; }

        /// <summary>
        ///  型'<see cref="Covid19Radar.ViewModels.NewsPageViewModel"/>'の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="logger">ログの出力先を指定します。</param>
        public NewsPageViewModel(ILoggerService logger)
        {
            _logger    = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Title = AppResources.NewsPageTitle;

            _blo = new BrowserLaunchOptions() {
                LaunchMode            = BrowserLaunchMode.External,
                Flags                 = BrowserLaunchFlags.None,
                TitleMode             = BrowserTitleMode.Show,
                PreferredControlColor = SysColor.FromArgb(0xCC, 0xCC, 0xCC, 0xCC),
                PreferredToolbarColor = SysColor.FromArgb(0x44, 0x44, 0x44, 0x44)
            };

            this.OnSearch = new Command(async () => {
                // await this.ShowPage($"{AppResources.GoogleSearchUrl}+{Uri.EscapeDataString(_g_search ?? string.Empty)}");
                string url  = AppResources.GoogleSearchUrl;
                string text = Uri.EscapeDataString(_g_search ?? string.Empty);
                await this.ShowPage(new Uri(string.Create((url.Length + text.Length + 1), (url, text), (span, arg) => {
                    arg.url.AsSpan().CopyTo(span);
                    span = span.Slice(arg.url.Length);
                    span[0] = '+';
                    arg.text.AsSpan().CopyTo(span.Slice(1));
                })));
            });

            this.OnClick_ShowCoronaGoJP    = new Command(async () => await this.ShowPage(CoronaGoJPUrl));
            this.OnClick_ShowStopCOVID19JP = new Command(async () => await this.ShowPage(StopCOVID19JPUrl));
            this.OnClick_ShowWikipedia     = new Command(async () => await this.ShowPage(WikipediaUrl));
        }

        /// <summary>
        ///  指定されたWebサイトを表示します。
        /// </summary>
        /// <param name="url">Webサイトへの完全なアドレスです。<see langword="null"/>または空文字を指定する事はできません。</param>
        /// <returns>この処理の非同期操作です。</returns>
        protected async ValueTask ShowPage(Uri url)
        {
            _logger.StartMethod();
            if (url is null) {
                _logger.Warning("The URL was null or empty.");
                await UserDialogs.Instance.AlertAsync(AppResources.NewsPage_UrlWasEmpty);
            } else {
                _logger.Info($"The URL: {url}");
                _logger.Debug($"The launch mode: {_blo.LaunchMode}");
                _logger.Debug($"The flags: {_blo.Flags}");
                _logger.Debug($"The title mode: {_blo.TitleMode}");
                _logger.Debug($"The preferred control color: {_blo.PreferredControlColor}");
                _logger.Debug($"The preferred toolbar color: {_blo.PreferredToolbarColor}");
                await Browser.OpenAsync(url, _blo);
            }
            _logger.EndMethod();
        }
    }
}
