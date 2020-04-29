namespace Covid19Radar.Models
{
    public class OtpValidateRequest : IPayload
    {
        public UserModel User { get; set; }
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
