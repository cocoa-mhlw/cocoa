using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public abstract class IntervalUpdateServiceBase<TModel>
    {
        private DateTime _LastUpdateTime = DateTime.MinValue;
        private DateTime _LastCheckTime = DateTime.MinValue;
        private object LastLock = new object();
        private int LastCheckInterval;

        private DateTime LastGetTime = DateTime.MinValue;
        private object LastGetLock = new object();

        protected TModel[] Models = new TModel[] { };

        public DateTime LastUpdate
        {
            get
            {
                RefereshLastTime();
                return _LastUpdateTime;
            }
        }

        protected void SetInterval(int second)
        {
            LastCheckInterval = second;
        } 

        protected void RefereshLastTime()
        {
            if ((DateTime.UtcNow - _LastCheckTime).TotalSeconds > LastCheckInterval)
            {
                lock (LastLock)
                {
                    if ((DateTime.UtcNow - _LastCheckTime).TotalSeconds > LastCheckInterval)
                    {
                        _LastUpdateTime = GetLastTime();
                        _LastCheckTime = DateTime.UtcNow;
                    }
                }
            }
        }

        protected abstract DateTime GetLastTime();

        public IEnumerable<TModel> GetNotificationMessages(DateTime lastClientTime, out DateTime lastTime)
        {
            lastTime = _LastUpdateTime;
            if (_LastUpdateTime > LastGetTime)
            {
                lock (LastGetLock)
                {
                    if (_LastUpdateTime > LastGetTime)
                    {
                        Models = GetModels(LastGetTime);
                    }
                }
            }
            return Models.Where(_ => WhereForClient(_, lastClientTime));
        }

        protected abstract TModel[] GetModels(DateTime lastGetTime);

        protected abstract bool WhereForClient(TModel model, DateTime lastClienttime);
    }
}
