#nullable enable

using System;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class WebViewerPageViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;
		private          string?        _url;

		public string? Url
		{
			get => _url;
			set => this.SetProperty(ref _url, value);
		}

		public WebViewerPageViewModel(ILoggerService logger)
		{
			_logger    = logger ?? throw new ArgumentNullException(nameof(logger));
			this.Title = AppResources.NewsPageTitle;
		}

		public override void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			if (parameters.TryGetValue("url", out object? url)) {
				this.Url = url?.ToString() ?? "about:newtab";
				_logger.Info($"The URL: {_url}");
			} else {
				this.Url = "about:newtab";
				_logger.Warning("The URL was not found on parameters.");
			}
			_logger.EndMethod();
		}
	}
}
