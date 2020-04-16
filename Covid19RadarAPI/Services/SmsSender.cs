using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Covid19Radar.Services
{
    public class SmsSender : ISmsSender
    {
        private readonly string twilioSid;
        private readonly string twilioToken;
        private readonly string fromPhone;
        public SmsSender(IConfiguration configuration)
        {
            twilioSid = configuration["TWILIO_SID"];
            twilioToken = configuration["TWILIO_TOKEN"];
            fromPhone = configuration["TWILIO_FROM_PHONE"];
            TwilioClient.Init(twilioSid,twilioToken);
        }

        public async Task<bool> SendAsync(string body, string toPhone)
        {
            try
            {
                var messageOptions = new CreateMessageOptions(new PhoneNumber(toPhone))
                {
                    From = new PhoneNumber(fromPhone), 
                    Body = body
                };
                var message = MessageResource.Create(messageOptions);
                return message.Status == MessageResource.StatusEnum.Queued 
                       || message.Status == MessageResource.StatusEnum.Sent
                       || message.Status == MessageResource.StatusEnum.Sending;
            }
            catch
            {
                //Suppress for now
            }
            return false;
        }
    }
}
