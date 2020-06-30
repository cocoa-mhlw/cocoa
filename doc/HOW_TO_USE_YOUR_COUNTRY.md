# If you want to use this in your country
Apple and Google place restrictions on the use of this API by public health authorities only.
Therefore, you should contact Google and Apple through public health authorities.
These are required for the device to work. You need to gain access to the API.

## Google
1. Please contact through a public health official.
2. You need to submit the app signature to Google.For more information, talk to your Google representative.
3. Create New Development Google's account (Join google's exposure notification developer groups, For more information, talk to your Google representative.)
4. Create SafetyNet Attestation API  https://developer.android.com/training/safetynet/attestation?hl=en

## Apple

1. Please contact through a public health official.If you have Apple through public health authorities Account (You must Account Holder Role !!). You can request entitlement https://developer.apple.com/contact/request/exposure-notification-entitlement
2. Create App ID  (You must Account Holder Role !!)
3. Set Exposure Notification Entitlement in *Provisioning Profile* (You must Account Holder Role !! and not app identifiers)
4. Create Development Cert (App Manager Role) / Profile etc... (From here, it is the same as the general development method.)
5. Create New Key (Generate Device Check Key - using Apple's Device Check) in Apple Developer Connection. https://developer.apple.com/documentation/devicecheck

# Prepare Development Environment

Requirements, prepare IDE, see below

https://github.com/Covid-19Radar/Covid19Radar/blob/master/doc/Developer.md

# Build 1st time in simurator / emurator

1. Open Covid19Radar Folder
2. Open Covid19Radar.sln
3. Check mockup mode (see below)
4. Right click Covid19Radar.Android and Set as Startup Project (if you want run android)
5. Right click Covid19Radar.iOS and Set as Startup Project (if you want run android)
6. Ctrl + Build + B (Build Solution)
7. Debug Emurator / Simulator (F5 Key)

## Mockup mode

Originally it does not work without API, but there is a mockup mode to work with the emulator. It is useful when you want to change the appearance or modify it.
Please note that in fact Exposure Notification is not working.


