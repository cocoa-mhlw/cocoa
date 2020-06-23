using System.ComponentModel;

namespace Covid19Radar.Model
{
    public class MainMenuModel : INotifyPropertyChanged
    {
        private string iconColor;
        private string textColor;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }
        public string Icon { get; set; }
        public string PageName { get; set; }
        public string IconColor
        {
            set
            {
                if (iconColor != value)
                {
                    iconColor = value;
                    OnPropertyChanged("IconColor");
                }
            }
            get
            {
                return iconColor;
            }
        }
        public string TextColor
        {
            set
            {
                if (textColor != value)
                {
                    textColor = value;
                    OnPropertyChanged("TextColor");
                }
            }
            get
            {
                return textColor;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
