using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.Common
{
    /// <summary>
    /// Asynchronous locking on key strings.
    /// </summary>
    /// <remarks>
    /// This is primarily a lock that assumes processing of about 200 milliseconds.
    /// </remarks>
    public class KeyLock : IDisposable
    {
        /// <summary>
        /// internal fields class
        /// </summary>
        public class LockItem
        {
            /// <summary>
            /// lock threads count
            /// </summary>
            internal int count = 0;
            /// <summary>
            /// event wait handle
            /// </summary>
            internal System.Threading.EventWaitHandle ev = new System.Threading.EventWaitHandle(true, System.Threading.EventResetMode.AutoReset);
        }

        /// <summary>
        /// Dictionary for storing lock information
        /// </summary>
        static Dictionary<string, LockItem> Locks = new Dictionary<string, LockItem>();

        /// <summary>
        /// Global locking for short-term locking
        /// </summary>
        static object globalLock = new object();

        /// <summary>
        /// Lock wait handle
        /// </summary>
        System.Threading.EventWaitHandle ev;

        /// <summary>
        /// Identification key to lock
        /// </summary>
        string key;

        /// <summary>
        /// Constructor for initialization and acquireing locks
        /// </summary>
        /// <param name="key"></param>
        public KeyLock(string key)
        {
            this.key = key;
            lock (globalLock)
            {
                if (!Locks.ContainsKey(key))
                {
                    Locks.Add(key, new LockItem());
                }
                var item = Locks[key];
                item.count++;
                ev = item.ev;
            }
            ev.WaitOne();
        }

        /// <summary>
        /// Releasing locks and destroying objects
        /// </summary>
        public void Dispose()
        {
            lock (globalLock)
            {
                var item = Locks[key];
                item.count--;
                if (item.count == 0)
                {
                    Locks.Remove(key);
                }
            }
            ev.Set();
        }
    }
}
