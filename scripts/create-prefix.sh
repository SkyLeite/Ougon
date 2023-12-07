#!/usr/bin/env bash

. "$PWD/scripts/wine-environment.sh"

# Command
wineboot

echo "---> Installing dependencies"
winetricks -q d3dx9_43 vcrun2022 dotnet48 atmlib corefonts gdiplus msxml3 msxml6 vcrun2008 vcrun2010 vcrun2012 fontsmooth-rgb amstream quartz lavfilters

echo "---> Enabling wine virtual desktop"
winetricks vd=1920x1080

if [[ ! -d "$RELOADED_DATA_PATH" ]]; then
    echo "---> Reloaded-II is not installed. Installing..."

    RELOADED_SETUP_PATH="$WINEPREFIX/drive_c/users/$USER/Temp/Setup.exe"
    curl -L https://github.com/Reloaded-Project/Reloaded-II/releases/download/1.25.1/Setup.exe -o "$RELOADED_SETUP_PATH"

    wine "$RELOADED_DATA_PATH"
fi

if [[ ! -d "$X64DBG_PATH" ]]; then
    echo "---> x64dbg is not installed. Installing..."

    X64DBG_SETUP_PATH="$WINEPREFIX/drive_c/users/$USER/Temp/x64dbg.zip"
    curl -L https://github.com/x64dbg/x64dbg/releases/download/snapshot/snapshot_2023-12-04_17-51.zip -o "$X64DBG_SETUP_PATH"

    unzip "$X64DBG_SETUP_PATH" "release/**/*" -d "$PROGRAM_FILES"
    mv "$PROGRAM_FILES/release" "$X64DBG_PATH"
fi
