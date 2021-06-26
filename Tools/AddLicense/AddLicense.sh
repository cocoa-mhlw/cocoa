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
  local fileType=${1##*.}
  local lineNumber=1

  # 各種ファイルに適したコメントを生成する
  if [ "$fileType" == 'axml' ]; then
    comment='\\n'$(printf '<!-- %s\\n - %s\\n - %s -->\\n' "${MPL[@]}")
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$fileType" == 'bat' ]; then
    comment=$(printf '@REM %s\\n' "${MPL[@]}")

  elif [ "$fileType" == 'cs' ]; then
    comment=$(printf '/* %s\\n * %s\\n * %s */\\n' "${MPL[@]}")

  elif [ "$fileType" == 'feature' ]; then
    comment=$(printf '# %s\\n' "${MPL[@]}")

  elif [ "$fileType" == 'resx' ]; then
    comment='\\n'$(printf '<!-- %s\\n - %s\\n - %s -->\\n' "${MPL[@]}")
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$fileType" == 'sh' ]; then
    comment='\\n'$(printf '# %s\\n' "${MPL[@]}")
    lineNumber=2 # Shebang の後に挿入する

  elif [ "$fileType" == 'storyboard' ]; then
    comment='\\n'$(printf '<!-- %s\\n - %s\\n - %s -->\\n' "${MPL[@]}")
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$fileType" == 'strings' ]; then
    comment=$(printf '/* %s\\n * %s\\n * %s */\\n' "${MPL[@]}")

  elif [ "$fileType" == 'tf' ]; then
    comment=$(printf '# %s\\n' "${MPL[@]}")

  elif [ "$fileType" == 'xaml' ]; then
    comment='\\n'$(printf '<!-- %s\\n - %s\\n - %s -->\\n' "${MPL[@]}")
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$fileType" == 'xml' ]; then
    comment='\\n'$(printf '<!-- %s\\n - %s\\n - %s -->\\n' "${MPL[@]}")
    lineNumber=2 # XML 宣言の後に挿入する

  elif [ "$fileType" == 'yml' ]; then
    comment=$(printf '# %s\\n' "${MPL[@]}")
  fi

  # MPL の適用対象であるかを確認する。その中でライセンスの文言がないものに限り，生成したコメントを挿入する
  if [ "$(grep -c "$1" Tools/AddLicense/Exclusions.txt)" -eq 0 ]; then
    git grep -z -I -L "${MPL[0]}" -- "$1" | xargs -0 -r sed -i -e "${lineNumber}i$comment"
  else
    echo "Excluded: $1"
  fi
}
