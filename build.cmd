@echo off

setlocal
set ORIGINAL_CURRENT_DIR=%cd%
set KOREBUILD_DOTNET_CHANNEL=rel-1.0.0
set KOREBUILD_DOTNET_VERSION=1.0.1

cd %~dp0

set local_nuget_package_manager=.nuget\NuGet.exe
set package_dir=packages

if not exist %package_dir%\NUnit.Runners (
	%local_nuget_package_manager% install NUnit.Runners -Version 3.4.1 -O %package_dir% -ExcludeVersion -NoCache
)

PowerShell -NoProfile -NoLogo -ExecutionPolicy unrestricted -Command "[System.Threading.Thread]::CurrentThread.CurrentCulture = ''; [System.Threading.Thread]::CurrentThread.CurrentUICulture = '';& '%~dp0build.ps1' %*; exit $LASTEXITCODE"