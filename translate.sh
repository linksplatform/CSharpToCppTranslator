#!/bin/bash

TARGET_EXTENSION="${2:-'h'}"

echo "started ${1}.${TARGET_EXTENSION}"
dotnet run -c Release "${1}.cs" "${1}.${TARGET_EXTENSION}"
echo "done ${1}.${TARGET_EXTENSION}"