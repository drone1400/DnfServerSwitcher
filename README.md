# DnfServerSwitcher
A simple .NET Framework application for switching server modes in Duke Nukem Forever 2011.

![](.doc/Application.png)

## How it works
Upon launching the first time, the application will attempt to auto locate the DukeForever.exe file and the system.ini file.
If auto-detection fails, you can manually set the paths using the browse buttons.

Click Launch Normal to select the normal servers and play multiplayer normally.

Click Launch Derpecated to select the deprecated server mode and play multiplayer on custom servers.

When launching the game, the application configures the server settings in system.ini according to which mode you launch. 

Additionally, when launching in Deprecated mode, the dnWindow.u mod file required for Deprecated mode to work will be copied over into Duke Nukem Forever's system folder. 

NOTE: The application will copy the dnWindow.u file from the "ThirdParty" subfolder. Before launching Deprecated mode, if the file already exists, the dnWindow.u file checksum will be verified and the file will be replaced if different. If you want to use a different dnWindow.u file, replace the one in the "ThirdParty" subfolder!