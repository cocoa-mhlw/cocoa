using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Shiny.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible, IDialogService
    {

        // ナビゲーション
        protected INavigationService NavigationService { get; private set; }
        // Shiny接続
        protected IConnectivity ConnectivityService { get; private set; }

        protected IDialogService DialogService { get; private set; }
        // ページタイトル
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
        public ViewModelBase(INavigationService navigationService, IConnectivity connectivityService)
        {
            NavigationService = navigationService;
            ConnectivityService = connectivityService;
        }
        public ViewModelBase(INavigationService navigationService, IConnectivity connectivityService, IDialogService dialogService)
        {
            NavigationService = navigationService;
            ConnectivityService = connectivityService;
            DialogService = dialogService;
        }
        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            throw new NotImplementedException();
        }
    }
}
