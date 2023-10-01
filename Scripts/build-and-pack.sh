#!/bin/bash
UNITY_PATH="/Q/Programs/Unity/2021.3.30f1/Editor/Unity.exe"
PROJECT_PATH="$(pwd)"
LOG_FILE=$PROJECT_PATH/Build/CoolLogs.txt

set -e

touch $LOG_FILE

echo "Launching Unity..."
$UNITY_PATH -quit -batchmode -executeMethod "Pogo.Building.GameBuilder.BuildAll" -pathRoot "$PROJECT_PATH/Build" -shortLogs $LOG_FILE -logfile $PROJECT_PATH/Build/full-build-logs.txt &
mypid=$!
tail -f -n +0 $LOG_FILE &
wait $mypid
kill $mypid