#!/bin/sh

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

git clean -xdf --exclude=packages

find . -type d -name 'obj' | xargs rm -rf
find . -type d -name 'bin' | xargs rm -rf

