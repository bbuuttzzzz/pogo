#!/bin/bash

YLW='\033[1;33m'
RED='\033[1;31m'
GRN='\033[0;32m'
NC='\033[0m' # No Color

FILE_NAME='ProjectSettings/ProjectSettings.asset'
VERSION_MATCH='bundleVersion: \(.*\)'


display_usage () {
    echo "Usage: get-version.sh [OPTION]..."
}
#parse positional args
POSITIONAL_ARGS=()

while [[ $# -gt 0 ]]; do
    case $1 in
        -*|--*)
            echo "Unknown option $1"
	        display_usage
            exit 1
            ;;
        *)
            POSITIONAL_ARGS+=("$1") # save positional arg
            shift # past argument
            ;;
    esac
done

set -- "${POSITIONAL_ARGS[@]}" # restore positional parameters

set -e

VERSION_REPLACE_PATTERN="s/$VERSION_MATCH/\1/p"

sed -n "$VERSION_REPLACE_PATTERN" $FILE_NAME