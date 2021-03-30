/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;
using System.Security.Cryptography;

namespace GenerateKeys
{
    class Program
    {
        static void Main(string[] args)
        {

            var symmetric = Aes.Create();
            symmetric.Mode = CipherMode.CBC;
            symmetric.Padding = PaddingMode.ISO10126;
            symmetric.KeySize = 256;

            var to = RandomNumberGenerator.GetInt32(10, 100);
            for (int i = 0; i < to; i++)
            {
                symmetric.GenerateKey();
                symmetric.GenerateIV();
            }

            Console.WriteLine($"CRYPTION-KEY:{Convert.ToBase64String(symmetric.Key)}");
            Console.WriteLine($"CRYPTION-IV:{Convert.ToBase64String(symmetric.IV)}");

            var to2 = RandomNumberGenerator.GetInt32(10, 100);
            for (int i = 0; i < to2; i++)
            {
                symmetric.GenerateKey();
                symmetric.GenerateIV();
            }

            Console.WriteLine($"CRYPTION-KEY2:{Convert.ToBase64String(symmetric.Key)}");
            Console.WriteLine($"CRYPTION-IV2:{Convert.ToBase64String(symmetric.IV)}");


            var hash = new HMACSHA512();
            Console.WriteLine($"HASH:{Convert.ToBase64String(hash.Key)}");

            Console.ReadLine();
        }
    }
}
