using Prism.AppModel;
using Prism.Commands;
using System;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Covid19Radar.ViewModels
{
    public class DialogViewModel : BindableBase, IDialogAware, IAutoInitialize
    {
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public DelegateCommand CloseCommand { get; }

        public DialogViewModel()
        {
            CloseCommand = new DelegateCommand(() => RequestClose(null));
        }

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // ダイアログのパラメータ受け取り
            // IAutoInitializeが有効の場合は自動
            if (parameters.ContainsKey("Message"))
            {
                Message = parameters.GetValue<string>("Message");
            }
        }
    }
}
