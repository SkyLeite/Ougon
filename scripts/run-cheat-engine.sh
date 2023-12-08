#!/usr/bin/env bash

. "$PWD/scripts/wine-environment.sh"

# Working Directory
cd "$X64DBG_PATH" || exit 1

# Command
wine "$PROGRAM_FILES/Cheat Engine 7.5/Cheat Engine.exe"
