using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.ViewModels
{
    public class ExceptionPageViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
    }
}
