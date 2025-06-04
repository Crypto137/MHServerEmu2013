# MHServerEmu2013

MHServerEmu2013 is a server emulator for the classic version of Marvel Heroes. If you are looking for a server emulator for later versions of the game, see [MHServerEmu](https://github.com/Crypto137/MHServerEmu).

The primary supported version of the game client is **1.10.0.643** released on July 18, 2013. The server can also be built for game version **1.10.0.69** released on May 31, 2013 by specifying the `BUILD_1_10_0_69` conditional compilation symbol.

## FAQ

**How is this different from MHServerEmu?**

This is a side project I have been working on to make the launch version of Marvel Heroes from 2013 playable again. Because of the many differences between it and the more modern versions of the game, I believe it is cleaner to maintain a separate codebase and backport individual features on a case-by-case basis for this.

**Is it fully playable?**

No, and I expect it is going to be a while before it happens. There are various technical challenges in supporting version 1.10 specifically:

- There is no Mac version to get debug symbols from. The earliest Mac build we have access to is version 1.25, which is over a year newer than 1.10.

- We do not have any packet captures for 1.10.

- 1.10 uses certain legacy features, like behavior tree AI controllers, that were removed in later versions of the game.

Despite that, I have made a significant amount of progress, and the current set of features puts MHServerEmu2013 at about the same level as version 0.2.0 of the original MHServerEmu. In other words, you can explore the procedurally generated world and activate powers, all with limited multiplayer support, but most of the combat related functionality does not work yet.

**What about version 1.48 (the last pre-BUE patch)?**

Supporting version 1.48 is still the priority, and it will be implemented based on the primary version of MHServerEmu.

MHServerEmu2013 is more of an experiment born out of my personal nostalgia and curiosity to figure out if supporting version 1.10 is even viable.

## Setup Instructions

These instructions assume you already have an MHServerEmu setup of some kind. If not, see [Initial Setup](https://github.com/Crypto137/MHServerEmu/blob/master/docs/Setup/InitialSetup.md).

1. Download the latest nightly build of MHServerEmu2013 or build the source code yourself.

2. Acquire a copy of the 1.10 client. You can download older versions of the game client, including 1.10, **if you have Marvel Heroes in your Steam library**.
   
   1. Open the Steam console by entering `steam://open/console` in your web browser's address bar.
   
   2. Enter the following command in the Steam console: `download_depot 226320 226321 2968810361455133183`. You should see the following message in the console: `Downloading depot 226321 (36326 files, 10375 MB) ...`. There will be no download progress indicator, but when the download finishes, a `Depot download complete` message will be printed to the Steam console.
   
   3. This will download version 1.10.0.643 to the following directory by default: `C:\Program Files (x86)\Steam\steamapps\content\app_226320\depot_226321`. The exact directory depends on where you have Steam installed. We suggest to move the game client to some other directory that is more convenient to access after the download is finished.

3. Game version 1.10 uses the legacy game data archive format. You need to convert it to use with MHServerEmu2013.
   
   1. Download the latest version of [MHSqlitePakRepacker](https://github.com/Crypto137/MHSqlitePakRepacker/releases).
   
   2. Drag and drop the `mu_cdata.sip` file located in `Marvel Heroes\UnrealEngine3\Binaries\Win32\Data` onto `MHSqlitePakRepacker.exe`.
   
   3. Copy the newly created `Calligraphy.sip` and `mu_cdata.sip` files from the `Output` directory to `MHServerEmu2013\Data\Game`.

4. Open the `ClientConfig.xml` file located in `Marvel Heroes\Data\Configs` with a text editor and replace the value of `SiteConfigLocation` with the address of your web server (e.g. `localhost/SiteConfig.xml` if you are running a local server).

5. Start Apache or another web server / reverse proxy solution you are using. You can reuse the same setup as regular MHServerEmu.

6. Start MHServerEmu2013 and wait for it to initialize.

7. Run `MarvelGame.exe` located in `Marvel Heroes\UnrealEngine3\Binaries\Win32` with the following arguments: `-nobitraider -nosteam`.

8. Log in with any email and password.
