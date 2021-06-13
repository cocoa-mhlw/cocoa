#!/bin/bash

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

function addLicense() {
  local MPL=(
    'This Source Code Form is subject to the terms of the Mozilla Public'
    'License, v. 2.0. If a copy of the MPL was not distributed with this'
    'file, You can obtain one at https://mozilla.org/MPL/2.0/.'
  )
  local comment=''
  local lineNumber=1

  # 各種ファイルに適したコメントを生成する処理
  if [ "$1" == '*.axml' ]; then
    comment='\\n<!--\n'$(printf '  %s\\n' "${MPL[@]}")'-->\n'
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$1" == '*.bat' ]; then
    comment=$(printf '@REM %s\\n' "${MPL[@]}")

  elif [ "$1" == '*.cs' ]; then
    comment=$(printf '// %s\\n' "${MPL[@]}")

  elif [ "$1" == '*.feature' ]; then
    comment=$(printf '# %s\\n' "${MPL[@]}")

  elif [ "$1" == '*.resx' ]; then
    comment='\\n<!--\n'$(printf '  %s\\n' "${MPL[@]}")'-->\n'
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$1" == '*.sh' ]; then
    comment='\\n'$(printf '# %s\\n' "${MPL[@]}")
    lineNumber=2 # Shebang の後に挿入する

  elif [ "$1" == '*.storyboard' ]; then
    comment='\\n<!--\n'$(printf '  %s\\n' "${MPL[@]}")'-->\n'
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$1" == '*.strings' ]; then
    comment='\\n/*\n'$(printf '  %s\\n' "${MPL[@]}")'*/\n'

  elif [ "$1" == '*.tf' ]; then
    comment=$(printf '# %s\\n' "${MPL[@]}")

  elif [ "$1" == '*.xaml' ]; then
    comment='\\n<!--\n'$(printf '  %s\\n' "${MPL[@]}")'-->\n'
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$1" == '*.xlf' ]; then
    comment='\\n<!--\n'$(printf '  %s\\n' "${MPL[@]}")'-->\n'
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$1" == '*.xml' ]; then
    comment='\\n<!--\n'$(printf '  %s\\n' "${MPL[@]}")'-->\n'
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$1" == '*.yml' ]; then
    comment=$(printf '# %s\\n' "${MPL[@]}")
  fi

  # ライセンスの文言が無いファイルを探し，もしあれば生成したコメントを挿入する
  git grep -z -L "${MPL[1]}" -- "$1" | xargs -0 sed -i -e "${lineNumber}i$comment"
}

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

for item in "${FILES[@]}"; do
  addLicense "$item"
done
