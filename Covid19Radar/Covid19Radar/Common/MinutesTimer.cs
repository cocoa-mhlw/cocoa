using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.Common
{

    public class MinutesTimer
    {
        public delegate void TimeOutHandler(EventArgs e);

        public event TimeOutHandler TimeOutEvent;

        private DateTime _startDateTime;

        private bool _timerRunning;

        private int _timeDiffernce;

        public MinutesTimer(int timeDiffernce)
        {
            _timeDiffernce = timeDiffernce;
            this._timerRunning = false;
        }
        public void Start()
        {
            if (this._timerRunning == true)
                return;
            RegistTimer(this.HandleFunc);
        }

        public void Stop()
        {
            this._timerRunning = false;
        }

        private bool HandleFunc()
        {
            if (this._timerRunning == true)
            {

                if (this.TimeOutEvent != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.TimeOutEvent(new EventArgs());
                    });
                }
                RegistTimer(this.HandleFunc);
            }
            return false;
        }

        private void RegistTimer(Func<bool> callback)
        {
            this._startDateTime = DateTime.Now;
            double spanSecond = 60 - this._startDateTime.Second+_timeDiffernce;

            this._timerRunning = true;
            Device.StartTimer(TimeSpan.FromSeconds(spanSecond), callback);
        }
    }
}
