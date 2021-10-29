#!/bin/bash

if [ ! -z "${3}" ]
then

  SOURCE="${3}.cs"
  TARGET_EXTENSION="${1:-'h'}"
  TARGET_PATH=$(echo "${3}" | sed -e "${2}")
  TARGET="${TARGET_PATH}.${TARGET_EXTENSION}"

  echo "started $SOURCE"
  dotnet run -c Release "$SOURCE" "$TARGET"
  echo "done as $TARGET"

fi