using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Common
{
    public class UuidLock : IDisposable
    {
        public class LockItem
        {
            internal int count = 0;
            internal System.Threading.EventWaitHandle ev = new System.Threading.EventWaitHandle(true, System.Threading.EventResetMode.AutoReset);
        }
        static Dictionary<string, LockItem> Locks = new Dictionary<string, LockItem>();
        static object globalLock = new object();
        System.Threading.EventWaitHandle ev;
        string uuid;

        public UuidLock(string uuid)
        {
            this.uuid = uuid;
            lock (globalLock)
            {
                if (!Locks.ContainsKey(uuid))
                {
                    Locks.Add(uuid, new LockItem());
                }
                var item = Locks[uuid];
                item.count++;
                ev = item.ev;
            }
            ev.WaitOne();
        }

        public void Dispose()
        {
            lock (globalLock)
            {
                var item = Locks[uuid];
                item.count--;
                if (item.count == 0)
                {
                    Locks.Remove(uuid);
                }
            }
            ev.Set();
        }
    }
}
