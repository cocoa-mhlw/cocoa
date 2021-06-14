#!/bin/bash

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

# リポジトリ内のファイルについて MPL の記載漏れをチェックします。
# 詳しい処理につきましては Tools/GitHooks/PreCommit.sh をご参照ください。

# `addLicense` の読み込み
source Tools/GitHooks/PreCommit.sh

# 対象ファイル
FILES=(
  '*.axml'
  '*.bat'
  '*.cs'
  '*.feature'
  '*.resx'
  '*.sh'
  '*.storyboard'
  '*.strings'
  '*.tf'
  '*.xaml'
  '*.xlf'
  '*.xml'
  '*.yml'
)

# 対象ファイルの内，MPL が抜けているもの対して文言を挿入する
for file in "${FILES[@]}"; do
  addLicense "$file"
  git add "$file"
done
