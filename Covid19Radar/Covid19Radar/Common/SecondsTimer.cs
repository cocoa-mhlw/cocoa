/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Xamarin.Forms;

namespace Covid19Radar.Common
{

    public class SecondsTimer
    {
        public delegate void TimeOutHandler(EventArgs e);

        public event TimeOutHandler TimeOutEvent;

        private DateTime _startDateTime;

        private bool _timerRunning;

        private int _timeDiffernce;

        public SecondsTimer(int timeDiffernce)
        {
            _timeDiffernce = timeDiffernce;
            this._timerRunning = false;
        }
        public void Start()
        {
            if (this._timerRunning == true)
                return;
            RegisterTimer(this.HandleFunc);
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
                RegisterTimer(this.HandleFunc);
            }
            return false;
        }

        private void RegisterTimer(Func<bool> callback)
        {
            //this._startDateTime = DateTime.Now;
            //double spanSecond = 60 - this._startDateTime.Second+_timeDiffernce;

            this._timerRunning = true;
            Device.StartTimer(TimeSpan.FromSeconds(_timeDiffernce), callback);
        }
    }
}
