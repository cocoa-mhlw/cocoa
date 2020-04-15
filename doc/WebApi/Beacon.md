# POST /api/Beacon
for Client Application.
Send a "contact record" from processed beacon information to this api.

## Request
### Content-Type:`application/json`
### Body:
```
{
  "UserUuid": "string",         // by Register api
  "UserMajor": "string",        // by Register api
  "UserMinor": "string",        // by Register api
  "Count": int,                 // by Client App
  "BeaconUuid": "string",       // by Beacon
  "Major": "string",            // by Beacon
  "Minor": "string",            // by Beacon
  "Distance": "string",         // by Beacon
  "Rssi": "string",             // by Beacon
  "TXPower": "string",          // by Beacon
  "ElaspedTime": "TimeSpan",    // by Client App
  "LastDetectTime": "DateTime"  // by Client App
}
```
### Example:
```
{
  "userUuid": "810bb416816543a88f41cd22e1e4d0be637224202532520421",
  "userMajor": "0",
  "userMinor": "1",
  "BeaconUuid": "TEST",
  "Count" 2,
  "Major": "0",
  "Minor": "1",
  "Distance": 1.0,
  "Rssi": 9,
  "TXPower": 140,
  "ElaspedTime": "00:02:00",
  "LastDetectTime": "2020-04-11T00:15:11.000Z"
}
```

## Response 201 Created
no body

## Response 400
Bad Request. for Malicious user

## Response 500
other exceptions

## Response 503
too many request CosmosDB
