using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppHomePageMaster : ContentPage
    {
        public ListView ListView;

        public AppHomePageMaster()
        {
            InitializeComponent();

            BindingContext = new AppHomePageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class AppHomePageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<AppHomePageMasterMenuItem> MenuItems { get; set; }

            public AppHomePageMasterViewModel()
            {
                MenuItems = new ObservableCollection<AppHomePageMasterMenuItem>(new[]
                {
                    new AppHomePageMasterMenuItem { Id = 0, Title = "Page 1" },
                    new AppHomePageMasterMenuItem { Id = 1, Title = "Page 2" },
                    new AppHomePageMasterMenuItem { Id = 2, Title = "Page 3" },
                    new AppHomePageMasterMenuItem { Id = 3, Title = "Page 4" },
                    new AppHomePageMasterMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}