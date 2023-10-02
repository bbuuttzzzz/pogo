#!/bin/bash
WINTAR="/c/Windows/System32/tar.exe"

set -e
set -x

echo "Packing Win64"
cd Build/win64
$WINTAR -a -cvf pogo_win64.zip \
    --exclude pogo/pogo_BurstDebugInformation_DoNotShip \
    *

cd -
mv Build/win64/pogo_win64.zip Build/

echo "Packing Win32"
cd Build/win32
$WINTAR -a -cvf pogo_win32.zip \
    --exclude pogo/pogo_BurstDebugInformation_DoNotShip \
    *

cd -
mv Build/win32/pogo_win32.zip Build/

echo "Packing Linux64"
cd Build/linux64
$WINTAR -a -cvf pogo_linux64.zip \
    --exclude pogo/pogox86_BurstDebugInformation_DoNotShip \
    *
cd -
mv Build/linux64/pogo_linux64.zip Build/