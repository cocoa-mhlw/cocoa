# Android版アプリのテスターの方へ

「COVID-19 Radar」アプリのテスターにご参加いただきありがとうございます。  
本ドキュメントでは、テスターに向けてテストリリースのインストール方法をご紹介します。

## 前提条件
以下の手続きが完了している事を前提とします。
- [ベータテスターへの応募](https://bit.ly/2XSuVUJ) 
- ベータテスターへ応募した際のメールアドレスにて、  
Microsoftアカウント、もしくはGitHubアカウントの作成

## ドキュメント作成環境
本ドキュメントでは、以下の環境にてスクリーンショットの取得・動作確認を行っております。  
テスターの皆様の環境に応じて、適宜読み替えをお願いします。
- KYOCERA URBANO V02 KYV34
- Android 5.1

## 1. 「提供元不明のアプリ」のインストールの許可
Androidデバイスにおいて、Google Playストア以外からのアプリ（提供元不明のアプリ）のインストールを許可します。  
「設定」アプリより、「セキュリティ」内に、「提供元不明のアプリ」の設定がありますので、「許可」に変更します。

![Change Device Security](../.attachments/Android_003_DeviceSecurity.png)


## 2. App Center にサインイン

[App Center](https://appcenter.ms/sign-in) に、Android端末 でアクセスします。  
QRコードスキャナアプリより、以下のQRコードをスキャンしてもアクセス可能です。  
![App Center QRCode](../.attachments/appcenter-qrcode.png)

AppCenterのサインインページより、テスター登録時にFormに入力したメールアドレスでサインインをしてください。  

![App Centerにサインイン](../.attachments/Android_005_appcenter_signin.png)


## 3. アプリのダウンロード

My Appsの画面にAndroid版とiOS版のアプリが表示されています。
「COVID-19 Radar」のAndroidアプリをクリックします。
![App Centerでアプリをダウンロード](../.attachments/Android_006_appcenter_selectapps.png)


ダウンロードページへのQRコードが表示されますので、下部のリンク（赤枠部）をタップします。
![App Centerでアプリをダウンロード](../.attachments/Android_007_appcenter_installpage-qr)


現在リリースされているバージョンの一覧が表示されていますので、最上部の「Latest release」の「DOWNLOAD」ボタンをタップします。

![ブラウザからのインストール許可](../.attachments/Android_008_appcenter_intallpage.png)

ブラウザ（Google Chrome)からのダウンロード許可を求めるポップアップが表示されます。

![App Centerでアプリをダウンロード](../.attachments/Android_009_appcenter_downloading_apk.png)

ポップアップの指示に従って「許可」してください。

## 4. アプリのインストール

APKファイルのダウンロードが完了しましたら、インストールを行います。  
ストレージにAPKファイルが保存されているので、パッケージインストーラにて開きます。
![APKのインストール](../.attachments/Android_011_installing_apk.png)

アプリケーションのインストールについて、確認がされますので、画面に従い「次へ」をタップします。
![APKのインストール](../.attachments/Android_012_installing_apk.png)


無事にインストールが完了すると、以下の表示になります。
![APKのインストール](../.attachments/Android_014_installed_apk.png)

ランチャー内に「Covid19Radar」アプリが表示されていますので、こちらをご利用ください。
![ランチャー](../.attachments/Android_015_installed_apk.png)


以上で、「Covid19Radar」のAndroidアプリをインストールすることができます。

## バグを発見した場合
バグの報告、および新機能の追加の提案をされたい場合、GitHub上リポジトリにて、Issueの作成をお願いします。
詳細は、リポジトリ内の「HOW_TO_CONTRIBUTE」をご覧ください。
[Covid19Radar(GitHub) - HOW_TO_CONTRIBUTE.md](https://github.com/Covid-19Radar/Covid19Radar/blob/master/HOW_TO_CONTRIBUTE.md)

## 相談をしたい場合
無償のチャットアプリケーション(Discord)にて、コミュニケーションを行っております。  
下記招待リンクより参加をお願いします。
[Covid19Radar(Discord)](https://discord.gg/EzaYhD)


-----

「COVID-19 Radar」は現在開発中のアプリです。  
コミットする都度、最新版の通知がメールアドレスに来ますので、最新版を都度ダウンロードいただきますよう、よろしくお願いいたします。