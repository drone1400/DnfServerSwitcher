# DnfServerSwitcher
A simple .NET Framework application for switching server modes in Duke Nukem Forever 2011.

Big thanks to Star Nukem for actually starting this project and providing the graphics!

For intense Duke Nukem Forever multiplayer action, join us here: https://steamcommunity.com/groups/dnftournaments

![](.doc/ApplicationTheme.png)

## Description

### Launching the Game
Upon launching the first time, the application will attempt to auto locate the DukeForever.exe file and the system.ini file.
If auto-detection fails, you can manually set the paths using the browse buttons.

Click Launch Normal to select the normal servers and play multiplayer normally.

Click Launch Derpecated to select the deprecated server mode and play multiplayer on custom servers.

When launching the game, the application configures the server settings in system.ini according to which mode you launch. 

Additionally, when launching in Deprecated mode, the dnWindow.u mod file required for Deprecated mode to work will be copied over into Duke Nukem Forever's system folder. 

NOTE: The application will copy the dnWindow.u file from the "ThirdParty" subfolder. Before launching Deprecated mode, if the file already exists, the dnWindow.u file checksum will be verified and the file will be replaced if different. If you want to use a different dnWindow.u file, replace the one in the "ThirdParty" subfolder!

The `Command Line Args` field can be used to specify additional parameters when launching the game...

### Useful tools

The `Get some DNF maps` button opens [https://dnfmaps.com/dnf-2011/](https://dnfmaps.com/dnf-2011/) in a browser. You can downlload custom maps here for both singleplayer and multiplayer.

The `Launch Custom Map` button lets you select a custom map file, then launches the game using that map file as a command line argument, letting you immediately play the map in singleplayer mode!

### Troubleshooting
Sometimes the Steam Cloud sync can get in the way by overwriting the local modified system.ini file with an older version from the cloud. To deal with this you have two options:
1. Use the `Delete remotecache.vdf file` button to delete the local cache file and hopefully have the Steam Cloud sync work properly
2. Tick the checkbox to `Enable uploading the system.ini in the Steam Cloud` - this uses [Facepunch.Steamworks](https://github.com/Facepunch/Facepunch.Steamworks) to send the modified system.ini file to the Steam Cloud

IMPORTANT NOTE: For the app to sync the `system.ini` file into the cloud, Steam needs to be already running. Otherwise this won't work.


## Old Theme

I added a cool new theme, but if you don't like it and want the borring Default look, edit this line from the configuration ini:
`ThemeName=DookieNookie2001` change it into `ThemeName=Default` or just leave it empty like this: `ThemeName=`

Note: I will add a way to do this automatically in the application... soon (tm)

![](.doc/Application.png)