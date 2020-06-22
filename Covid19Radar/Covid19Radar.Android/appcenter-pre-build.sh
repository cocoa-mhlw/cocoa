#!/usr/bin/env bash

echo "Arguments for updating:"

APP_MANITEST_FILE=$BUILD_REPOSITORY_LOCALPATH/Covid19Radar/Covid19Radar.Android/Properties/AndroidManifest.xml
sed -i '' "s/APP_PACKAGE_NAME/$APP_PACKAGE_NAME/g" $APP_MANITEST_FILE
sed -i '' "s/android:debuggable=\"true\"//g" $APP_MANITEST_FILE
cat $APP_MANITEST_FILE

# Updating ids

APP_CONSTANT_FILE=$BUILD_REPOSITORY_LOCALPATH/Covid19Radar/Covid19Radar/settings.json

sed -i '' "s/API_SECRET/$API_SECRET/g" $APP_CONSTANT_FILE
sed -i '' "s/API_URL_BASE/$API_URL_BASE/g" $APP_CONSTANT_FILE
sed -i '' "s/ANDROID_SAFETYNETKEY/$ANDROID_SAFETYNETKEY/g" $APP_CONSTANT_FILE
sed -i '' "s/CDN_URL_BASE/$CDN_URL_BASE/g" $APP_CONSTANT_FILE
sed -i '' "s/SUPPORT_EMAIL/$SUPPORT_EMAIL/g" $APP_CONSTANT_FILE

# Print out file for reference
cat $APP_CONSTANT_FILE

echo "Updated id!"