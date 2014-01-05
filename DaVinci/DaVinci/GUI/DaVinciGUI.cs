using System;
using System.Windows.Forms;
using DaVinci.Helpers;
using DaVinci.Properties;

namespace DaVinci.GUI {
    public partial class DaVinciGUI : Form {
        public DaVinciGUI() {
            InitializeComponent();
        }

        private void DaVinciGUI_Load(object sender, EventArgs e) {
            WindowSettings.Restore(Settings.Default.CustomWindowSettings, this);
        }

        private void DaVinciGUI_FormClosing(object sender, FormClosingEventArgs e) {
            Settings.Default.CustomWindowSettings = WindowSettings.Record(Settings.Default.CustomWindowSettings, this);

            Settings.Default.Save();
        }
    }
}
