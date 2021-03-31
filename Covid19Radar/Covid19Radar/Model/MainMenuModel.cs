/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;
using Xamarin.Forms;

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
