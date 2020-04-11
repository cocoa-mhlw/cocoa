using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Tests.Mock
{
    public class ResponseCookiesMock : IResponseCookies
    {
        public Dictionary<string, string> store = new Dictionary<string, string>();
        public void Append(string key, string value)
        {
            store.Add(key, value);
        }

        public CookieOptions _options;
        public void Append(string key, string value, CookieOptions options)
        {
            store.Add(key, value);
            _options = options;
        }

        public void Delete(string key)
        {
            store.Remove(key);
        }

        public void Delete(string key, CookieOptions options)
        {
            store.Remove(key);
            _options = options;
        }
    }
}
