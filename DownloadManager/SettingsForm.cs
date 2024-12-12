using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DownloadManager
{
    public partial class SettingsForm : Form
    {
        private AppSettings settings;

        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            settings = Utilities.LoadConfig();

            numPartCount.Value = settings.PartCount;
            txtDestinationPath.Text = settings.DestinationPath;
            numTimeout.Value = settings.Timeout;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDestinationPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            settings.PartCount = (int)numPartCount.Value;
            settings.DestinationPath = txtDestinationPath.Text.Trim();
            settings.Timeout = (int)numTimeout.Value;

            Utilities.SaveConfig(settings);
            MessageBox.Show("Ayarlar başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }

    public class Settings
    {
        public int PartCount { get; set; }
        public string DestinationPath { get; set; }
        public int Timeout { get; set; }
    }
}
