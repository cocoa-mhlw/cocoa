using Acr.UserDialogs;
using ReactiveUI;
using Shiny;
using Shiny.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Shiny.Infrastructure
{
    public class GlobalExceptionHandler : IObserver<Exception>, IShinyStartupTask
    {
        readonly IUserDialogs dialogs;
        public GlobalExceptionHandler(IUserDialogs dialogs) => this.dialogs = dialogs;


        public void Start() => RxApp.DefaultExceptionHandler = this;
        public void OnCompleted() { }
        public void OnError(Exception error) { }


        public void OnNext(Exception value)
        {
            Log.Write(value);
            this.dialogs.Alert(value.ToString(), "ERROR");
        }
    }
}
