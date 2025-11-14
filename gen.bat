set WORKSPACE=%~dp0

set GEN_CLIENT=%WORKSPACE%Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%DataTables

dotnet %GEN_CLIENT% ^
    -t client ^
    -c cs-simple-json ^
    -d json  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=Assets/Gen/code ^
    -x outputDataDir=Assets/Gen/json ^
    -x pathValidator.rootDir=D:/TestProject/MyProject/ ^