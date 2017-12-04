set project_name=MsieJavaScriptEngine
set project_source_dir=..\src\%project_name%
set project_bin_dir=%project_source_dir%\bin\Release
set licenses_dir=..\Licenses
set dotnet_cli="%ProgramFiles%\dotnet\dotnet.exe"
set nuget_package_manager=..\.nuget\nuget.exe

rmdir lib /Q/S

del sass-and-coffee-license.txt /Q/S
del chakra-host-license.txt /Q/S
del jsrt-dotnet-license.txt /Q/S
del cross-browser-split-license.txt /Q/S
del bundler-and-minifier-license.txt /Q/S

%dotnet_cli% restore "%project_source_dir%"

%dotnet_cli% build "%project_source_dir%" --framework net40-client --configuration Release --no-dependencies --no-incremental
xcopy "%project_bin_dir%\net40-client\%project_name%.dll" lib\net40-client\
xcopy "%project_bin_dir%\net40-client\%project_name%.xml" lib\net40-client\
xcopy "%project_bin_dir%\net40-client\ru-ru\%project_name%.resources.dll" lib\net40-client\ru-ru\

%dotnet_cli% build "%project_source_dir%" --framework net45 --configuration Release --no-dependencies --no-incremental
xcopy "%project_bin_dir%\net45\%project_name%.dll" lib\net45\
xcopy "%project_bin_dir%\net45\%project_name%.xml" lib\net45\
xcopy "%project_bin_dir%\net45\ru-ru\%project_name%.resources.dll" lib\net45\ru-ru\

%dotnet_cli% build "%project_source_dir%" --framework netstandard1.3 --configuration Release --no-dependencies --no-incremental
xcopy "%project_bin_dir%\netstandard1.3\%project_name%.dll" lib\netstandard1.3\
xcopy "%project_bin_dir%\netstandard1.3\%project_name%.xml" lib\netstandard1.3\
xcopy "%project_bin_dir%\netstandard1.3\ru-ru\%project_name%.resources.dll" lib\netstandard1.3\ru-ru\

copy "%licenses_dir%\sass-and-coffee-license.txt" sass-and-coffee-license.txt /Y
copy "%licenses_dir%\chakra-host-license.txt" chakra-host-license.txt /Y
copy "%licenses_dir%\jsrt-dotnet-license.txt" jsrt-dotnet-license.txt /Y
copy "%licenses_dir%\cross-browser-split-license.txt" cross-browser-split-license.txt /Y
copy "%licenses_dir%\bundler-and-minifier-license.txt" bundler-and-minifier-license.txt /Y

%nuget_package_manager% pack "%project_name%.nuspec"