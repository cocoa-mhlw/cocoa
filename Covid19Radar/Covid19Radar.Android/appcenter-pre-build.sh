#!/usr/bin/env bash

if [ -z "$ANDROID_APP_ID" ]
then
    echo "You need define the ANDROID_APP_ID variable in App Center"
    exit
fi

APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/Covid19Radar/Covid19Radar/Common/AppConstants.cs

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating app ids to $ANDROID_APP_ID and $IOS_APP_ID in AppConstant.cs"
    sed -i '' 's#AppCenterTokensAndroid = "[-A-Za-z0-9:_./]*"#AppCenterTokensAndroid = "'$ANDROID_APP_ID'"#' $APP_CONSTANT_FILE

    echo "File content:"
    cat $APP_CONSTANT_FILE
fi
