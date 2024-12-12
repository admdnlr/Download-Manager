using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadManager
{
    public class DownloadPart
    {
        public event Action<int> PartCompleted; // Triggered when part download completes
        public event Action<int, long> ProgressChanged;

        private readonly string _url;
        private readonly string _destinationFolder;
        private readonly int _partIndex;
        private readonly long _startByte;
        private readonly long _endByte;

        private volatile bool _isPaused; // Indicates if the operation is paused
        private volatile bool _isStopped; // Indicates if the operation is stopped
        private ManualResetEventSlim _pauseEvent; // Event for pause control
        private string _partFilePath;

        public bool IsCompleted { get; private set; } // Indicates if the part is downloaded

        public int PartIndex => _partIndex;

        public DownloadPart(string url, string destinationFolder, int partIndex, long startByte, long endByte)
        {
            _url = url;
            _destinationFolder = destinationFolder;
            _partIndex = partIndex;
            _startByte = startByte;
            _endByte = endByte;
            _pauseEvent = new ManualResetEventSlim(true); // Initially set to running state
            _partFilePath = Path.Combine(_destinationFolder, $"part_{_partIndex}.tmp");
        }

        public async Task StartDownloadAsync()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
                request.Method = "GET";
                request.AddRange(_startByte, _endByte);

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                        throw new Exception("Failed to get response stream.");

                    using (FileStream fileStream = new FileStream(_partFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[8192];
                        long totalBytesWritten = 0;
                        int bytesRead;

                        while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            if (_isStopped)
                                return;

                            _pauseEvent.Wait();

                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesWritten += bytesRead;

                            // Trigger progress update
                            ProgressChanged?.Invoke(_partIndex, totalBytesWritten);
                        }
                    }

                    IsCompleted = true;
                    PartCompleted?.Invoke(_partIndex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download part {_partIndex}: {ex.Message}");
            }
        }

        public long GetBytesDownloaded()
        {
            if (File.Exists(_partFilePath))
                return new FileInfo(_partFilePath).Length;

            return 0;
        }

        public void Pause()
        {
            if (!_isPaused)
            {
                _isPaused = true;
                _pauseEvent.Reset(); // Pause the operation
                Console.WriteLine($"Part {_partIndex} paused.");
            }
        }

        public void Resume()
        {
            if (_isPaused)
            {
                _isPaused = false;
                _pauseEvent.Set(); // Resume the operation
                Console.WriteLine($"Part {_partIndex} resumed.");
            }
        }

        public void Stop()
        {
            _isStopped = true;
            _pauseEvent.Set(); // Resume if paused to allow exit
            Console.WriteLine($"Part {_partIndex} stopped.");
        }
    }
}
