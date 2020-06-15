# PUT /api/diagnosis
for Client Application
At the time of first registration.

## Request
### Authorization: Bearer [Secret at Register]
### Content-Type: `application/json`
### Body:
```
{
  "userUuid": "string",
  "keys": [
    {
      "keyData": "string",
      "rollingStartNumber": int,
      "rollingPeriod": int,
      "transmissionRisk": int
    }
  ],
  "regions": ["string", "string"],
  "platform": "string",
  "deviceVerificationPayload": "string",
  "appPackageName": "string",
  "verificationPayload": "string",
  "padding": "string"
}
```
## Response 204
Successful

## Response 400
for Malicious user
