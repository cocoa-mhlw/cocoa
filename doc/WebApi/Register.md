# POST /api/Register
for Client Application
At the time of first registration.

## Request
### Content-Type: `application/json`
### Body:
```
{}
```
## Response 200
### Content-Type: `application/json`
### Body:
```
{
  "userUuid": "string",      // [Generate by This Function] 
  "secret": "string",        // [private secret key, require server request.] 
  "jumpConsistentSeed": int  // [0 .. use seed of jumpConsistentHash in client app]
}
```
### Example:
```
{
  "userUuid": "e117e8e4bab24306908fd4983bd6879a637277863263105317",
  "secret": "e1NJzqTHtA7MDj5aYpLIFTx0WoJ1tHV5lvoz2naY5E.....",
  "jumpConsistentSeed": 12345
}
```

## Response 400
for Malicious user
