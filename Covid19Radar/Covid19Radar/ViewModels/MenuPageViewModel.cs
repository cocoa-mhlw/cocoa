using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using Covid19Radar.Model;
using Covid19Radar.Views;

namespace Covid19Radar.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {
        public ObservableCollection<MainMenuModel> MenuItems { get; set; }

        private MainMenuModel selectedMenuItem;
        public MainMenuModel SelectedMenuItem
        {
            get => selectedMenuItem;
            set => SetProperty(ref selectedMenuItem, value);
        }

        public DelegateCommand NavigateCommand { get; private set; }

        public MenuPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            MenuItems = new ObservableCollection<MainMenuModel>();
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf965",
                PageName = nameof(HomePage),
                Title = Resources.AppResources.HomePageTitle
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf013",
                PageName = nameof(SettingsPage),
                Title = Resources.AppResources.SettingsPageTitle
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0e0",
                PageName = nameof(InqueryPage),
                Title = "お問い合わせ"
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf70e",
                PageName = nameof(TermsofservicePage),
                Title = "利用規約"
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(ThankYouNotifyOtherPage),
                Title = nameof(ThankYouNotifyOtherPage)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(NotContactPage),
                Title = nameof(NotContactPage)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(ContactedNotifyPage),
                Title = nameof(ContactedNotifyPage)
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(DebugPage),
                Title = nameof(DebugPage)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage1),
                Title = nameof(TutorialPage1)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage2),
                Title = nameof(TutorialPage2)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage3),
                Title = nameof(TutorialPage3)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage4),
                Title = nameof(TutorialPage4)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage5),
                Title = nameof(TutorialPage5)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage6),
                Title = nameof(TutorialPage6)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpMenuPage),
                Title = nameof(HelpMenuPage)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage1),
                Title = nameof(HelpPage1)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage2),
                Title = nameof(HelpPage2)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage3),
                Title = nameof(HelpPage3)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage4),
                Title = nameof(HelpPage4)
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(ChatbotPage),
                Title = nameof(ChatbotPage)
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(NotifyOtherPage),
                Title = nameof(NotifyOtherPage)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(SubmitConsentPage),
                Title = nameof(SubmitConsentPage)
            });

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            if (SelectedMenuItem.PageName == nameof(StartTutorialPage))
            {
                await NavigationService.NavigateAsync(nameof(StartTutorialPage), useModalNavigation: true);
                return;
            }
            await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
            return;
        }

    }
}
