@echo off

setlocal
set ORIGINAL_CURRENT_DIR=%cd%
set KOREBUILD_DOTNET_CHANNEL=rel-1.0.0
set KOREBUILD_DOTNET_VERSION=1.1.5

cd %~dp0

set local_nuget_package_manager=.nuget\NuGet.exe
set package_dir=packages

if not exist %package_dir%\NUnit.Console (
	%local_nuget_package_manager% install NUnit.Console -Version 3.7.0 -O %package_dir% -ExcludeVersion -NoCache
)

PowerShell -NoProfile -NoLogo -ExecutionPolicy unrestricted -Command "[System.Threading.Thread]::CurrentThread.CurrentCulture = ''; [System.Threading.Thread]::CurrentThread.CurrentUICulture = '';& '%~dp0build.ps1' %*; exit $LASTEXITCODE"