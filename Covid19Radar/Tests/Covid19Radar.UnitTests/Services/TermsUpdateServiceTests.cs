/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;

namespace Covid19Radar.UnitTests.Services
{
    public class TermsUpdateServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;

        public TermsUpdateServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
        }

        private TermsUpdateService CreateService()
        {
            return new TermsUpdateService(mockLoggerService.Object);
        }
    }
}
