#!/bin/bash

set -e

source .env

POGOWIN64_PATH=Build/Itch/win64
POGOLINUX_PATH=Build/Itch/linux
POGOWEB_PATH=Build/Itch/web/pogo

$ITCHBUTLER_PATH push $POGOWIN64_PATH hedgewizards/pogo3d:win64-free --ignore pogo_BurstDebugInformationDoNotShip*
$ITCHBUTLER_PATH push $POGOLINUX_PATH hedgewizards/pogo3d:linux-free --ignore pogo.86_BurstDebugInformationDoNotShip*
$ITCHBUTLER_PATH push $POGOWEB_PATH hedgewizards/pogo3d:web-free
