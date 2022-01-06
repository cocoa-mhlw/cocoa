// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;
using Chino;
using Xunit;

namespace Covid19Radar.UnitTests
{
    public class UnionTests
    {
        private ScanInstance CreateScanInstance()
            => new ScanInstance()
            {
                MinAttenuationDb = 45,
                SecondsSinceLastScan = 300,
                TypicalAttenuationDb = 37,
            };

        private ExposureWindow CreateExposureWindow()
            => new ExposureWindow()
            {
                CalibrationConfidence = CalibrationConfidence.Medium,
                DateMillisSinceEpoch = 152345000,
                Infectiousness = Infectiousness.Standard,
                ReportType = ReportType.ConfirmedTest,
                ScanInstances = new List<ScanInstance>() { CreateScanInstance() },
            };

        [Fact]
        public void UnionTest1()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();
            exposureWindow2.Infectiousness = Infectiousness.High;

            var exposureWindow3 = CreateExposureWindow();
            var exposureWindow4 = CreateExposureWindow();
            exposureWindow4.Infectiousness = Infectiousness.None;

            var list1 = new List<ExposureWindow>() {
                exposureWindow1,
                exposureWindow2
            };
            var list2 = new List<ExposureWindow>() {
                exposureWindow3,
                exposureWindow4
            };

            var unionList = list1.Union(list2);
            Assert.Equal(3, unionList.Count());
            Assert.Contains(exposureWindow2, unionList);
            Assert.Contains(exposureWindow4, unionList);

            Assert.Contains(exposureWindow1, unionList);
            Assert.Contains(exposureWindow3, unionList);
        }

        [Fact]
        public void SortTest1()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();
            exposureWindow2.Infectiousness = Infectiousness.High;
            var exposureWindow3 = CreateExposureWindow();
            exposureWindow3.ReportType = ReportType.ConfirmedTest;
            var exposureWindow4 = CreateExposureWindow();
            exposureWindow4.Infectiousness = Infectiousness.None;
            exposureWindow4.ReportType = ReportType.SelfReport;

            var list = new List<ExposureWindow>() {
                exposureWindow1,
                exposureWindow2,
                exposureWindow3,
                exposureWindow4
            };

            list.Sort(new ExposureWindow.Comparer());
            Assert.Equal(4, list.Count());

            Assert.Equal(exposureWindow2, list[0]);
            Assert.Equal(exposureWindow3, list[1]);
            Assert.Equal(exposureWindow1, list[2]);
            Assert.Equal(exposureWindow4, list[3]);
        }
    }
}
