using Newtonsoft.Json;

namespace Covid19Radar.Models
{
    [JsonObject("otpSendRequest")]
    public class OtpSendRequest : IPayload
    {
        [JsonProperty("user")]
        public UserModel User { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }

        public bool IsValid()
        {
            return User != null
                   && !string.IsNullOrEmpty(User.UserUuid)
                   && !string.IsNullOrEmpty(User.Major)
                   && !string.IsNullOrEmpty(User.Minor)
                   && !string.IsNullOrEmpty(Phone);
        }
    }
}
