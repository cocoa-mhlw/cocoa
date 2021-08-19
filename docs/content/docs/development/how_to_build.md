---
title: "ビルド方法"
weight: 1
draft: false
---

# ビルド方法

_文体は後で調整する。何を書くかざっくり記述していく。_

## ビルド構成（Build Configuration）
COCOAのビルド構成は次の通り。

 * Debug
 * Debug_Mock
 * Release

### Debug
デバッグ用の構成。サーバー通信と接触確認APIを利用する。
[デバッグレベルのログを出力したり、AndroidのAndroidManifestの一部がデバッグ用に切り替わる。](https://github.com/cocoa-mhlw/cocoa/search?q=%22if+DEBUG%22)

### Debug_Mock
Debugに加えて、サーバーとの通信と接触確認APIについてはモック（Mock）が応答する構成。
各種サーバーのURLを指定することなく、また後述する接触確認APIをデバッグ目的で利用する権限がなくても動作する。

### Release
リリース用の構成。

## アプリケーションの情報の設定

### Android
Androidのアプリケーション情報は、[AndroidManifest.xml](https://github.com/cocoa-mhlw/cocoa/blob/master/Covid19Radar/Covid19Radar.Android/Properties/AndroidManifest.xml)で設定する。

 * ApplicationId: `manifest`タグの属性`package`
 * Version: `manifest`タグの属性`android:versionName`

### iOS
iOSのアプリケーション情報は、[info.plist](https://github.com/cocoa-mhlw/cocoa/blob/master/Covid19Radar/Covid19Radar.iOS/Info.plist)で設定する。

 * Bundle ID: `CFBundleIdentifier`
 * Version: `CFBundleShortVersionString`

## 接触確認APIを利用したデバッグについて
DebugやReleaseのConfigurationでビルドしても、接触確認APIにはアクセスできない。

Androidの場合は、ホワイトリストに登録されているGoogleアカウントが必要。ホワイトリストへの登録にはGoogleへ申請する。

iOSの場合は、Appleから承認を得たプロビジョニングプロファイルが必要。

申請はGoogle, Appleともに公衆衛生機関のみ許可される。
ホワイトリストに登録されたアカウントやプロビジョニングプロファイルは、開発チームが運用している（これはどの国も同じ状況という認識）。
