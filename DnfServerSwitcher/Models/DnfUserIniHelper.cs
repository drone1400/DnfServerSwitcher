using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DnfServerSwitcher.Models.KrazyIni;
using DnfServerSwitcher.Models.Trace;
namespace DnfServerSwitcher.Models {
    public static class DnfUserIniHelper {

        public static bool SetUserIniData(string userIniFile, Key hotkey, string playerName, int desiredFov) {
            IniDocument? data = DnfIniParseHelper.ParseDnf2011Ini(userIniFile);
            if (data == null) {
                MessageBox.Show("An error has occurred parsing the user.ini file!...",  "ERROR parsing user.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Glog.Message(MyTraceCategory.General, $"Successfully parsed system.ini at path={userIniFile}");

            // set player name binding
            string hotkeyIni = DnfHotkeyHelper.GetKeyIniString(hotkey);
            string command = $"name {playerName}";
            data["Engine.Input"][hotkeyIni].SetSimpleValue(command);
            
            // set field of view
            data["Engine.PlayerPawn"]["DefaultFOV"].SetSimpleValue(desiredFov.ToString());
            data["dnGame.DukeMultiplayerAssets"]["DefaultFOV"].SetSimpleValue(desiredFov.ToString());
            data["dnGame.DukePlayer"]["DefaultFOV"].SetSimpleValue(desiredFov.ToString());
            data["DLC03_Game.DukePlayerDLC03"]["DefaultFOV"].SetSimpleValue(desiredFov.ToString());
            
            Glog.Message(MyTraceCategory.General, new List<string>() {
                "--- Updated user.ini values are ---",
                "[Engine.Input]hotkeyIni=" + data["Engine.Input"][hotkeyIni].GetSimpleValue(),
                "[Engine.PlayerPawn]DefaultFOV=" + data["Engine.PlayerPawn"]["DefaultFOV"].GetSimpleValue(),
                "[dnGame.DukeMultiplayerAssets]DefaultFOV=" + data["dnGame.DukeMultiplayerAssets"]["DefaultFOV"].GetSimpleValue(),
                "[dnGame.DukePlayer]DefaultFOV=" + data["dnGame.DukePlayer"]["DefaultFOV"].GetSimpleValue(),
                "[DLC03_Game.DukePlayerDLC03]DefaultFOV=" + data["DLC03_Game.DukePlayerDLC03"]["DefaultFOV"].GetSimpleValue(),
                "--- end of updated user.ini values ---",
            });

            if (!DnfIniParseHelper.WriteDnf2011Ini(userIniFile, data)) {
                MessageBox.Show("An error has occurred saving the user.ini file!...",  "ERROR saving user.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Glog.Message(MyTraceCategory.General, $"Successfully wrote updated user.ini at path={userIniFile}");
            return true;
        }
    }
}
