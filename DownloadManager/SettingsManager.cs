using Newtonsoft.Json;
using System.IO;

namespace DownloadManager
{
    public static class SettingsManager
    {
        private const string ConfigFilePath = "Config.json";

        /// <summary>
        /// Config.json dosyasını yükler ve ayarları döndürür.
        /// </summary>
        public static AppSettings LoadSettings()
        {
            return Utilities.LoadConfig(ConfigFilePath);
        }

        /// <summary>
        /// Config.json dosyasına ayarları kaydeder.
        /// </summary>
        public static void SaveSettings(AppSettings settings)
        {
            Utilities.SaveConfig(settings, ConfigFilePath);
        }

        /// <summary>
        /// DestinationPath ayarını döndürür.
        /// </summary>
        public static string GetDestinationPath()
        {
            var settings = LoadSettings();
            return settings.DestinationPath;
        }
    }
}
