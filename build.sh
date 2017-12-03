#!/usr/bin/env bash
ORIGINAL_CURRENT_DIR=%cd%
KOREBUILD_DOTNET_CHANNEL=rel-1.0.0
KOREBUILD_DOTNET_VERSION=1.1.5

repoFolder="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $repoFolder

localNugetPackageManager=.nuget/NuGet.exe
packageDir=packages

if test ! -d $packageDir/NUnit.Console; then
  mono $localNugetPackageManager install NUnit.Console -Version 3.7.0 -O $packageDir% -ExcludeVersion -NoCache
fi

koreBuildZip="https://github.com/aspnet/KoreBuild/archive/02bd945d32558d24c1e5c6b74e37d44585ad9691.zip"
if [ ! -z $KOREBUILD_ZIP ]; then
    koreBuildZip=$KOREBUILD_ZIP
fi

buildFolder=".build"
buildFile="$buildFolder/KoreBuild.sh"

if test ! -d $buildFolder; then
    echo "Downloading KoreBuild from $koreBuildZip"

    tempFolder="/tmp/KoreBuild-$(uuidgen)"
    mkdir $tempFolder

    localZipFile="$tempFolder/korebuild.zip"

    retries=6
    until (wget -O $localZipFile $koreBuildZip 2>/dev/null || curl -o $localZipFile --location $koreBuildZip 2>/dev/null)
    do
        echo "Failed to download '$koreBuildZip'"
        if [ "$retries" -le 0 ]; then
            exit 1
        fi
        retries=$((retries - 1))
        echo "Waiting 10 seconds before retrying. Retries left: $retries"
        sleep 10s
    done

    unzip -q -d $tempFolder $localZipFile

    mkdir $buildFolder
    cp -r $tempFolder/**/build/** $buildFolder

    chmod +x $buildFile

    # Cleanup
    if test ! -d $tempFolder; then
        rm -rf $tempFolder
    fi
fi

$buildFile -r $repoFolder "$@"