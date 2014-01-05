using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DaVinci.Helpers;
using Styx;
using Styx.Common;

namespace DaVinci.GUI {
    public class GeneralSettings {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static GeneralSettings Instance = new GeneralSettings();

        // ===========================================================
        // Constructors
        // ===========================================================

        static GeneralSettings() {
            var folderPath = Path.GetDirectoryName(SettingsFilePath);

            if(folderPath != null && !Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }

            Load();
        }

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        public static string SettingsFilePath {
            get { return Path.Combine(Utilities.AssemblyDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "DaVinci", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "GeneralSettings")); }
        }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void Load() {
            try {
                Instance = ObjectXMLSerializer<GeneralSettings>.Load(SettingsFilePath);
            } catch(Exception) {
                Instance = new GeneralSettings();
            }
        }

        public static void Save() {
            ObjectXMLSerializer<GeneralSettings>.Save(Instance, SettingsFilePath);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
