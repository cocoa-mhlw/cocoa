using System.Collections.ObjectModel;
using Covid19Radar.Model;
using Covid19Radar.Views;
using Prism.Commands;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class HelpMenuPageViewModel : ViewModelBase
    {
        public ObservableCollection<MainMenuModel> MenuItems { get; set; }

        private MainMenuModel selectedMenuItem;
        public MainMenuModel SelectedMenuItem
        {
            get => selectedMenuItem;
            set => SetProperty(ref selectedMenuItem, value);
        }

        public DelegateCommand NavigateCommand { get; private set; }

        public HelpMenuPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpMenuPageTitle;
            MenuItems = new ObservableCollection<MainMenuModel>();
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage1),
                Title = Resources.AppResources.HelpMenuPageLabel1
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage2),
                Title = Resources.AppResources.HelpMenuPageLabel2
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage3),
                Title = Resources.AppResources.HelpMenuPageLabel3
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage4),
                Title = Resources.AppResources.HelpMenuPageLabel4
            });

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            await NavigationService.NavigateAsync(SelectedMenuItem.PageName);
            SelectedMenuItem = null;
            return;
        }
    }
}
