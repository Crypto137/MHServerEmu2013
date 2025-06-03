# MHServerEmu2013

MHServerEmu2013 is a server emulator for the classic version of Marvel Heroes. If you are looking for a server emulator for later versions of the game, see [MHServerEmu](https://github.com/Crypto137/MHServerEmu).

The primary supported version of the game client is **1.10.0.643** released on July 18, 2013. The server can also be built for game version **1.10.0.69** released on May 31, 2013 by specifying the `BUILD_1_10_0_69` conditional compilation symbol.

## Setup Instructions

These instructions assume you already have an MHServerEmu setup of some kind. If not, see [Initial Setup](https://github.com/Crypto137/MHServerEmu/blob/master/docs/Setup/InitialSetup.md).

1. Game version 1.10 uses the legacy game data archive format. You need to convert it to use with MHServerEmu2013.
   
   1. Download the latest version of [MHSqlitePakRepacker](https://github.com/Crypto137/MHSqlitePakRepacker/releases).
   
   2. Drag and drop the `mu_cdata.sip` file located in `Marvel Heroes\UnrealEngine3\Binaries\Win32\Data` onto `MHSqlitePakRepacker.exe`.
   
   3. Copy the newly created `Calligraphy.sip` and `mu_cdata.sip` files from the `Output` directory to `MHServerEmu2013\Data\Game`.

2. Open the `ClientConfig.xml` file located in `Marvel Heroes\Data\Configs` with a text editor and replace the value of `SiteConfigLocation` with the address of your web server (e.g. `localhost/SiteConfig.xml` if you are running a local server).

3. Start Apache or another web server / reverse proxy solution you are using. You can reuse the same setup as regular MHServerEmu.

4. Start MHServerEmu2013 and wait for it to initialize.

5. Run `MarvelGame.exe` located in `Marvel Heroes\UnrealEngine3\Binaries\Win32` with the following arguments: `-nobitraider -nosteam`.

6. Log in with any email and password.
