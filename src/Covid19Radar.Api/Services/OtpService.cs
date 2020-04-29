using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Covid19Radar.Services
{
    public class OtpService : IOtpService
    {
        private readonly ICosmos _db;
        private readonly IOtpGenerator _otpGenerator;
        private readonly ISmsSender _smsSender;

        public OtpService(ICosmos db, IOtpGenerator otpGenerator, ISmsSender smsSender)
        {
            _db = db;
            _otpGenerator = otpGenerator;
            _smsSender = smsSender;
        }
        public async Task SendAsync(OtpSendRequest request)
        {
            //validate user existence
            var userExists = await UserExists(request.User);
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
            var userExists = await UserExists(request.User);
            if (!userExists)
            {
                throw new UnauthorizedAccessException(ErrorStrings.UserNotFound);
            }

            //Validate otp request existence
            var otpRequest = await GetOtpRequest(request.User);
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
            await DeleteOtpDocument(otpRequest.id, request.User.UserUuid);

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
            await _db.Otp.CreateItemAsync(otpDocument, new PartitionKey(user.UserUuid));
        }

        private async Task<bool> UserExists(UserModel user)
        {
            bool userFound = false;
            try
            {
                var userResult = await _db.User.ReadItemAsync<UserResultModel>(user.GetId(), PartitionKey.None);
                if (userResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    userFound = true;
                }
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == HttpStatusCode.NotFound)
                {
                    userFound = false;
                }
            }

            return userFound;
        }

        private async Task<OtpDocument> GetOtpRequest(UserModel user)
        {
            OtpDocument otpDocument = null;
            try
            {
                var iterator = _db.Otp.GetItemLinqQueryable<OtpDocument>()
                        .Where(o => o.UserUuid == user.UserUuid)
                        .ToFeedIterator();

                if (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    otpDocument = response.FirstOrDefault();
                }
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == HttpStatusCode.NotFound)
                {
                    otpDocument = null;
                }
            }

            return otpDocument;
        }

        private async Task DeleteOtpDocument(string id, string userUuid)
        {
            try
            {
                await _db.Otp.DeleteItemAsync<OtpDocument>(id, new PartitionKey(userUuid));
            }
            catch (CosmosException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
