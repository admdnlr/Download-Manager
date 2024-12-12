using Newtonsoft.Json;
using System;
using System.IO;

namespace DownloadManager
{
    public static class Utilities
    {
        /// <summary>
        /// Verilen URL'nin geçerli bir formatta olup olmadığını kontrol eder.
        /// </summary>
        public static bool IsValidUrl(string url)
        {
            try
            {
                return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) &&
                       (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            catch (Exception ex)
            {
                WriteLog($"URL doğrulama hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verilen dosya yolunun yazılabilir olup olmadığını kontrol eder.
        /// </summary>
        public static bool IsWritablePath(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path); // Dizin yoksa oluştur
                }

                string testFile = Path.Combine(path, "test.tmp");
                using (FileStream fs = File.Create(testFile, 1, FileOptions.DeleteOnClose))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Dosya yolu yazılabilir değil: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Belirtilen log dosyasına bir mesaj yazar.
        /// </summary>
        public static void WriteLog(string message, string logFilePath = "Logs.txt")
        {
            try
            {
                string directory = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now}] {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log dosyasına yazılamadı: {ex.Message}");
            }
        }

        /// <summary>
        /// Config.json dosyasını yükler.
        /// </summary>
        public static AppSettings LoadConfig(string configPath = "Config.json")
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    return JsonConvert.DeserializeObject<AppSettings>(json);
                }
                else
                {
                    WriteLog($"Config.json bulunamadı, varsayılan ayarlar yükleniyor.");
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Config dosyası okunamadı: {ex.Message}");
            }

            // Varsayılan ayarlar
            return new AppSettings
            {
                PartCount = 5,
                Timeout = 30,
                DestinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Downloads")
            };
        }

        /// <summary>
        /// Config.json dosyasını kaydeder.
        /// </summary>
        public static void SaveConfig(AppSettings settings, string configPath = "Config.json")
        {
            try
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                WriteLog($"Config dosyası kaydedilemedi: {ex.Message}");
            }
        }
    }
}
