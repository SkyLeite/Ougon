#!/usr/bin/env bash

. "$PWD/scripts/wine-environment.sh"

# Working Directory
cd "$X64DBG_PATH" || exit 1

# Command
wine "$X64DBG_PATH/x32/x32dbg.exe" "$GAME_PATH"
