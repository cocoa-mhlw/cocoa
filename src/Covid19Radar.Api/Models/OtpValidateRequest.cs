using Newtonsoft.Json;

namespace Covid19Radar.Models
{
    [JsonObject("otpValidateRequest")]
    public class OtpValidateRequest : IPayload
    {
        [JsonProperty("user")]
        public UserModel User { get; set; }
        [JsonProperty("otp")]
        public string Otp { get; set; }

        public bool IsValid()
        {
            return User != null
                   && !string.IsNullOrEmpty(User.UserUuid)
                   && !string.IsNullOrEmpty(User.Major)
                   && !string.IsNullOrEmpty(User.Minor)
                   && !string.IsNullOrEmpty(Otp);
        }
    }
}
