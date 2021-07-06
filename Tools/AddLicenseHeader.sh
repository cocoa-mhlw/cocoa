#!/bin/bash
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

dotnet script `dirname $0`/AddLicenseHeader/AddLicenseHeader.csx `dirname $0`/../

# https://github.com/cocoa-mhlw/cocoa/pull/237 を取り込んだ場合は下記の処理に変更する。

#cd `dirname $0`/../
#`dirname $0`/AddLicenseHeader/AddLicenseHeader.sh
