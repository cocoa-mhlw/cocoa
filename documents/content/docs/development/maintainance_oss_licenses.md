---
title: "ライセンスの取り扱い"
weight: 30
type: docs
---

# ライセンスの取り扱い

## COCOAのライセンス
COCOAは、オープンソースライセンスに「Mozilla Public License 2.0（MPL 2.0）」を採用しています。

https://www.mozilla.org/en-US/MPL/2.0/

MPL 2.0では、各ソースコードに次のライセンス通知を表示することを規定しています。

```
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
```

ライセンス通知はプログラム・コードに加えて、XMLファイルなどにも追加する必要があります。


## 使用しているOSSライブラリに変更があったとき

COCOAが使用しているOSSのライセンス情報はリポジトリのルートにある`COPYRIGHT_THIRD_PARTY_SOFTWARE_NOTICES.md`に集約します。

ライセンスに更新があった場合、ファイル`COPYRIGHT_THIRD_PARTY_SOFTWARE_NOTICES.md`をHTMLに変換したものを`license.html`として、プラットフォームそれぞれ次の場所に配置します。

 * Android
   * `/Covid19Radar/Covid19Radar.Android/Assets` ディレクトリ
 * iOS
   * `Covid19Radar/Covid19Radar.iOS/Resources` ディレクトリ 

現状、ライセンスファイルの変換はGitHub Flavorであればよく、手動で行っています。これはライセンスの変更が週一程度の頻度では起こらないと考えているためです。ライセンスの編集が頻繁に発生するようであれば（それは開発工程を考え直した上で）、CIでの変換を検討します。
