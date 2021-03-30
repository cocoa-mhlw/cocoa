/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;
using System.Threading.Tasks;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class SendLogCompletePageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;

        public SendLogCompletePageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
        }

        private SendLogCompletePageViewModel CreateViewModel()
        {
            var vm = new SendLogCompletePageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object);
            return vm;
        }

        [Fact]
        public void OnClickSendMailCommandTests_Success()
        {
            var testLogId = "test-log-id";

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters { { "logId", testLogId } });

            var actualCalls = 0;
            string actualSubject = default;
            string actualBody = default;
            string[] actualTo = default;
            unitUnderTest.ComposeEmailAsync = (subject, body, to) =>
            {
                actualCalls++;
                actualSubject = subject;
                actualBody = body;
                actualTo = to;
                return Task.CompletedTask;
            };
            unitUnderTest.OnClickSendMailCommand.Execute(null);

            Assert.Equal(1, actualCalls);
            Assert.NotEmpty(actualSubject);
            Assert.Contains(testLogId, actualBody);
            Assert.Single(actualTo);
            Assert.NotEmpty(actualTo[0]);
        }

        [Fact]
        public void OnClickSendMailCommandTests_Exception()
        {
            var testLogId = "test-log-id";

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters { { "logId", testLogId } });

            var actualCalls = 0;
            unitUnderTest.ComposeEmailAsync = (subject, body, to) =>
            {
                actualCalls++;
                throw new Exception();
            };
            unitUnderTest.OnClickSendMailCommand.Execute(null);

            Assert.Equal(1, actualCalls);
        }

        [Fact]
        public void OnClickHomeCommandTests()
        {
            var testLogId = "test-log-id";

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters { { "logId", testLogId } });

            unitUnderTest.OnClickHomeCommand.Execute(null);

            mockNavigationService.Verify(x => x.NavigateAsync("/MenuPage/NavigationPage/HomePage"), Times.Once());
        }
    }
}
