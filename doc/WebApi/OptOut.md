# DELETE /api/OptOut/{userUuid}
for Client Application
At the time of first registration.

## Request
### Authorization: Bearer [Secret at Register]
### Content-Type: `application/json`
### Body:
```
{
  "userUuid": "string"
}
```
## Response 204
Successful

## Response 400
for Malicious user
