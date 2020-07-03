# Overview

This document describe how to set up the development environment.

# Client Android Covid19Radar.sln)

**Requirements**
On the Windows
This project is developed using Xamarin Forms, so you need to have Xamarin installed.

## Windows 10

- [Visual Studio 2019 https://visualstudio.microsoft.com/en/vs/community/](https://visualstudio.microsoft.com/en/vs/community/)
  - Xamarin.Android
  - [Multilingual App Toolkit v4.0](https://marketplace.visualstudio.com/items?itemName=MultilingualAppToolkit.MultilingualAppToolkit-18308)
  - (Option) Hyper-V
	- If you want to get a significantly improved experience of Android Emulator
- Android Studio v4.0
  - JDK (Xamarin)
  - Android SDK
	- Build Tools 29
	- Platform 28, 29

## MacOS

- macOS Catalina v10.15.5
- [Visual Studio for Mac](https://visualstudio.microsoft.com/ja/vs/mac/xamarin/) v8.6.4
  - .NET Core SDK v3.1.301
  - Xamarin.Android

  NOTE: You can also use [homebrew cask](https://github.com/Homebrew/homebrew-cask) to install these packages.

  ```
  brew cask install visual-studio dotnet-sdk xamarin-android
  ```

- Android Studio v4.0
  - JDK (Xamarin)
  - Android SDK
	- Build Tools 29
	- Platform 28, 29


## More info

- See [Client Side Project Note](Developer-Note.md).

# Client iOS (Covid19Radar.sln)

**Requirements**

## Windows

- macOS Catalina v10.15.5
- Xcode v11.5
You can remote build from windows with mac.However, if you have a Mac, it's better to build it there.
[Installing Xamarin.iOS on Windows](https://docs.microsoft.com/en-us/xamarin/ios/get-started/installation/windows/)connecting-to-mac/

## macOS

- macOS Catalina v10.15.5
- Xcode v11.5
- [Visual Studio for Mac](https://visualstudio.microsoft.com/ja/vs/mac/xamarin/) v8.6.4
  - .NET Core SDK v3.1.301
  - Xamarin.iOS

  NOTE: You can also use [homebrew cask](https://github.com/Homebrew/homebrew-cask) to install these packages.

  ```
  brew cask install visual-studio dotnet-sdk xamarin-ios
  ```

**It is recommended to use the following runtimes until the next latest runtime is updated.**

https://github.com/xamarin/ExposureNotification.Sample/issues/44#issuecomment-634381146

https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-6/29c4ea73109b377a71866c53a6d43033d5c5e90b/49/package/notarized/xamarin.ios-13.18.2.1.pkg

https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-6/29c4ea73109b377a71866c53a6d43033d5c5e90b/49/package/notarized/xamarin.mac-6.18.2.1.pkg

# Server (Covid19Radar.Functions.sln)

**Requirments**

Local
- .NET Core 3.1
- Azure Function Runtime
- Windows 10 / Linux / Mac OS X
- Visual Studio 2019 or Visual Studio Code

Azure (by yourself hosting)

- Azure Functions
https://github.com/Azure/Azure-Functions

- Azure Cosmos
  - Alternative plan: [Azure Cosmos Emulator (Windows only)](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator-release-notes)

More info

- [Easy step by step for functions development](HOW_TO_BUILD_SERVER_SIDE.md)
- [Infrastructure デプロイメントスクリプト](../infrastructure/Readme.md)

# References

- [Client Side Project Note](Developer-Note.md)

