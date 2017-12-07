@echo off
setlocal

set local_nuget_package_manager=.nuget\NuGet.exe
set package_dir=packages
set nunit_console_path=%package_dir%\NUnit.ConsoleRunner\tools\nunit3-console.exe

if not exist %package_dir%\NUnit.Console (
	%local_nuget_package_manager% install NUnit.Console -Version 3.7.0 -O %package_dir% -ExcludeVersion -NoCache
)

@echo Run unit tests for .NET 4.0 version...
@echo.
%nunit_console_path% test\MsieJavaScriptEngine.Test.Net4.nunit --process=Multiple --domain=Single --work=test\ --noheader --trace=Off