---
title: "設定ファイルの仕様"
weight: 20
type: docs
---

### v1.3.0 - 
 * 設定項目はAndroid/iOSプラットフォーム固有のKey-value storeに保存する（`IPreferencesService`）
 * 接触情報はJSON形式に変換後、`SecureStorageService`を通じて暗号化された状態で格納されている
 * 日付情報はUNIX Epoch（e.g. `1592528400`）

#### 保存内容

##### IPreferencesService

|  キー  |  形式  | 保存内容 |
| ---- | ---- | ---- |
|  AppVersion  |  文字列  | Preferenceが対応しているアプリのバージョン（e.g. `1.3.0`） |
|  StartDateTimeEpoch  |  数値（long）  | 利用開始日時のUNIX Epoch（e.g. `1592528400`） |
|  TermsOfServiceLastUpdateDateTimeEpoch  |  数値（long）  | 利用規約に合意した日時のUNIX Epoch（e.g. `1592528400`） |
|  PrivacyPolicyLastUpdateDateTimeEpoch  |  数値（long）  | プライバシーポリシーに合意した日時のUNIX Epoch（e.g. `1592528400`） |
|  ExposureNotificationConfiguration  |  文字列  | JSON形式に変換された`Xamarin.ExposureNotifications.Configuration` |
|  LastProcessTekTimestamp  |  文字列  | JSON形式に変換された`Dictionary<string, long>` キーはRegion（e.g. `440`）、数値はTEKのタイムスタンプ（e.g. `1632551138`） |
|  CanConfirmExposure  |  Boolean  | 最後に試行した接触確認（診断キーのダウンロード・接触確認実行）が正常に完了したことを示すフラグ |
|  LastConfirmedDateTimeEpoch  |  数値（long）  | 最後に接触確認を完了した日時のUNIX Epoch（e.g. `1592528400`） |

##### ISecureStorageService

|  キー  |  形式  | 保存内容 |
| ---- | ---- | ---- |
|  ExposureSummary  |  文字列  | JSON形式に変換された`UserDataModel.ExposureSummary` |
|  ExposureInformation  |  文字列  | JSON形式に変換された`ObservableCollection<UserExposureInfo>` |


### v1.2.2 - v1.2.6
 * Application.PropertiesからPreferenceServiceへのマイグレーション
 * 設定項目はAndroid/iOSプラットフォーム固有のKey-value storeに保存する（`IPreferencesService`）
 * 接触情報はJSON形式に変換後、`SecureStorageService`を通じて暗号化された状態で格納されている
 * 日付情報はDateTimeをToString()した結果（e.g. `2020/06/19 10:00:00`）
 * 確認されている不具合
     * 日付情報の文字列変換時にフォーマットを指定していないため、日時を保存後に端末に設定されている地域や言語が変わった場合、ただしく日付が復元できなくなることがある
     * v1.1.5より前のバージョンで利用規約に合意していた（`UserDataModel.IsOptined`がtrue）場合、`TermsOfServiceLastUpdateDateTime`の値に `0001/01/01 00:00:00`が設定される。これは厳密には不具合ではなく仕様
     * v1.1.5より前のバージョンでプライバシーポリシーに合意していた（`UserDataModel.IsPolicyAccepted`がtrue）ていて、v1.2.0でプライバシーポリシーの再同意を経ていない場合、`PrivacyPolicyLastUpdateDateTime`の値に `0001/01/01 00:00:00`が設定される。これは厳密には不具合ではなく仕様
 
#### 保存内容

##### IPreferencesService

|  キー  |  形式  | 保存内容 |
| ---- | ---- | ---- |
|  StartDateTime  |  文字列  | 利用開始日時（UTC） - `2020/06/19 10:00:00` |
|  TermsOfServiceLastUpdateDateTime  |  文字列  | 利用規約に合意した日時（JST） - `2020/06/19 10:00:00` |
|  PrivacyPolicyLastUpdateDateTime  |  文字列  | プライバシーポリシーに合意した日時（JST） - `2020/06/19 10:00:00` |
|  LastProcessTekTimestamp  |  文字列  | JSON形式に変換された`Dictionary<string, long>` キーはRegion（e.g. `440`）、数値はTEKのタイムスタンプ（e.g. `1632551138`） |

##### ISecureStorageService

|  キー  |  形式  | 保存内容 |
| ---- | ---- | ---- |
|  ExposureSummary  |  文字列  | JSON形式に変換された`UserDataModel.ExposureSummary` |
|  ExposureInformation  |  文字列  | JSON形式に変換された`ObservableCollection<UserExposureInfo>` |

### v1.2.0 - v1.2.1

 * Application.Propertiesに保存（`IApplicationPropertyService`）
    * Xamarin.Forms固有のファイル永続化方式
    * 実体ファイルはBinary XML形式
 * 設定項目と接触情報は [UserDataModel](https://github.com/cocoa-mhlw/cocoa/blob/develop/Covid19Radar/Covid19Radar/Model/UserDataModel.cs) を、JSON形式に変換して保存されている
 * 日付情報はDateTimeをToString()した結果（e.g. `2020/06/19 10:00:00`）
 * 確認されている不具合
     * Application.Propertiesの特性上、複数スレッドから書き込みをするとデータが壊れる事象が発生
     * Androidのバックグラウンド処理時に設定の読み書きができない事象が発生
       * 仕様上、Xamarin.Formsが初期化されていないタイミングではApplication.Propertiesにアクセスできない
     * 日付情報の文字列変換時にフォーマットを指定していないため、日時を保存後に端末に設定されている地域や言語が変わった場合、ただしく日付が復元できなくなることがある
     * v1.1.5より前のバージョンで利用規約に合意していた（`UserDataModel.IsOptined`がtrue）場合、`TermsOfServiceLastUpdateDateTime`のキーは作成されない。これは厳密には不具合ではなく仕様
       * v1.2.0からv1.2.1が運用されている期間内に、利用規約の再同意プロセスが利用されたことがないため

#### 保存内容

|  キー  |  形式  | 保存内容 |
| ---- | ---- | ---- |
|  UserData  |  文字列  | JSON形式に変換されたUserDataModel |
|  TermsOfServiceLastUpdateDateTime  |  文字列  | 利用規約に合意した日時（JST） - `2020/06/19 10:00:00` |
|  PrivacyPolicyLastUpdateDateTime  |  文字列  | プライバシーポリシーに合意した日時（JST） - `2020/06/19 10:00:00` |

```
public class UserDataModel
{
    public DateTime StartDateTime { get; set; }
    public bool IsOptined { get; set; } = false;
    public bool IsPolicyAccepted { get; set; } = false;
    public Dictionary<string, long> LastProcessTekTimestamp { get; set; } = new Dictionary<string, long>();
    public ObservableCollection<UserExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<UserExposureInfo>();
    public UserExposureSummary ExposureSummary { get; set; }
}
```

### v1.0.0 - v1.1.5

 * Application.Propertiesに保存（`IApplicationPropertyService`）
    * Xamarin.Forms固有のファイル永続化方式
    * 実体ファイルはBinary XML形式
 * 設定項目と接触情報は [UserDataModel](https://github.com/cocoa-mhlw/cocoa/blob/develop/Covid19Radar/Covid19Radar/Model/UserDataModel.cs) を、JSON形式に変換して保存されている
 * 日付情報はDateTimeをToString()した結果（e.g. `2020/06/19 10:00:00`）
 * 確認されている不具合
     * Application.Propertiesの特性上、複数スレッドから書き込みをするとデータが壊れる事象が発生
     * Androidのバックグラウンド処理時に設定の読み書きができない事象が発生
       * 仕様上、Xamarin.Formsが初期化されていないタイミングではApplication.Propertiesにアクセスできない
     * 日付情報の文字列変換時にフォーマットを指定していないため、日時を保存後に端末に設定されている地域や言語が変わった場合、ただしく日付が復元できなくなることがある

#### 保存内容

|  キー  |  形式  | 保存内容 |
| ---- | ---- | ---- |
|  UserData  |  文字列  | JSON形式に変換されたUserDataModel |

```
public class UserDataModel
{
    public DateTime StartDateTime { get; set; }
    public bool IsOptined { get; set; } = false;
    public bool IsPolicyAccepted { get; set; } = false;
    public Dictionary<string, long> LastProcessTekTimestamp { get; set; } = new Dictionary<string, long>();
    public ObservableCollection<UserExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<UserExposureInfo>();
    public UserExposureSummary ExposureSummary { get; set; }
}
```
