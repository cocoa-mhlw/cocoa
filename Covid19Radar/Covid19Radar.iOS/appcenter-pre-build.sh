#!/usr/bin/env bash

echo "Arguments for updating:"

# Updating ids

APP_CONSTANT_FILE=$BUILD_REPOSITORY_LOCALPATH/Covid19Radar/Covid19Radar/settings.json

sed -i '' "s/API_SECRET/$API_SECRET/g" $APP_CONSTANT_FILE
sed -i '' "s/API_URL_BASE/$API_URL_BASE/g" $APP_CONSTANT_FILE
sed -i '' "s/ANDROID_SAFETYNETKEY/$ANDROID_SAFETYNETKEY/g" $APP_CONSTANT_FILE
sed -i '' "s/CDN_URL_BASE/$CDN_URL_BASE/g" $APP_CONSTANT_FILE
sed -i '' "s/APPCENTER_ANDROID/$APPCENTER_ANDROID/g" $APP_CONSTANT_FILE
sed -i '' "s/APPCENTER_IOS/$APPCENTER_IOS/g" $APP_CONSTANT_FILE

# Print out file for reference
cat $APP_CONSTANT_FILE
echo "Updated id!"

# Donwload Runtime
curl -L -O https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-6/29c4ea73109b377a71866c53a6d43033d5c5e90b/49/package/notarized/xamarin.ios-13.18.2.1.pkg
curl -L -O https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-6/29c4ea73109b377a71866c53a6d43033d5c5e90b/49/package/notarized/xamarin.mac-6.18.2.1.pkg

# Install Runtime
sudo installer -store -pkg "xamarin.ios-13.18.2.1.pkg" -target /
sudo installer -store -pkg "xamarin.mac-6.18.2.1.pkg" -target /
