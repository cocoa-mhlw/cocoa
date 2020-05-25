namespace Covid19Radar.Api.Models
{
    /// <summary>
    /// Payload for http request message.
    /// </summary>
    public interface IPayload
    {
        /// <summary>
        /// Validation Results
        /// </summary>
        /// <returns>true if valid</returns>
        bool IsValid();
    }
}
