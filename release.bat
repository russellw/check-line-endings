set version=1

MSBuild.exe check-line-endings.sln /p:Configuration=Release /p:Platform="Any CPU"
if errorlevel 1 goto :eof

md check-line-endings-%version%
copy App.config check-line-endings-%version%
copy LICENSE check-line-endings-%version%
copy bin\Release\check-line-endings.exe check-line-endings-%version%

del check-line-endings-%version%.zip
7z a check-line-endings-%version%.zip check-line-endings-%version%
7z l check-line-endings-%version%.zip

rd /q /s check-line-endings-%version%
