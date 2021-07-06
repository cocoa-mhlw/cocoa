#!/bin/bash

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

# Commit 時に MPL の記載漏れをチェックします。
# あらかじめ Tools/GitHooks/PrepareHooks.sh を実行する必要があります。
# 詳しい処理につきましては Tools/AddLicense/AddLicense.sh をご参照ください。

# リポジトリのルートへ移動
cd "$(git rev-parse --show-toplevel)" || exit 1

# `addLicense` の読み込み
source Tools/AddLicense/AddLicense.sh

# ステージングエリアのファイル一覧を取得（削除を除く）
stagedFiles=$(git diff --name-only --cached --diff-filter=d)

# 対象ファイルの内，MPL が抜けているもの対して文言を挿入する
for file in $stagedFiles; do
  addLicense "$file" && git add "$file"
done
