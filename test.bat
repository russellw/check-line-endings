MSBuild.exe check-line-endings.sln /p:Configuration=Debug /p:Platform="Any CPU"
if errorlevel 1 goto :eof
bin\Debug\check-line-endings %*
