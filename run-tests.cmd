@echo off
setlocal

set common_test_project_name=MsieJavaScriptEngine.Test.Common
set auto_test_project_name=MsieJavaScriptEngine.Test.Auto
set chakra_edge_jsrt_test_project_name=MsieJavaScriptEngine.Test.ChakraEdgeJsRt
set chakra_ie_jsrt_test_project_name=MsieJavaScriptEngine.Test.ChakraIeJsRt
set chakra_activescript_test_project_name=MsieJavaScriptEngine.Test.ChakraActiveScript
set classic_test_project_name=MsieJavaScriptEngine.Test.Classic

set common-args=%*
set test_dir_path=test
set common_test_project_file_path=%test_dir_path%\%common_test_project_name%\%common_test_project_name%.csproj
set auto_test_project_file_path=%test_dir_path%\%auto_test_project_name%\%auto_test_project_name%.csproj
set chakra_edge_jsrt_test_project_file_path=%test_dir_path%\%chakra_edge_jsrt_test_project_name%\%chakra_edge_jsrt_test_project_name%.csproj
set chakra_ie_jsrt_test_project_file_path=%test_dir_path%\%chakra_ie_jsrt_test_project_name%\%chakra_ie_jsrt_test_project_name%.csproj
set chakra_activescript_test_project_file_path=%test_dir_path%\%chakra_activescript_test_project_name%\%chakra_activescript_test_project_name%.csproj
set classic_test_project_file_path=%test_dir_path%\%classic_test_project_name%\%classic_test_project_name%.csproj

@echo Run unit tests...
@echo.

dotnet test %common-args% "%common_test_project_file_path%"
@echo.

dotnet test %common-args% "%auto_test_project_file_path%"
@echo.

dotnet test %common-args% "%chakra_edge_jsrt_test_project_file_path%"
@echo.

dotnet test %common-args% "%chakra_ie_jsrt_test_project_file_path%"
@echo.

dotnet test %common-args% "%chakra_activescript_test_project_file_path%"
@echo.

dotnet test %common-args% "%classic_test_project_file_path%"
@echo.