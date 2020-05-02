using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Covid19Radar.DataAccess;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Covid19Radar.Tests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;

namespace Covid19Radar.Tests.Services
{
    [TestClass]
    public class OtpService_SendTest
    {
        private const string DefaultOtp = "123456";

        private OtpService _otpService;
        private Mock<IUserRepository> _userRepository;
        private Mock<IOtpRepository> _otpRepository;
        private Mock<IOtpGenerator> _otpGenerator;
        private Mock<ISmsSender> _smsSender;

        [TestInitialize]
        public void Setup()
        {
            (_otpService, _userRepository, _otpRepository, _otpGenerator, _smsSender) =
                MoqMockHelper.Create<OtpService, IUserRepository, IOtpRepository, IOtpGenerator, ISmsSender>();

            var request = CreateRequest();

            _userRepository.Setup(r => r.Exists(request.User.GetId())).Returns(Task.FromResult(true));
            _otpGenerator.Setup(o => o.Generate()).Returns(DefaultOtp);
            _smsSender.Setup(r => r.SendAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
        }

        [TestMethod]
        public async Task ShouldThrowUnauthorizedAccessExceptionIfUserDoesNotExists()
        {
            _userRepository.Setup(r => r.Exists(It.IsAny<string>())).Returns(Task.FromResult(false));

            Func<Task> action = async () => await _otpService.SendAsync(CreateRequest());
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [TestMethod]
        public async Task ShouldCreateOtpDocumentOnSuccess()
        {
            OtpDocument docAdded = null;

            _otpRepository.Setup(r => r.Create(It.IsAny<OtpDocument>()))
                .Callback((OtpDocument doc) => docAdded = doc);

            var request = CreateRequest();
            await _otpService.SendAsync(request);

            docAdded.Should().NotBeNull();
            docAdded.id.Should().NotBeNullOrWhiteSpace();
            docAdded.UserUuid.Should().Be(request.User.UserUuid);
            docAdded.UserId.Should().Be(request.User.GetId());
            docAdded.OtpCreatedTime.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public async Task ShouldSendSmsOnSuccess()
        {
            string bodyParam = null;
            string toPhoneParam = null;

            _smsSender.Setup(r => r.SendAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string body, string toPhone) => (bodyParam, toPhoneParam) = (body, toPhone))
                .Returns(Task.FromResult(true));

            var request = CreateRequest();
            await _otpService.SendAsync(request);

            bodyParam.Should().Contain(DefaultOtp);
            toPhoneParam.Should().Be(request.Phone);
        }

        [TestMethod]
        public async Task ShouldThrowApplicationExceptionWhenFailedToSendSms()
        {
            _smsSender.Setup(r => r.SendAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            Func<Task> action = async () => await _otpService.SendAsync(CreateRequest());
            await action.Should().ThrowAsync<ApplicationException>();
        }

        private OtpSendRequest CreateRequest()
        {
            return new OtpSendRequest
            {
                User = new UserModel
                {
                    UserUuid = "UserUuid",
                    Major = "1",
                    Minor = "1",
                },
                Phone = "123456789"
            };
        }
    }

    [TestClass]
    public class OtpService_ValidateTest
    {
        private const string DefaultOtp = "123456";

        private OtpService _otpService;
        private Mock<IUserRepository> _userRepository;
        private Mock<IOtpRepository> _otpRepository;
        private OtpValidateRequest _defaultRequest;
        private OtpDocument _defaultOtpDoc;

        [TestInitialize]
        public void Setup()
        {
            _defaultRequest = CreateRequest();
            _defaultOtpDoc = CreateOtpDocument();

            (_otpService, _userRepository, _otpRepository, _, _) =
                MoqMockHelper.Create<OtpService, IUserRepository, IOtpRepository, IOtpGenerator, ISmsSender>();

            _userRepository
                .Setup(r => r.Exists(_defaultRequest.User.GetId()))
                .Returns(Task.FromResult(true));
            _otpRepository
                .Setup(r => r.GetOtpRequestOfUser(_defaultRequest.User.UserUuid))
                .Returns(Task.FromResult(_defaultOtpDoc));
        }

        [TestMethod]
        public async Task ShouldThrowUnauthorizedAccessExceptionIfUserDoesNotExists()
        {
            _userRepository
                .Setup(r => r.Exists(_defaultRequest.User.GetId()))
                .Returns(Task.FromResult(false));

            Func<Task> action = async () => await _otpService.ValidateAsync(_defaultRequest);
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [TestMethod]
        public async Task ShouldThrowUnauthorizedAccessExceptionIfOtpDocDoesNotExists()
        {
            _otpRepository
                .Setup(r => r.GetOtpRequestOfUser(_defaultRequest.User.UserUuid))
                .Returns(Task.FromResult<OtpDocument>(null));

            Func<Task> action = async () => await _otpService.ValidateAsync(_defaultRequest);
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [TestMethod]
        public async Task ShouldReturnTrueWhenOtpIsValid()
        {
            var result = await _otpService.ValidateAsync(_defaultRequest);

            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task ShouldReturnFalseWhenOtpIsNotCorrect()
        {
            var request = CreateRequest("654321");
            var result = await _otpService.ValidateAsync(request);

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ShouldReturnFalseWhenOtpIsOutdated()
        {
            _otpRepository
                .Setup(r => r.GetOtpRequestOfUser(_defaultRequest.User.UserUuid))
                .Returns(Task.FromResult(CreateOtpDocument(DateTime.UtcNow.AddSeconds(-30))));

            var result = await _otpService.ValidateAsync(_defaultRequest);

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ShouldDeleteOtpDocumentAfterValidation()
        {
            OtpDocument deletedDoc = null;

            _otpRepository
                .Setup(r => r.Delete(It.IsAny<OtpDocument>()))
                .Callback((OtpDocument doc) => deletedDoc = doc);

            var result = await _otpService.ValidateAsync(_defaultRequest);

            deletedDoc.Should().Be(_defaultOtpDoc);
        }

        private OtpValidateRequest CreateRequest(string otp = DefaultOtp)
        {
            return new OtpValidateRequest
            {
                User = new UserModel
                {
                    UserUuid = "UserUuid",
                    Major = "1",
                    Minor = "1",
                },
                Otp = otp,
            };
        }

        private OtpDocument CreateOtpDocument(DateTime? creationTime = null)
        {
            var request = CreateRequest();
            return new OtpDocument(
                id: "id",
                userId: request.User.GetId(),
                userUuid: request.User.UserUuid,
                otpCreatedTime: creationTime ?? DateTime.UtcNow,
                otp: DefaultOtp
            );
        }
    }
}
