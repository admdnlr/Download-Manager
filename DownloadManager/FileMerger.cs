using System;
using System.IO;

namespace DownloadManager
{
    public static class FileMerger
    {
        /// <summary>
        /// Parçaları birleştirir ve birleştirilmiş dosyayı oluşturur.
        /// </summary>
        /// <param name="destinationPath">Dosyanın kaydedileceği klasör yolu.</param>
        /// <param name="fileName">Oluşturulacak birleşik dosyanın adı.</param>
        /// <param name="partCount">Birleştirilecek toplam parça sayısı.</param>
        public static void MergeFiles(string destinationPath, string fileName, int partCount)
        {
            try
            {
                string mergedFilePath = Path.Combine(destinationPath, fileName);

                // Birleştirilen dosya zaten mevcutsa işlemden çık
                if (File.Exists(mergedFilePath))
                {
                    LogMessage(destinationPath, $"Birleştirme işlemi atlandı, dosya zaten mevcut: {mergedFilePath}");
                    return;
                }

                using (FileStream mergedFileStream = new FileStream(mergedFilePath, FileMode.Create, FileAccess.Write))
                {
                    for (int i = 0; i < partCount; i++)
                    {
                        string partFilePath = Path.Combine(destinationPath, $"part_{i}.tmp");

                        if (!File.Exists(partFilePath))
                        {
                            throw new FileNotFoundException($"Parça dosyası bulunamadı: {partFilePath}");
                        }

                        using (FileStream partFileStream = new FileStream(partFilePath, FileMode.Open, FileAccess.Read))
                        {
                            partFileStream.CopyTo(mergedFileStream);
                        }

                        // Parça dosyasını temizle
                        File.Delete(partFilePath);
                    }
                }

                LogMessage(destinationPath, $"Birleştirme işlemi başarıyla tamamlandı. Dosya: {mergedFilePath}");
            }
            catch (Exception ex)
            {
                LogMessage(destinationPath, $"Birleştirme işlemi başarısız oldu: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Log mesajlarını belirli bir dosyaya yazar.
        /// </summary>
        /// <param name="destinationPath">Log dosyasının kaydedileceği klasör yolu.</param>
        /// <param name="message">Kaydedilecek log mesajı.</param>
        private static void LogMessage(string destinationPath, string message)
        {
            string logPath = Path.Combine(destinationPath, "Logs.txt");
            try
            {
                using (StreamWriter logWriter = new StreamWriter(logPath, true))
                {
                    logWriter.WriteLine($"[{DateTime.Now}] {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log yazılamadı: {ex.Message}");
            }
        }
    }
}
