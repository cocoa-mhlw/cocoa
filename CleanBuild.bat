# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

git clean -xdf --exclude=packages

for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"
pause