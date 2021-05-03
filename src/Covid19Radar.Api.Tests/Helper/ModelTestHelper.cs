﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Helper
{
    public class ModelTestHelper
    {
        public static void PropetiesTest(object item)
        {
            var t = item.GetType();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var p in t.GetProperties(flags))
            {
                // Not test for not have setter
                if (p.SetMethod == null) continue;
                // get
                var value = p.GetValue(item);
                // set
                p.SetValue(item, value);
                // assert
                var actual = p.GetValue(item);
                Assert.AreEqual(value, actual);
            }
        }
    }
}
