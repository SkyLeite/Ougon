# Ougon Trainer

A mod for the Steam version of Umineko: Golden Fantasia (also known as Ougon Musou Kyoku).

## Features

The following is a list of my goals with this mod, in no particular order. Some of them might not even be possible, so don't hold your breath.

✔️ - Done

🏁 - Planned

❎ - Canceled

### Misc
* ✔️ ImGui-based in-game overlay
* 🏁 Load move data from a file, allowing users to create and share modifications for rebalancing
* 🏁 Add video settings in-game
* 🏁 Port 2.31's netcode to Steam, or at least improve it
  * Not sure how they differ yet

### Bugfixes
* ✔️ Fix running at the incorrect speed in higher refresh rate monitors by implementing an FPS limiter (configurable, default 60)
* 🏁 Fix grey health disappearing when a single HP bar is lost
  * This might not be a glitch. Instead, the glitch might be that when you lose the exact amount of a single bar of HP, grey health is not lost (thanks Hexer)
* 🏁 Input system seems to be janky on keybord, particularly related to diagonal inputs
* 🏁 Fix not being able to bind to R2/L2
* 🏁 Fix controller hotplug
* 🏁 Fix training mode reset erasing recorded inputs for the dummy
* 🏁 Fix fullscreen not working in some setups
  * Particularly Hexer's, with an RTX2060. No idea why this could be happening, will need more debug information

### Debug menu
* ✔️ Character health (read + write)
* ✔️ Character meter (read + write)
* ✔️ Character stun value (read + write)
* 🏁 Character X and Y position (read + write)
* 🏁 Character name
* 🏁 Character move visualizer
* 🏁 Move's properties and values (like damage, knockback type, etc) (read + write)

### Training mode
* 🏁 In-game real-time hitbox visualizer
* 🏁 In-game real-time frame data visualizer
* 🏁 Combo damage
* 🏁 Combo scaling visualizer
* 🏁 Change characters in real time
* 🏁 Change character palette in real time

### Online
* 🏁 Set delay and rollback cooldown on the fly

### Fun
* 🏁 Combo trials
* 🏁 Community-made tutorial
* 🏁 Jump into training mode from replay (I think Strive has something like this and it's apparently cool, need to look into it more)
