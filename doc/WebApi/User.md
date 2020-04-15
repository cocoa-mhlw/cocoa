# POST /api/User
for Client Application.
Receive a change of user's status from this api.

## Request
### Content-Type: `application/json`
### Body:
```
{
  "UserUuid": "[UserUuid]"
  "Major": int,           // [0 .. 65536]
  "Minor": int,           // [0 .. 65536]
}
```
### Example:
```
{
  "userUuid": "bd897428eb744e419197e1d642e00f53637224174377641499",
  "major": "0",
  "minor": "1"
}
```

## Response 200

### Content-Type: `application/json`
### Body:
```
{
  "UserStatus": "[State]" // [any enum name]
}
```

## Response 400
for Malicious user

## Response 404
not found user.

## Response 503
too many request CosmosDB
