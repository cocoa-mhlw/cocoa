using Prism.Mvvm;

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
