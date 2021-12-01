---
title: "v1.4.0リリースで発生した障害について"
weight: 1
type: docs
url: /report/accident-v1.4.0.html
---

# v1.4.0リリースで発生した障害について

## 障害の内容
v1.1.5以前にインストールをして利用規約に合意した状態からv1.4.0にアップデートするとアプリが起動できなくなる。

### 発生日時
2021年11月25日午後3時

### 復旧日時
iOS: 2021年11月26日午前10時47分

Android: 2021年11月27日午前11時40分

### 発生した事象
v1.4.0にアップデート後、COCOAが起動不能になる。

### 影響範囲

#### プラットフォーム・OSバージョン
Android, iOSのすべてのOSバージョン

#### 台数
2,100万台（推定される最大値）

##### 算出根拠
v1.2.0がリリースされる前（v1.0.0 - v1.1.5）にダウンロードされたすべてに影響したと考える。アンインストール数については全数を把握できないため考慮しない。

[【接触確認アプリ】ダウンロード数・陽性登録件数 推移](https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/cocoa_00138.html)を元に
"R2.11.30（2,084）"から"R2.12.28（2,245）"に線形に推移したと仮定して、v1.2.0リリースのあった"R2.12.03"時点のダウンロード数を推計した。


#### 影響を受けた機能
 * UIを伴うすべての操作（陽性情報登録を含む）
 * バックグラウンドでの診断キーのダウンロードと接触確認

#### 影響を受けなかった機能
 * Bluetooth受発信による接触記録（接触確認APIの機能）

## 発生から復旧までのタイムライン
 * 11/25
     * 15:00 - v1.4.0リリース（Google Play, AppStore）
         * 開発チーム iOS端末3台、Android端末2台で公開されたアプリv1.4.0が起動できることを確認
     * 16:17 - GitHub [Issue #517 「COCOA v1.4.0 が起動しない」](https://github.com/cocoa-mhlw/cocoa/issues/517)投稿
     * 16:20 - 開発チーム 対応を開始
     * 16:49 - GitHub クラッシュレポート（iOS）共有
     * 17:11 - GitHub クラッシュレポート（Android）共有
     * 17:30 - GitHub 推測される原因が「MinValue/MaxValue に加算減算が定義されていない」との指摘がGitHubに投稿される
         * 開発チーム GitHubの投稿内容で調査を進める
     * 17:46 - 開発チーム 障害をデバッグモードで再現することに成功
     * 18:35 - 開発チーム Hotfixリリース準備開始
     * 18:35 - GitHub [修正（Hotfix）コード](https://github.com/cocoa-mhlw/cocoa/pull/518)提出
     * 18:55 - 開発チーム 修正（Hotfix）コードで障害が解決することを確認
     * 19:19 - 開発チーム 修正（Hotfix）コード取り込み
     * 20:12 - 開発チーム 利用者からの問い合わせ状況を共有
     * 20:13 - 開発チーム v1.1.5以前にアプリの利用を開始すると利用規約の同意日付がDateTime最小値のままとなることが原因と判明
         * 開発チーム 障害の再現方法を確定
     * 20:39 - 開発チーム リリーステストの作成開始
     * 22:40 - 開発チーム リリーステストの作成完了
     * 23:16 - 開発チーム リリーステストの実施完了
         * 開発チーム 準備したv1.4.0がクラッシュする端末のアプリを製品版アプリv1.4.1へアップデート、正常起動し問題が解消していることを確認
 * 11/26
     * 00:28 - 開発チーム v1.4.1 審査提出完了（Google Play, AppStore）
     * 10:10 - 開発チーム v1.4.1 AppStoreでリリース
         * 開発チーム iOS端末7台で公開されたアプリv1.4.1が起動できることを確認
     * 10:47 - GitHub iOS版の復旧報告
 * 11/27
     * 11:28 - 開発チーム v1.4.1 Google Playでリリース開始
     * 11:40 - GitHub Android版の復旧報告
         * 開発チーム Android端末1台で公開されたアプリv1.4.1が起動できることを確認

## 原因
　本障害に至った原因として、次の3点が挙げられる。

 * タイムゾーン変換（JSTからUTCへ）方法の不適切な選択
 * 言語仕様への理解が不十分なまま実装をしたこと
 * 過去バージョンの仕様の調査が不足していたこと

### タイムゾーン変換（JSTからUTCへ）方法の不適切な選択
　タイムゾーンを日本標準時（JST）から協定世界時（UTC）に変換する際に、時差の9時間を直接減算するという方法は不適切であった（[該当コード](https://github.com/cocoa-mhlw/cocoa/blob/release_1_4_0/Covid19Radar/Covid19Radar/Services/Migration/Migrator_1_3_0.cs)）。

```
namespace Covid19Radar.Services.Migration
{
    internal class Migrator_1_3_0
    {
        private readonly TimeSpan TIME_DIFFERENCIAL_JST_UTC = TimeSpan.FromHours(+9);

        private void MigrateDateTimeToEpoch(string dateTimeKey, string epochKey, TimeSpan differential)
        {
            string dateTimeStr = _preferencesService.GetValue(dateTimeKey, DateTime.UtcNow.ToString());

            DateTime dateTime;
            try
            {
                dateTime = DateTime.SpecifyKind(DateTime.Parse(dateTimeStr) + differential, DateTimeKind.Utc);
            }
            catch (FormatException exception)
            {
                _loggerService.Exception($"Parse dateTime FormatException occurred. {dateTimeStr}", exception);
                dateTime = DateTime.UtcNow;
            }
            _preferencesService.SetValue(epochKey, dateTime.ToUnixEpoch());
            _preferencesService.RemoveValue(dateTimeKey);
        }
    }
}
```

　タイムゾーンの補正はシステムやプラットフォームが用意している標準関数`TimeZoneInfo.ConvertTimeToUtc`を使用するのが適切であり、後述する`FindSystemTimeZoneById`の問題については、懸念があることを開発チーム・テストチームに共有することで十分な動作テストをしてカバーすべき問題であった。

#### [参考]実装に当たっての検討の課程
　参考として、時差を直接減算するという判断に至るまでの検討の過程を記す。

　前提として、次の仕様のデータを取り扱う必要があった。

 * 使用開始日や規約への合意日時が文字列型で保存されている
 * 日時を表す文字列にタイムゾーンが指定されていない
 * 使用開始日はUTCとして保存されているが、規約合意日時はJST

##### DateTime.ToUniversalTime()の利用
　当該部分を実装する過程で、まずはじめに`DateTime.ToUniversalTime()`の利用を検討した。

　しかし、この場合、採用される時差はDateTimeに含まれる日時情報の種別（`DateTime.Kind`）に基づく。また、`DateTime.Kind`に指定可能なのは`Unspecified`, `Utc`そして`Local`の3種類で、明示的にタイムゾーンを指定できるものではない。

　`Kind`を`Local`とした場合、端末に設定されているタイムゾーンに基づく変換となる。端末に必ずしも日本のタイムゾーンが設定されているという保証はないため、この方法ではJSTからUTCの明示的な変換はできないと判断した。

##### TimeZoneInfo.ConvertTimeToUtcの利用
　また、タイムゾーンを明示して補正する`TimeZoneInfo.ConvertTimeToUtc`についても利用を検討した。

　これは明示的にJSTのタイムゾーンを取得した上で、`DateTime`をUTCに変換できることが確認できた。
一方で、タイムゾーンを取得する際に指定する名前（文字列）がプラットフォーム依存であるが採用を忌避する要因となった。

[C#] UTCからJST、JSTからUTCへ変換する
https://www.ipentec.com/document/csharp-convert-utc-jst

　前述の記事では`TimeZoneInfo jstZoneInfo = System.TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");`でタイムゾーンを取得している。また、Microsoftが公開している公式ドキュメントでも同様に、タイムゾーンの名前に`Tokyo Standard Time`を指定している。

TimeZoneInfo.FindSystemTimeZoneById(String) メソッド
https://docs.microsoft.com/ja-jp/dotnet/api/system.timezoneinfo.findsystemtimezonebyid?view=net-6.0

　しかしながら、Xamarinでは名前では日本標準時のタイムゾーンは取得できず、`TimeZoneNotFoundException`が発生した。
調査の結果、.NETでは、ソフトウェアが動作するプラットフォームWindowsとLinuxで取り扱いが異なり、Xamarinの場合、Linuxに準拠した`Asia/Tokyo`を指定する必要があることがわかった。

.NET CoreでWindowsとLinuxでタイムゾーンを識別するID表記が異なるという話
https://tech.tanaka733.net/entry/2020/02/timezone-id

　調査の結果から、現在、端末のロケールの違いでさまざまな不具合を抱えているCOCOAが、解決のためにプラットフォームごとに挙動が違う（違うかもしれない）方法を使うのはリスクがあると判断した。日本国において時差が変わると言うことは滅多にあることではないので、COCOAの想定される稼働期間に発生する可能性は極めて低く、日時情報から9時間を減算するもっともシンプルな実装を採用した。

### 言語仕様への理解が不十分なまま実装をしたこと
　実装時に、C#で`new DateTime()`が`DateTime.MinValue`となることを認識できていなかった。
また、`DateTime.MinValue`（`0001/01/01 00:00:00`）から減算をすると`ArgumentOutOfRangeException`が発生することを認識できていなかった。

　これら言語仕様への理解が不十分なまま実装していた。

### 過去バージョンの仕様の調査が不足していたこと
　実装に際して、当該コードを実装する時点で、設定ファイルの形式に変更が加わったのはv1.2.2, v1.2.6 そしてv1.3.0（v1.4.0として配信）の「3つのタイミング」と考えていた。

　しかし、障害発生後に調査した結果、設定ファイルの形式に変更が加わったのは **v1.2.0**, v1.2.2, v1.2.6 そしてv1.3.0（v1.4.0として配信）の 「4つのタイミング」 だったことがわかった。

　マイグレーションは、すべての過去バージョンを取り扱う（または取り扱わないバージョンを定義する）必要から、仕様の調査については細心の注意を払うべきである。また、開発に関与するメンバーが入れ替わっていく中で、過去仕様に対する理解が共通化できるよう取り組む必要がある。

## 復旧方法
　原因を回避するコードを追加してHotfix版（v1.4.1）をリリース。

## 復旧後の状況
　任意のバージョンからv1.4.1へアップデートすることで障害が発生していないこと。また、v1.4.0にアップデートすることで障害が発生した端末についてもv1.4.1にアップデートすることで障害が解決することを確認している。

## 当時の対応について

### 正しく対応できたこと
 * GitHubに投稿されたIssueを把握して、迅速に対応を開始できた
 * GitHubコミュニティと連携して、迅速に障害の原因を特定できた
 * Hotfixを作成と障害の再現方法を調査・確定を並列で行うなど、効率的に作業することができた
 * Hotfixの影響する範囲の確認とリリースに必要なテストケースの作成、実施までを円滑に進めることができた

### 課題
 * リリース日のTwitterなどSNSへの観測がリリース後オペレーションとして組み込まれていなかった。
 * 障害対応フロー自体は定まっていたが、GitHubコミュニティとも連携したHotfix対応について、障害対応時の開発チーム・連携チーム横断で役割分担が定まっていなかった
 * Android/iOSともに端末を対象としたリリース（100%リリース）を行ったため、障害発生を把握してからリリースを中止することが容易ではなく、Hotfixリリースを優先することになった。

## 今後の対応

### 再発防止策（案）
　再発防止策として、次の項目を検討している。

 * ローリングアップデートを実施することで、リリースを中断できるようにする
 * リリース予定日・リリース状況（ローリングアップデート状況）をGitHubおよびSNSで公表して、必要に応じて関係各所と連携した情報発信をできるようにする
 * GitHubコミュニティへBeta版を配布することで、より多くの端末でテストできるようにする
    * COCOA2では接触確認APIのバージョンアップが含まれ、多くの課題の発見が想定される
 * ユニットテストを整備する
    * コードカバレッジを計測して数値としてテストの行き届いていないところを可視化する
 * リリース時期（ローリングアップデート時含む）には、SNSの観測をオペレーションとして行う。
 * 今後もGitHubコミュニティと連携したPRベースでの開発は行っていくため、本件を踏まえて障害対応手順をアップグレードする。


#### 再発防止策として取り上げないこと
 　ダブルチェック、トリプルチェックなど、現行のコードレビューに加えて介在する人間の数を増やす方法は再発防止策に含めない。ヒューマンエラーが必ず発生するという前提で、仕組みで解決するのが適切と考える。

## 参考
 * GitHubのIssueにまとめた障害と実装に関する情報
    * https://github.com/cocoa-mhlw/cocoa/issues/517#issuecomment-979325741
    * https://github.com/cocoa-mhlw/cocoa/issues/517#issuecomment-979676626
 * [C#] UTCからJST、JSTからUTCへ変換する
    * https://www.ipentec.com/document/csharp-convert-utc-jst
 * TimeZoneInfo.FindSystemTimeZoneById(String) メソッド
    * https://docs.microsoft.com/ja-jp/dotnet/api/system.timezoneinfo.findsystemtimezonebyid?view=net-6.0
 * .NET CoreでWindowsとLinuxでタイムゾーンを識別するID表記が異なるという話
    * https://tech.tanaka733.net/entry/2020/02/timezone-id
