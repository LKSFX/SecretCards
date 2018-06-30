#!/bin/bash
 
# 1. Set up these constants
# 2. Launch the Shell script with: "sh push_apk_all_devices.sh" in a terminal window
 
APK_PATH='SecretCards.apk'
BUNDLE_ID='com.lksfx.secretcards'
MAIN_ACTIVITY='com.unity3d.player.UnityPlayerActivity'
 
 
echo "APK_PATH: $APK_PATH"
echo "BUNDLE_ID: $BUNDLE_ID"
echo "MAIN_ACTIVITY: $MAIN_ACTIVITY"
 
install_to_device() {
  local prettyName=$(adb -s $1 shell getprop ro.product.model)
  echo "
  Starting Installatroning on $prettyName"
  adb -s $1 install -r "${APK_PATH}"
  adb -s $1 shell am start -n "${BUNDLE_ID}/${MAIN_ACTIVITY}"
  adb -s $1 shell input keyevent KEYCODE_WAKEUP
  echo "------> Installatron completed on $prettyName"
}
echo "----------------------- INSTALLATION-------------------------"
 
adb devices -l
 
for SERIAL in $(adb devices | tail -n +2 | cut -sf 1);
do 
  install_to_device $SERIAL&
done
 
exit