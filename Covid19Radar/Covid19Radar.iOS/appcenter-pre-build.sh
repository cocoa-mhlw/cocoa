#!/bin/bash
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

echo "Arguments for updating:"

APP_PLIST_FILE=$BUILD_REPOSITORY_LOCALPATH/Covid19Radar/Covid19Radar.iOS/Info.plist
sed -i '' "s/APP_PACKAGE_NAME/$APP_PACKAGE_NAME/g" $APP_PLIST_FILE
sed -i '' "s/APP_VERSION/$APP_VERSION/g" $APP_PLIST_FILE
cat $APP_PLIST_FILE

#ENTITLEMENTS_PLIST_FILE=$BUILD_REPOSITORY_LOCALPATH/Covid19Radar/Covid19Radar.iOS/Entitlements.plist
#sed -i '' "s/ENTITLEMENTS_PLIST_VALUE/$ENTITLEMENTS_PLIST_VALUE/g" $ENTITLEMENTS_PLIST_FILE
#cat $ENTITLEMENTS_PLIST_FILE

# Updating ids

APP_CONSTANT_FILE=$BUILD_REPOSITORY_LOCALPATH/Covid19Radar/Covid19Radar/settings.json

sed -i '' "s/APP_VERSION/$APP_VERSION/g" $APP_CONSTANT_FILE
sed -i '' "s/API_SECRET/$API_SECRET/g" $APP_CONSTANT_FILE
sed -i '' "s/API_KEY/$API_KEY/g" $APP_CONSTANT_FILE
sed -i '' "s/API_URL_BASE/$API_URL_BASE/g" $APP_CONSTANT_FILE
sed -i '' "s/ANDROID_SAFETYNETKEY/$ANDROID_SAFETYNETKEY/g" $APP_CONSTANT_FILE
sed -i '' "s/CDN_URL_BASE/$CDN_URL_BASE/g" $APP_CONSTANT_FILE
sed -i '' "s/SUPPORT_EMAIL/$SUPPORT_EMAIL/g" $APP_CONSTANT_FILE
sed -i '' "s/LOG_STORAGE_URL_BASE/$LOG_STORAGE_URL_BASE/g" $APP_CONSTANT_FILE
sed -i '' "s/LOG_STORAGE_CONTAINER_NAME/$LOG_STORAGE_CONTAINER_NAME/g" $APP_CONSTANT_FILE
sed -i '' "s/LOG_STORAGE_ACCOUNT_NAME/$LOG_STORAGE_ACCOUNT_NAME/g" $APP_CONSTANT_FILE

# Print out file for reference
cat $APP_CONSTANT_FILE
echo "Updated id!"

# Download Runtime
curl -L -O https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-6/29c4ea73109b377a71866c53a6d43033d5c5e90b/49/package/notarized/xamarin.ios-13.18.2.1.pkg
curl -L -O https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-6/29c4ea73109b377a71866c53a6d43033d5c5e90b/49/package/notarized/xamarin.mac-6.18.2.1.pkg

# Install Runtime
sudo installer -store -pkg "xamarin.ios-13.18.2.1.pkg" -target /
sudo installer -store -pkg "xamarin.mac-6.18.2.1.pkg" -target /
