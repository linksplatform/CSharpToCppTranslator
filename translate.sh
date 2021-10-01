#!/bin/bash
echo "started ${1}.h"
dotnet run -c Release "${1}.cs" "${1}.h"
echo "done ${1}.h"