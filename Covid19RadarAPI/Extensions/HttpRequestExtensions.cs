using System;
using System.IO;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Covid19Radar.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<T> ParseAndThrow<T>(this HttpRequest request)
        where T : IPayload
        {
            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var serializedObject = JsonConvert.DeserializeObject<T>(requestBody);
            if (serializedObject == null || !serializedObject.IsValid())
            {
                throw new ArgumentException(ErrorStrings.PayloadInvalid);
            }
            return serializedObject;
        }
    }
}
