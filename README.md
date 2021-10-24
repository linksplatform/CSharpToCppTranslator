[![Actions Status](https://github.com/linksplatform/csharptocpptranslator/workflows/CI/badge.svg)](https://github.com/linksplatform/csharptocpptranslator/actions?workflow=CI)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/7a113c49b6124cbb8464b64ae2595878)](https://www.codacy.com/manual/drakonard/CSharpToCppTranslator?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=linksplatform/CSharpToCppTranslator&amp;utm_campaign=Badge_Grade)
[![CodeFactor](https://www.codefactor.io/repository/github/linksplatform/csharptocpptranslator/badge)](https://www.codefactor.io/repository/github/linksplatform/csharptocpptranslator)

# CSharpToCppTranslator
A specific translator for LinksPlatform's libraries.

## Example
`translate.sh` script is useful when Translator fails to translate the whole directory in one go.

Translate `.cs` to `.h`:
```sh
export BASE_PATH=/home/konard/Archive/Code/Links/Data.Doublets/csharp/Platform.Data.Doublets/Memory/United/Specific/; ls -p "$BASE_PATH" | grep -v / | grep -v .csproj | sed -e "s|csharp|cpp|" | sed -e 's/\.cs$//' | sed -e "s|^|${BASE_PATH}|" | xargs -n1 ./translate.sh "h"
```

Translate `.cs` to `.cpp`:
```sh
export BASE_PATH=/home/konard/Archive/Code/Links/Data.Doublets/csharp/Platform.Data.Doublets/Memory/United/Specific/; ls -p "$BASE_PATH" | grep -v / | grep -v .csproj | sed -e "s|csharp|cpp|" | sed -e 's/\.cs$//' | sed -e "s|^|${BASE_PATH}|" | xargs -n1 ./translate.sh "cpp"
```