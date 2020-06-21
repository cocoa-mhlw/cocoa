using Newtonsoft.Json;

namespace Covid19Radar.Common
{
    public static class Utils
    {
        public static string SerializeToJson(object obj) => JsonConvert.SerializeObject(obj);

        public static T DeserializeFromJson<T>(string jsonObj) => JsonConvert.DeserializeObject<T>(jsonObj);
    }
}
