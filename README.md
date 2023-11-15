# Ougon Trainer

A mod for the Steam version of Umineko: Golden Fantasia (also known as Ougon Musou Kyoku).

## Installing

1. Download the Reloaded-II Modding tool from [this link](https://github.com/Reloaded-Project/Reloaded-II/releases/latest) by clicking `Setup.exe` and running it
2. In the application's main window, click the little plus sigh on the left to add an application

![image](https://github.com/SkyLeite/Ougon/assets/16887983/a7f7dd6b-2ca3-4c5c-bfd7-d1bef1ed402d)

3. Navigate to Umineko: Golden Fantasia's directory (usually `C:/Program Files (x86)/Steam/steamapps/common/Umineko Golden Fantasia`) and select the file `Ougon.exe`

![image](https://github.com/SkyLeite/Ougon/assets/16887983/d27f86c6-8f70-4e43-a93a-45b610acb9d5)

4. Click the first icon on the left sidebar that says Download Mods
5. In the search bar, type `Ougon` and click the first result
6. Click the `Download` button on the bottom right corner
7. Once the download is finished, you should see yourself in the game's screen. If not, click Beatrice's face on the left sidebar.

![image](https://github.com/SkyLeite/Ougon/assets/16887983/27c65f81-ec99-4070-a852-4f28f88e367a)

8. If you see a minus (`-`) sign next to Ougon in the mods list, click it to make it a `+`
9. You're done! Click `Launch Application` to play. Note: you will have to launch the game through Reloaded-II every time you want to play with the mod enabled


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
* 🏁 Character grey health
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
* 🏁 Accessibility / training tool where attacks play specific sounds for easier practice of timing

### Online
* 🏁 Set delay and rollback cooldown on the fly

### Fun
* 🏁 Combo trials
* 🏁 Community-made tutorial
* 🏁 Jump into training mode from replay (I think Strive has something like this and it's apparently cool, need to look into it more)
* 🏁 Add more music from the VN
