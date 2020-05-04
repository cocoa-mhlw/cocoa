using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using OtpNet;

namespace Covid19Radar.Services
{
    public class OtpGenerator : IOtpGenerator
    {
        private readonly Random _rand = new Random();

        public string Generate()
        {
            lock (_rand)
            {
                return _rand.Next(0, 1000000).ToString("D6");
            }
        }
    }
}
