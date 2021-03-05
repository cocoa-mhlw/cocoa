#nullable enable

using System;
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class NewsPageViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;
		private          string?        _g_search;

		public string? GSearch
		{
			get => _g_search;
			set => this.SetProperty(ref _g_search, value);
		}

		public Command OnSearch => new Command(() => this.ShowPage(
			$"{AppResources.GoogleSearchUrl}+{Uri.EscapeDataString(_g_search ?? string.Empty)}"
		));
		public Command OnClick_ShowCoronaGoJP    => new Command(() => this.ShowPage(AppResources.CoronaGoJPUrl));
		public Command OnClick_ShowStopCOVID19JP => new Command(() => this.ShowPage(AppResources.StopCOVID19JPUrl));
		public Command OnClick_ShowWikipedia     => new Command(() => this.ShowPage(AppResources.WikipediaUrl));

		public NewsPageViewModel(ILoggerService logger)
		{
			_logger    = logger ?? throw new ArgumentNullException(nameof(logger));
			this.Title = AppResources.NewsPageTitle;
		}

		private async void ShowPage(string url)
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
