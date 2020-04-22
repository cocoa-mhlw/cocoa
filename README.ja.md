# Covid19Radar (Japanese)

*このアプリケーションは開発中*です。

iOS Builstatus [![iOS Build status](https://build.appcenter.ms/v0.1/apps/9c268337-4db9-4bf4-be09-efaf16672c15/branches/master/badge)](https://appcenter.ms)

Android Build status [![Android Build status](https://build.appcenter.ms/v0.1/apps/3dcdf5b5-da95-4d03-96a6-e6ed42de7e16/branches/master/badge)](https://appcenter.ms)

本アプリは、Bluetooth(BLE/iBeacon)を利用して、お互いの接触ログを取得します。
![アプリの概念](img/AppDescription.jpg)

現時点では、Android同士のビーコン取得と画面遷移のみができており、以下の機能の実装が行われていません。
コントリビューションとプルリクエストをお待ちしています。

## コントリビューションについて
コントリビューションをお考えいただき、誠にありがとうございます。
現在、私たちはコアコントリビューターと共に、2週間程度の間、チケット管理も含め、集中スプリント体制を展開しています。
リポジトリは現在Azure DevOpsからGithubのMasterにPush同期さて、Azure DevOpsで集中管理しています。
もし、あなたがコアコントリビューターとして活動をされたい場合は、Discordで私に話しかけてください。

## テスト用にアプリをインストールするには

以下のリンクからアプリをテスト用にインストールしてください:（現時点では Android のみ対応しています）

https://install.appcenter.ms/orgs/Covid19Radar/apps/Covid19RadarAndroid/releases

テスト用のデバイスの構成は、以下のドキュメントを参照してください:
https://docs.microsoft.com/ja-jp/appcenter/distribution/testers/testing-android

## 実装済
- 画面遷移とデザイン
- Android同士での近接通信の検知と取得
 
## 未実装
- iOS iBeacon実装と検証 (私がMac持ってない)
- ユーザ追加(via REST API)
- Push Nortification
- サーバサイド API (Azure Function and CosmosDB)
- 濃厚接触の条件などの整理
- Graph DB(Cosmos DB)の表示処理など（サーバサイド)

### 開発環境について

クライアント側は、Xamarin Forms(iOS and Android) with C# と Prism(MVVM IoC)を使っています。Visual Studio for Windows もしくは Visual Studio for Macで開発可能です。

https://visualstudio.microsoft.com/ja/xamarin/

![アプリ設定に関して](img/design00.jpg)

デバイスの以下の機能の利用許可が必須となります。 

1. Bluetooth
2. Push通知

設定完了後、本アプリをインストールしている人同士の接触ログを自動で記録します。

# デザインについて

[Adobe XD](https://www.adobe.com/jp/products/xd.html)を利用してデザイン制作を行っています。

![画面全体図](img/design01.jpg)

デザインファイルを確認する場合は、Adobe XDをインストールしてください。（無料で利用可能）

アプリケーションのより詳細な仕様とデータモデル、API仕様、関連業務知識に関しては、[濃厚接触検知のデータモデルと API仕様](doc/domain-model.md)をごらんください。


## アプリのプロトタイプ

以下のURLにアクセスすると、画面遷移を確認いただけます。

[プロトタイプ画面（日本語）](https://xd.adobe.com/view/f60f0c48-af7b-48cb-42c3-e74e64d07020-803e/?fullscreen)

Password：Covid19Radar

## ライセンス
Covid19Radar is licensed under the GNU Affero General Public License v3.0. See
[LICENSE](./LICENSE) for the full
license text.
