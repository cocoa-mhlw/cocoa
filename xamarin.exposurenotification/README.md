
# Xamarin Exposure Notification

![Nuget](https://img.shields.io/nuget/v/Xamarin.ExposureNotification?label=Cross-Platform)
![Nuget](https://img.shields.io/nuget/v/Xamarin.GooglePlayServices.Nearby.ExposureNotification?label=Android)
![Nuget](https://img.shields.io/nuget/v/Xamarin.iOS.ExposureNotification?label=iOS)

[Read our Planning Document with more details about how Exposure Notifications work](https://github.com/xamarin/xamarin.exposurenotification/blob/master/Exposure%20Notification%20Planning.pdf)

Apple and Google are both creating API’s for a compatible BLE based Contact Tracing implementation which relies heavily on generating and storing rolling unique identifiers on a device, which are broadcast to nearby devices.  Devices which detect nearby identifiers then store these identifiers as they come into range (or contact) with for up to 14 days.

When a person has confirmed a diagnosis, they tell their device which then submits the locally stored, self-generated rolling unique identifiers from the last 14 days to a back end service provided by the app implementing the API.

Devices continually request the keys submitted by diagnosed people from the backend server.  The device then compares these keys to the unique identifiers of other devices it has been near in the last 14 days.

## Xamarin.ExposureNotification

This project contains the cross platform wrapper API around the native Android and iOS API's.  The sample app uses this library to implement the exposure notification code one time for both platforms.

### Bindings to Native APIs _(NuGet)_

We also have NuGet packages available with bindings to the native Android and iOS Exposure Notifaction API's

 - iOS: [Xamarin.iOS.ExposureNotification](https://www.nuget.org/packages/Xamarin.iOS.ExposureNotification/) (Requires XCode 11.5 beta1 or newer)
 - Android: [Xamarin.GooglePlayServices.Nearby.ExposureNotification](https://www.nuget.org/packages/Xamarin.GooglePlayServices.Nearby.ExposureNotification/)


# Sample

![Sample App Animated Overview](https://github.com/xamarin/xamarin.exposurenotification/raw/master/exposure-notifications.gif)

## Mobile App

A sample mobile app written using C# and Xamarin.Forms which accesses the native iOS and Android Exposure Notification API's and communicates with the sample server backend.

> NOTE: There is a Xamarin.iOS fix pending release which is required to use `BGTask` related API's on iOS.  This means in order for the periodic background fetching task to be invoked, the fix is required.  Until the fix is available in a soon to be released stable version you can install these builds to test with on macOS:
>
> * [xamarin.ios-13.18.2.1.pkg](https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-6/29c4ea73109b377a71866c53a6d43033d5c5e90b/49/package/notarized/xamarin.ios-13.18.2.1.pkg)
> * [xamarin.mac-6.18.2.1.pkg](https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-6/29c4ea73109b377a71866c53a6d43033d5c5e90b/49/package/notarized/xamarin.mac-6.18.2.1.pkg)
>
> See [Issue #44](https://github.com/xamarin/xamarin.exposurenotification/issues/44#issuecomment-634381146) for more details.


## Server

A sample backend to handle diagnosis submissions.  This sample consists of Azure Functions, a EF Core Compatible Database, Azure Blob Storage, and Azure Key Vault.  You can read more about the [Server Architecture and Configuration in the Wiki](https://github.com/xamarin/xamarin.exposurenotification/wiki/Server-Architecture-&-Configuration)


# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
