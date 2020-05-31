using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExposureNotification.Backend.Database;
using Newtonsoft.Json.Linq;

namespace ExposureNotification.Backend.DeviceVerification
{
	public static class AppleVerify
	{
		const string appleDeviceCheckUrl = "https://api.development.devicecheck.apple.com/v1/validate_device_token";

		public static async Task<bool> VerifyToken(string token, DateTimeOffset requestTime, AuthorizedAppConfig app)
		{
			var http = new HttpClient();

			var timestamp = requestTime.ToUnixTimeMilliseconds();
			var id = Guid.NewGuid().ToString();

			var json = new JObject();
			json["device_token"] = token;
			json["timestamp"] = timestamp;
			json["transaction_id"] = id;

			var str = json.ToString();
			Console.WriteLine(str);
			//var json = "{\"device_token\":\"" + token + "\",\"transaction_id\":\"" + id + "\",\"timestamp\":" + timestamp + "}";

			var jwt = GenerateClientSecretJWT(requestTime, app.DeviceCheckKeyId, app.DeviceCheckTeamId, app.DeviceCheckPrivateKey);

			var req = new HttpRequestMessage(HttpMethod.Post, appleDeviceCheckUrl);
			req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
			req.Content = new StringContent(str);

			var response = await http.SendAsync(req);

			if (response.StatusCode != System.Net.HttpStatusCode.OK)
				return false;

			return true;
		}

		static string GenerateClientSecretJWT(DateTimeOffset requestTime, string keyId, string teamId, string p8FileContents)
		{
			var headers = new Dictionary<string, object>
			{
				{ "alg", "ES256" },
				{ "kid", keyId }
			};

			var payload = new Dictionary<string, object>
			{
				{ "iss", teamId },
				{ "iat", requestTime.ToUnixTimeMilliseconds() }
			};

			var secretKey = CleanP8Key(p8FileContents);

			// Get our headers/payloads into a json string
			var headerStr = "{" + string.Join(",", headers.Select(kvp => $"\"{kvp.Key}\":\"{kvp.Value.ToString()}\"")) + "}";
			var payloadStr = "{";
			foreach (var kvp in payload)
			{
				if (kvp.Value is int || kvp.Value is long || kvp.Value is double)
					payloadStr += $"\"{kvp.Key}\":{kvp.Value.ToString()},";
				else
					payloadStr += $"\"{kvp.Key}\":\"{kvp.Value.ToString()}\",";
			}
			payloadStr = payloadStr.TrimEnd(',') + "}";


			// Load the key text
			var key = CngKey.Import(Convert.FromBase64String(secretKey), CngKeyBlobFormat.Pkcs8PrivateBlob);

			using (var dsa = new ECDsaCng(key))
			{
				var unsignedJwt = Base64UrlEncode(Encoding.UTF8.GetBytes(headerStr))
										+ "." + Base64UrlEncode(Encoding.UTF8.GetBytes(payloadStr));

				var signature = dsa.SignData(Encoding.UTF8.GetBytes(unsignedJwt), HashAlgorithmName.SHA256);

				return unsignedJwt + "." + Base64UrlEncode(signature);
			}
		}

		static string CleanP8Key(string p8Contents)
		{
			// Remove whitespace
			var tmp = Regex.Replace(p8Contents, "\\s+", string.Empty, RegexOptions.Singleline);

			// Remove `---- BEGIN PRIVATE KEY ----` bits
			tmp = Regex.Replace(tmp, "-{1,}.*?-{1,}", string.Empty, RegexOptions.Singleline);

			return tmp;
		}

		static string Base64UrlEncode(byte[] data)
		{
			var base64 = Convert.ToBase64String(data, 0, data.Length);
			var base64Url = new StringBuilder();

			foreach (var c in base64)
			{
				if (c == '+')
					base64Url.Append('-');
				else if (c == '/')
					base64Url.Append('_');
				else if (c == '=')
					break;
				else
					base64Url.Append(c);
			}

			return base64Url.ToString();
		}
	}
}
