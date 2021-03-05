#nullable enable

using System;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class NewsPageViewModel : ViewModelBase
	{
		private readonly ILoggerService     _logger;
		private readonly INavigationService _ns;
		private          string?            _g_search;

		public string? GSearch
		{
			get => _g_search;
			set => this.SetProperty(ref _g_search, value);
		}

		public Command OnSearch => new Command(() => this.ShowPage(
			$"{AppResources.GoogleSearchUrl}+{Uri.EscapeDataString(_g_search ?? string.Empty)}"
		));
		public Command OnClick_ShowGoogle        => new Command(() => this.ShowPage(AppResources.GoogleSearchUrl));
		public Command OnClick_ShowCoronaGoJP    => new Command(() => this.ShowPage(AppResources.CoronaGoJPUrl));
		public Command OnClick_ShowStopCOVID19JP => new Command(() => this.ShowPage(AppResources.StopCOVID19JPUrl));
		public Command OnClick_ShowWikipedia     => new Command(() => this.ShowPage(AppResources.WikipediaUrl));

		public NewsPageViewModel(ILoggerService logger, INavigationService navigationService)
			// : base(navigationService) // ViewModelBase.NavigationService は不必要なプロパティなので利用しない。
		{
			_logger = logger            ?? throw new ArgumentNullException(nameof(logger));
			_ns     = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			this.Title = AppResources.NewsPageTitle;
		}

		private async void ShowPage(string url)
		{
			_logger.StartMethod();
			_logger.Info($"The URL: {url}");
			await _ns.NavigateAsync(nameof(WebViewerPage), new NavigationParameters() {
				{ "url", url }
			});
			_logger.EndMethod();
		}
	}
}
