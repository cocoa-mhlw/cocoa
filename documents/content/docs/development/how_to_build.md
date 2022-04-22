---
title: "ビルド方法"
weight: 20
type: docs
---
# ビルド方法
## 事前準備
COCOAはXamarinベースのアプリなので、Xamarin開発環境(Visual Studio)を自身のPCへインストールします。
詳細な手順等については、[Xamarinのインストール](https://docs.microsoft.com/ja-jp/xamarin/get-started/installation/)を参照してください。  

Xamarinについては、[Xamarinドキュメント](https://docs.microsoft.com/ja-jp/xamarin/) を参照してください。
## COCOAの構成
COCOA内には2つのソリューションファイルがあります。  

- COCOAアプリ・・・Covid19Radar.sln
- COCOAサーバ・・・Covid19Radar.Functions.sln

## COCOAアプリのビルド

### settings.json

`Covid19Radar`に`settings.json`を作成（`settings_template.json`をコピー）します。

```json
{
  "appVersion": "APP_VERSION",
  "apiSecret": "API_SECRET",
  "apiKey": "API_KEY",
  "supportedRegions": "440",
  "apiUrlBase": "https://API_URL_BASE/api",
  "cdnUrlBase": "https://CDN_URL_BASE/",
  "exposureConfigurationUrlBase": "https://CDN_URL_BASE/c19r",
  "blobStorageContainerName": "c19r",
  "logStorageEndpoint": "https://LOG_STORAGE_URL_BASE/",
  "logStorageContainerName": "LOG_STORAGE_CONTAINER_NAME",
  "logStorageAccountName": "LOG_STORAGE_ACCOUNT_NAME",
  "androidSafetyNetApiKey": "ANDROID_SAFETYNETKEY",
  "appStoreUrl": "https://itunes.apple.com/jp/app/id1516764458?mt=8",
  "googlePlayUrl": "https://play.google.com/store/apps/details?id=jp.go.mhlw.covid19radar",
  "supportEmail": "SUPPORT_EMAIL"
}
```

必要に応じて値を書き換えます。すべての値を正しく設定しなくてもビルドは可能です。
大文字のスネークケースで記述されている箇所（`APP_VERSION` など）はアプリ配布時のパイプラインで実際の値に書き換えます。

|キー|解説|
|---|---|
|appVersion|アプリのバージョン。CIでの差し替えを想定しているため、手動でのビルドの際は、AndroidManifest.xmlやInfo.plistを書き換える必要があります|
|apiSecret|サーバーのAzure Front Door用のシークレット|
|apiKey|サーバーのAzure Front Door用のキー|
|supportedRegions|接触通知APIが規定しているRegion。日本の場合は`440`|
|apiUrlBase|APIのエンドポイント（URL）の基礎となる値|
|cdnUrlBase|診断キーをダウンロードするCDNのURLの基礎となる値|
|exposureConfigurationUrlBase|設定ファイルのURLの基礎となる値|
|blobStorageContainerName|診断キーをダウンロードするBlobのコンテナ名|
|logStorageEndpoint|動作ログを送信するAzure Storageのエンドポイント|
|logStorageContainerName|動作ログを送信するAzure Storageのコンテナ名|
|logStorageAccountName|動作ログを送信するAzure Storageのアカウント名|
|androidSafetyNetApiKey|SafetyNet Attestation APIのキー|
|appStoreUrl|App StoreのURL|
|googlePlayUrl|Google Play StoreのURL|
|supportEmail|サポート用に表示するメールアドレス|


### ビルド構成
COCOAアプリのビルド構成は以下の通りです(Android/iOS共通)。  
接触確認APIの利用については、[接触確認APIの利用について](#接触確認apiの利用について)も参照してください。

|ビルド構成|サーバ通信|接触確認API|利用可能なメンバー|
|---|---|---|---|
|Debug|ON|ON|COCOA開発チーム|
|Debug_Mock|OFF(*)|OFF(*)|COCOA開発チーム, コミュニティメンバー|
|Release|ON|ON|COCOA開発チーム|
 
(*) モックAPIを利用

#### Debug
デバッグ用の構成です。サーバー通信と接触確認APIを利用します。
デバッグレベルのログを出力したり、AndroidのAndroidManifestの一部をデバッグ用に切り替えます。
切替内容に関する詳細は、[DEBUGマクロ](https://github.com/cocoa-mhlw/cocoa/search?q=%22if+DEBUG%22)を参照してください。  

COCOA開発チームがデバッグする際に利用します。コミュニティメンバーが利用することはできません。  

#### Debug_Mock
サーバーとの通信と接触確認APIについてはモック（Mock）が応答する構成です。  
上記以外はDebugと同じです。各種サーバーのURLを指定したり、接触確認APIを利用する権限(デバッグ用途)を保持している必要はありません。

コミュニティメンバーが主に利用する構成です。

#### Release
アプリリリース用の構成です。  

COCOA開発チームがアプリリリースする際に利用します。コミュニティメンバーが利用することはできません。  

#### デフォルトで有効でないシンボル

##### EN_DEBUG
ExposureNotificationをデバッグする目的のシンボルです。設定すると診断キーの重複チェックをスキップするほか、iOS版ではTEKを取得する際にTestモードで取得します。

切替内容に関する詳細は、[EN_DEBUGマクロ](https://github.com/cocoa-mhlw/cocoa/search?q=%22if+EN_DEBUG%22)を参照してください。

### ビルド＆実行手順
Debug_Mockを例にしてビルド手順を説明します。手順は色々あると思いますが、エディタの左上にある構成選択から適切な構成を選択して再生ボタン(▶︎)をクリックする方法を紹介します。

以下はVisual Studio 2019 for Macで選択する画面の例です。Windowsの場合は画面が異なりますが、基本的には同じです。

#### Androidアプリの場合
***[Covid19Radar.Android]>[Debug_Mock]>[Your_Device]*** を選択してビルドします。
![構成選択例(Android)](/cocoa/development/how_to_build//visualstudio-android.webp)

#### iOSアプリの場合
***[Covid19Radar.iOS]>[Debug_Mock]>[Your_Device]*** を選択してビルドします。  
iOSシミュレータを利用する場合、***[Debug_Mock|iPhoneSimulator]*** を選択します。
![構成選択例(iOS)](/cocoa/development/how_to_build//visualstudio-ios.webp)

### 注意事項
- iOS実機を利用する場合は、Apple Developersの登録が必要です。
- iOSシミュレータでSecure Storageを利用するには、Keychain を有効にする必要があります。詳細は[Xamarin.Essentials: Secure Storage](https://docs.microsoft.com/ja-jp/xamarin/essentials/secure-storage?tabs=ios#get-started)を参照してください。
- 正しい構成を選択しないと動作しません。例えば、***[Covid19Radar.Android]>[Debug_Mock|iPhoneSimulator]>[pixel3a...]*** のような構成を選択できますが当然ながら起動しません。

### ビルド失敗事例
これまでに、以下のビルド失敗に関する事例が報告されています。  

- https://github.com/cocoa-mhlw/cocoa/issues/40
- https://github.com/cocoa-mhlw/cocoa/issues/53
- https://github.com/cocoa-mhlw/cocoa/issues/173

### アプリケーション情報の設定

#### Android
Androidのアプリケーション情報は、[AndroidManifest.xml](https://github.com/cocoa-mhlw/cocoa/blob/master/Covid19Radar/Covid19Radar.Android/Properties/AndroidManifest.xml)で設定します。

 * ApplicationId: `manifest`タグの属性`package`
 * Version: `manifest`タグの属性`android:versionName`

#### iOS
iOSのアプリケーション情報は、[info.plist](https://github.com/cocoa-mhlw/cocoa/blob/master/Covid19Radar/Covid19Radar.iOS/Info.plist)で設定します。

 * Bundle ID: `CFBundleIdentifier`
 * Version: `CFBundleShortVersionString`

## COCOAサーバのビルド
COCOAサーバのビルド構成はDebug/Releaseの2通りです。

## 接触確認APIの利用について
アプリ開発で接触確認APIを利用するには、Goodle,Appleへ申請して許可を得る必要があります。  
申請については、Google,Appleともに公衆衛生機関のみ許可されています。詳細は以下を参照してください。

- [Google COVID-19 Exposure Notifications Service Additional Terms](https://blog.google/documents/72/Exposure_Notifications_Service_Additional_Terms.pdf/)
- [Apple Exposure Notification APIs Addendum](https://developer.apple.com/contact/request/download/Exposure_Notification_Addendum.pdf)

COCOA開発チームはAPIを利用するための申請を実施してアカウントやプロファイルを運用しています。
コミュニティメンバーはAPI利用許可を得ることができないため、DebugやRelease構成でビルドしても接触確認APIを利用できません。

### 利用手順
#### Androidの場合
ホワイトリストに登録されているGoogleアカウントを利用します。  
ホワイトリスト登録についてはGoogleへ申請します。

#### iOSの場合
Appleから承認を得たプロビジョニングプロファイルを利用します。
