using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.DataAccess;
using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

#nullable enable

namespace Covid19Radar.Services
{
    public class OtpService : IOtpService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpGenerator _otpGenerator;
        private readonly ISmsSender _smsSender;
        private readonly IOtpRepository _otpRepository;

        public OtpService(IUserRepository userRepository, IOtpRepository otpRepository, IOtpGenerator otpGenerator, ISmsSender smsSender)
        {
            _userRepository = userRepository;
            _otpGenerator = otpGenerator;
            _smsSender = smsSender;
            _otpRepository = otpRepository;
        }

        public async Task SendAsync(OtpSendRequest request)
        {
            //validate user existence
            var userExists = await _userRepository.Exists(request.User.GetId());
            if (!userExists)
            {
                throw new UnauthorizedAccessException(ErrorStrings.UserNotFound);
            }

            //Generate otp
            var otpGeneratedTime = DateTime.UtcNow;
            var otp = _otpGenerator.Generate(otpGeneratedTime);

            //store otp generation
            await CreateOtpDocument(request.User, otpGeneratedTime);

            //send sms
            var sent = await _smsSender.SendAsync($"{otp} is your OTP for Covid19Radar. Valid for next 30 seconds.",
                request.Phone);
            if (!sent)
            {
                throw new ApplicationException(ErrorStrings.OtpSmsSendFailure);
            }
        }

        public async Task<bool> ValidateAsync(OtpValidateRequest request)
        {
            //Validate user existence
            var userExists = await _userRepository.Exists(request.User.GetId());
            if (!userExists)
            {
                throw new UnauthorizedAccessException(ErrorStrings.UserNotFound);
            }

            //Validate otp request existence
            var otpRequest = await _otpRepository.GetOtpRequestOfUser(request.User.UserUuid);
            if (otpRequest == null)
            {
                throw new UnauthorizedAccessException(ErrorStrings.OtpInvalidValidateRequest);
            }

            //Validate otp code
            var otpGeneratedTime = otpRequest.OtpCreatedTime;
            var validOtp = _otpGenerator.Validate(request.Otp, otpGeneratedTime);
            if (!validOtp)
            {
                return false;
            }

            //Delete otp request for the user
            await _otpRepository.Delete(otpRequest);

            return true;
        }

        private async Task CreateOtpDocument(UserModel user, DateTime otpGeneratedTime)
        {
            var otpDocument = new OtpDocument()
            {
                id = $"{Guid.NewGuid():N}",
                UserUuid = user.UserUuid,
                UserId = user.GetId(),
                OtpCreatedTime = otpGeneratedTime
            };
            await _otpRepository.Create(otpDocument);
        }
    }
}
