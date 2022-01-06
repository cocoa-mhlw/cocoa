/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using Covid19Radar.Services;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class LocalPathServiceTests
    {
        private readonly MockRepository mockRepository;

        public LocalPathServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
        }

        [Theory]
        [InlineData("440", "111111", "440/111111")]
        [InlineData("440", null, "440")]
        public void GetDiagnosisKeysDir_Tests(string region, string subRegion, string expectedDir)
        {
            var dir = ILocalPathService.GetDiagnosisKeysDir(Path.GetTempPath(), region, subRegion);
            Assert.EndsWith(expectedDir, dir);
        }
    }
}
