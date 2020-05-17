# Covid19Radar (Japanese)

Now, We move to GitHub Org [Project Covid19Radar](https://github.com/Covid-19Radar)

私たちは現在、機械翻訳結果の確認レビュアーを募集しています。詳しくは [How to Transrate application](https://github.com/Covid-19Radar/Covid19Radar/blob/master/HOW_TO_TRANSRATE_CONTRIBUTE.md) をご確認ください。

iOS Build status [![iOS Build status](https://build.appcenter.ms/v0.1/apps/9c268337-4db9-4bf4-be09-efaf16672c15/branches/master/badge)](https://appcenter.ms)

Android Build status [![Android Build status](https://build.appcenter.ms/v0.1/apps/3dcdf5b5-da95-4d03-96a6-e6ed42de7e16/branches/master/badge)](https://appcenter.ms)

本アプリは、Exposure Notification / Bluetooth(BLE)を利用して、お互いの接触ログを取得します。
![アプリの概念](img/explanation.png)


## Thank you for Your Contribute !!! [Contributors List](https://github.com/Covid-19Radar/Covid19Radar/blob/master/CONTRIBUTORS.md)
コントリビューションとプルリクエストをお待ちしています。
コントリビューションルールについて、ご確認ください。
[Contribute Rule](https://github.com/Covid-19Radar/Covid19Radar/blob/master/HOW_TO_CONTRIBUTE.md)

## テスト用にアプリをインストールするには

以下のリンクからアプリをテスト用にインストールしてください。現在はGoogle/AppleによるSDKが各ベータ版に提供されるまでテストができない状況です。

### Android端末

https://install.appcenter.ms/orgs/Covid19Radar/apps/Covid19RadarAndroid/releases

テスト用のデバイスの構成は、以下のドキュメントを参照してください:
https://docs.microsoft.com/ja-jp/appcenter/distribution/testers/testing-android

### iOS端末

https://install.appcenter.ms/orgs/Covid19Radar/apps/Covid19RadarIOS/releases

テスト用のデバイスの構成は、以下のドキュメントを参照してください:
https://docs.microsoft.com/ja-jp/appcenter/distribution/testers/testing-ios


### 開発環境について

クライアント側は、Xamarin Forms(iOS and Android) with C# と Prism(MVVM DryIoC)を使っています。Visual Studio for Windows もしくは Visual Studio for Macで開発可能です。

https://visualstudio.microsoft.com/ja/xamarin/

![アプリ設定に関して](img/design00.png)

デバイスの以下の機能の利用許可が必須となります。 

1. Exposure notification
2. Bluetooth
3. Local Notification

設定完了後、本アプリをインストールしている人同士の接触ログを自動で記録します。

# デザインについて

[Adobe XD](https://www.adobe.com/jp/products/xd.html)を利用してデザイン制作を行っています。

![画面全体図](img/design01.jpg)

デザインファイルを確認する場合は、Adobe XDをインストールしてください。（無料で利用可能）

## アプリのプロトタイプ

以下のURLにアクセスすると、画面遷移を確認いただけます。

[プロトタイプ画面（日本語）](https://xd.adobe.com/view/3388de85-871b-46a1-5c3f-7ecacfa1a5cf-45b9/)

## ライセンス

Covid19Radar is licensed under the GNU Affero General Public License v3.0. See
[LICENSE](./LICENSE) for the full
license text.

以下は、原作者の意図に応じた、このライセンスの追加項目です。
AGPLに加えて、このプロジェクトでは共著者の著作者人格権の行使を許可しません。
各著者による論争または訴訟は一切許可されていません。

## サードパーティーソフトウェアについて

This file incorporates components from the projects listed [document](./COPYRIGHT_THIRD_PARTY_SOFTWARE_NOTICES.md).

