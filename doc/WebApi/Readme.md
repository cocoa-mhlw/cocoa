# Web api documents

## simple apis

### Register

This registration API is called first by the client app.
Issue a unique private key during registration.
It means getting started with the application.
It also issues special Hash code that avoids biased server load.
The private key is also required at the end of this app at the time of self-diagnosis registration.
It was designed for anonymity, security, and load leveling.

[Register api docments](./Register.md)


### Diagnosis

Call to notify other users that a user has been infected.
This is a self-diagnosis submission. Requires the approval of a public health agency.
You need a design tailored to your country.

[Diagnosis api docments](./Diagnosis.md)

### OptOut

Call this API if the user chooses to stop using this app.
Completely remove self-diagnostic TEK, generated UserUuid, and Secret before being collected in batch.

[OptOut api docments](./OptOut.md)
