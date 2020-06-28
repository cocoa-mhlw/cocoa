# Overview

This document describe how to set up the development environment.

# Android

**Requirements**

On the Windows

- Windows 10
- Visual Studio 2019
  - Xamarin.Android
  - [Multilingual App Toolkit v4.0](https://marketplace.visualstudio.com/items?itemName=MultilingualAppToolkit.MultilingualAppToolkit-18308)
  - (Option) Hyper-V
	- If you want to get a significantly improved experience of Android Emulator
- Android Studio v4.0
  - JDK (Xamarin)
  - Android SDK
	- Build Tools 29
	- Platform 28, 29

On the macOS

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

More info

- See [Client Side Project Note](Developer-Note.md).

# iOS

**Requirements**

On the Windows

- At writing, i didn't get the build method. Thus, I won't pen it.

On the macOS

- macOS Catalina v10.15.5
- Xcode v11.5
- [Visual Studio for Mac](https://visualstudio.microsoft.com/ja/vs/mac/xamarin/) v8.6.4
  - .NET Core SDK v3.1.301
  - Xamarin.iOS

  NOTE: You can also use [homebrew cask](https://github.com/Homebrew/homebrew-cask) to install these packages.

  ```
  brew cask install visual-studio dotnet-sdk xamarin-ios
  ```

More info

- Use Exposure Notification API. Thus, provisioning profile required `com.apple.developer.exposure-notification entitlement`.
  - If you haven't the entitlement, removing it in `Entitlements.plist`.
- See [Client Side Project Note](Developer-Note.md).

# Server

**Requirments**

Local

- Windows 10
- Visual Studio 2019

Azure (by yourself hosting)

- Azure Functions
- Azure Cosmos
  - Alternative plan: [Azure Cosmos Emulator (Windows only)](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator-release-notes)

More info

- [Easy step by step for functions development](./step-by-step-for-functions-development.md)
- [Infrastructure デプロイメントスクリプト](../infrastructure/Readme.md)

# References

- [Client Side Project Note](Developer-Note.md)
- [Installing Xamarin in Visual Studio 2019](https://docs.microsoft.com/en-us/xamarin/get-started/installation/windows)
- [Use the Azure Cosmos Emulator for local development and testing](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)

