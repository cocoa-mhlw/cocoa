#!/bin/bash

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

# pre-commit 用のスクリプトを追加
cp Tools/GitHooks/PreCommit.sh .git/hooks/pre-commit
chmod a+x .git/hooks/pre-commit

# 追加した Git Hooks を有効にする
git init
