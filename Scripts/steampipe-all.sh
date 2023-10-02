#!/bin/bash

set -e

source .env
pogo3d_vdf=$(realpath Scripts/vdf/pogo3d.vdf)

$STEAMCMD_PATH +login $steampipe_user \
    +run_app_build $pogo3d_vdf \
    +quit