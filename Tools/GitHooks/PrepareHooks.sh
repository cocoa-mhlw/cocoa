#!/bin/bash

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

# リポジトリのルートへ移動
cd "$(git rev-parse --show-toplevel)" || exit 1

# pre-commit 用のスクリプトを追加
install -m a+x -D Tools/GitHooks/PreCommit.sh .git/hooks/pre-commit

# 追加した Git Hooks を有効にする
git init
