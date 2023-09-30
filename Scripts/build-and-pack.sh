#!/bin/bash

UNITY_PATH="/Q/Programs/Unity/2021.3.30f1/Editor/Unity.exe"
PROJECT_PATH="$(pwd)"
BUILD_PATH_WIN="$PROJECT_PATH/Build/pogo"
BUILD_PATH_LINUX="$PROJECT_PATH/Build_Linux/pogo"
BUILD_NAME_LINUX="horde.x86_64"

LOG_PATH_WIN="$PROJECT_PATH/Build/build-cli-logs.txt"
LOG_PATH_LINUX="$PROJECT_PATH/Build_Linux/build-cli-logs.txt"

set -e

echo "Building Windows to $BUILD_PATH_WIN"
$UNITY_PATH -batchmode -buildTarget Win64 -configuration Release -buildLocation "$BUILD_PATH" - logfile -
