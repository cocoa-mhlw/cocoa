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

            var otp = _otpGenerator.Generate();

            //store otp generation
            await CreateOtpDocument(request.User, otp);

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
            if (request.Otp != otpRequest.Otp || DateTime.UtcNow > otpRequest.OtpCreatedTime.AddSeconds(30))
            {
                return false;
            }

            //Delete otp request for the user
            await _otpRepository.Delete(otpRequest);

            return true;
        }

        private async Task CreateOtpDocument(UserModel user, string otp)
        {
            var otpDocument = new OtpDocument(
                id: $"{Guid.NewGuid():N}",
                userId: user.GetId(),
                userUuid: user.UserUuid,
                otpCreatedTime: DateTime.UtcNow,
                otp: otp
            );
            await _otpRepository.Create(otpDocument);
        }
    }
}
