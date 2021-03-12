#nullable enable

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	/// <summary>
	///  <see cref="Covid19Radar.Views.NewsPage"/>のイベントを実行します。
	/// </summary>
	public class NewsPageViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;
		private          string?        _g_search;

		/// <summary>
		///  Google 検索する内容を取得または設定します。
		/// </summary>
		public string? GSearch
		{
			get => _g_search;
			set => this.SetProperty(ref _g_search, value);
		}

		/// <summary>
		///  「最新情報をGoogleで検索」ボタンが押下された時に実行される処理を取得します。
		/// </summary>
		public virtual ICommand OnSearch => new Command(async () =>
			await this.ShowPage( $"{AppResources.GoogleSearchUrl}+{Uri.EscapeDataString(_g_search ?? string.Empty)}"
		));

		/// <summary>
		///  「内閣官房のデータ」ボタンが押下された時に実行される処理を取得します。
		/// </summary>
		public virtual ICommand OnClick_ShowCoronaGoJP => new Command(async () => await this.ShowPage(AppResources.CoronaGoJPUrl));

		/// <summary>
		///  「対策ダッシュボード」ボタンが押下された時に実行される処理を取得します。
		/// </summary>
		public virtual ICommand OnClick_ShowStopCOVID19JP => new Command(async () => await this.ShowPage(AppResources.StopCOVID19JPUrl));

		/// <summary>
		///  「ウィキペディアの情報」ボタンが押下された時に実行される処理を取得します。
		/// </summary>
		public virtual ICommand OnClick_ShowWikipedia => new Command(async () => await this.ShowPage(AppResources.WikipediaUrl));

		/// <summary>
		///  型'<see cref="Covid19Radar.ViewModels.NewsPageViewModel"/>'の新しいインスタンスを生成します。
		/// </summary>
		/// <param name="logger">ログの出力先を指定します。</param>
		public NewsPageViewModel(ILoggerService logger)
		{
			_logger    = logger ?? throw new ArgumentNullException(nameof(logger));
			this.Title = AppResources.NewsPageTitle;
		}

		/// <summary>
		///  指定されたWebサイトを表示します。
		/// </summary>
		/// <param name="url">Webサイトへの完全なアドレスです。<see langword="null"/>または空文字を指定する事はできません。</param>
		/// <returns>この処理の非同期操作です。</returns>
		protected async ValueTask ShowPage(string url)
		{
			_logger.StartMethod();
			if (string.IsNullOrEmpty(url)) {
				_logger.Warning("The URL was null or empty.");
				await UserDialogs.Instance.AlertAsync(AppResources.NewsPage_UrlWasEmpty);
			} else {
				_logger.Info($"The URL: {url}");
				await Browser.OpenAsync(url);
			}
			_logger.EndMethod();
		}
	}
}
