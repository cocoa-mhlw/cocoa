// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.IO;
using Covid19Radar.Repository;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class IServerConfigurationRepositoryTests
    {
        [Fact]
        public void CombineAsUrlTest()
        {
            string[] paths = new[] { "foo", "bar" };

            string combinedPath = Path.Combine(paths);
            Assert.Equal("foo/bar", combinedPath);

            string result = IServerConfigurationRepository.CombineAsUrl(paths);
            Assert.Equal("foo/bar", result);
        }

        [Fact]
        public void CombineAsUrlTest_removeHeadSlash()
        {
            string[] paths = new[] { "/foo", "bar" };

            string combinedPath = Path.Combine(paths);
            Assert.Equal("/foo/bar", combinedPath);

            string result = IServerConfigurationRepository.CombineAsUrl(paths);
            Assert.Equal("foo/bar", result);
        }

        [Fact]
        public void CombineAsUrlTest_removeHeadSlash2()
        {
            string[] paths = new[] { "/foo", "/bar" };

            string combinedPath = Path.Combine(paths);
            Assert.Equal("/bar", combinedPath);

            string result = IServerConfigurationRepository.CombineAsUrl(paths);
            Assert.Equal("foo/bar", result);
        }

        [Fact]
        public void CombineAsUrlTest_manySlashes()
        {
            string[] paths = new[] { "/foo", "//bar" };

            string combinedPath = Path.Combine(paths);
            Assert.Equal("//bar", combinedPath);

            string result = IServerConfigurationRepository.CombineAsUrl(paths);
            Assert.Equal("foo/bar", result);
        }

        [Fact]
        public void CombineAsUrlTest_keepLastSlash()
        {
            string[] paths = new[] { "/foo", "/bar/" };

            string combinedPath = Path.Combine(paths);
            Assert.Equal("/bar/", combinedPath);

            string result = IServerConfigurationRepository.CombineAsUrl(paths);
            Assert.Equal("foo/bar/", result);
        }

        [Fact]
        public void CombineAsUrlTest_keepLastOneSlash()
        {
            string[] paths = new[] { "/foo", "/bar//" };

            string combinedPath = Path.Combine(paths);
            Assert.Equal("/bar//", combinedPath);

            string result = IServerConfigurationRepository.CombineAsUrl(paths);
            Assert.Equal("foo/bar/", result);
        }
    }
}
