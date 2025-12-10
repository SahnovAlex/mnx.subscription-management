@echo off
SET host=https://nuget.minux-test-infra.ru
SET NUGET-SERVER-API-KEY=5f34a5492f5a4d65aad62a91822df079953c9723364b4fb3

cd /d %~dp0\..

call :PushPackage "src\MNX.SubscriptionManagement.Infrastructure.Bus.Contracts" "MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.csproj"

pause
exit /b

:: Function to check out the version and publish the package
:PushPackage
setlocal
set projectPath=%1
set csprojFile=%2
set version=

for /f "delims=" %%i in ('powershell -Command "[xml]$csproj = Get-Content \"%projectPath%\\%csprojFile%\"; $csproj.Project.PropertyGroup.Version"') do set version=%%i

set version=%version: =%

echo Version extracted: %version%

if exist %projectPath%\bin\Release (
    cd %projectPath%\bin\Release
) else (
    echo Error: Folder %projectPath%\bin\Release does not exists!
)

dotnet nuget push -s %host%/v3/index.json -k %NUGET-SERVER-API-KEY% --skip-duplicate %csprojFile:~0,-8%.%version%.nupkg
endlocal
goto :eof