/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using NUnit.Framework;
using Xamariners.EndToEnd.Xamarin.Infrastructure;

namespace Covid19Radar.UITest
{
    [SetUpFixture]
    public class NUnitAssemblyHooks : NUnitAssemblyHooksBase
    {
        static NUnitAssemblyHooks()
        {
            RunnerConfiguration.CurrentAssembly = typeof(NUnitAssemblyHooks).Assembly;
        }
    }
}