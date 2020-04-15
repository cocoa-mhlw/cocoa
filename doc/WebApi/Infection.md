# POST /api/Infection
for Client Application
At user Infection

## Request
### Content-Type: `application/json`
### Body:
```
{
  "UserUuid": "string", // [Generate by This Function] 
  "Major": int,         // [0 .. 65536]
  "Minor": int          // [0 .. 65536]
}
```
### Example:
```
{
  "userUuid": "3c1be03028cc4a51a54142d0e5b738d0637224227348959150",
  "major": "0",
  "minor": "1"
}
```

## Response 200
success

## Response 400
for Malicious user
