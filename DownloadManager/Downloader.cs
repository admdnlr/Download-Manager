using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DownloadManager
{
    public enum DownloadStatus
    {
        Pending,
        InProgress,
        Completed,
        Paused,
        Stopped,
        Failed
    }

    public class DownloadItem
    {
        public string Url { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public int PartCount { get; set; }
        public DownloadStatus Status { get; set; }
        public int Progress { get; set; }
        public DateTime DownloadDate { get; set; }
    }

    public class Downloader
    {
        public event Action<DownloadItem> ProgressChanged;
        public event Action<DownloadItem> StatusChanged;

        private readonly string _url;
        private readonly string _destinationFolder;
        private readonly int _partCount;

        private List<DownloadPart> _downloadParts;
        private DownloadItem _downloadItem;
        private bool _isPaused;
        private bool _isStopped;

        public Downloader(string url, int partCount)
        {
            _url = url;
            _destinationFolder = SettingsManager.GetDestinationPath();
            _partCount = partCount;

            ServicePointManager.DefaultConnectionLimit = Math.Max(ServicePointManager.DefaultConnectionLimit, _partCount);

            if (!Directory.Exists(_destinationFolder))
            {
                Directory.CreateDirectory(_destinationFolder);
            }

            _downloadItem = new DownloadItem
            {
                Url = url,
                FileName = Path.GetFileName(url),
                FileSize = GetFileSize(url),
                PartCount = partCount,
                Status = DownloadStatus.Pending,
                Progress = 0,
                DownloadDate = DateTime.Now
            };

            _downloadParts = new List<DownloadPart>();
        }

        public DownloadItem GetDownloadItem() => _downloadItem;

        public async Task StartDownloadAsync()
        {
            try
            {
                _downloadItem.Status = DownloadStatus.InProgress;
                StatusChanged?.Invoke(_downloadItem);

                long totalFileSize = _downloadItem.FileSize;
                long partSize = totalFileSize / _partCount;

                var tasks = new List<Task>();

                for (int i = 0; i < _partCount; i++)
                {
                    long start = i * partSize;
                    long end = (i == _partCount - 1) ? totalFileSize - 1 : start + partSize - 1;

                    var downloadPart = new DownloadPart(_url, _destinationFolder, i, start, end);
                    downloadPart.PartCompleted += OnPartCompleted;
                    downloadPart.ProgressChanged += OnPartProgressChanged;
                    _downloadParts.Add(downloadPart);

                    // Her part ayrı bir task olarak başlatılıyor
                    tasks.Add(Task.Run(() => downloadPart.StartDownloadAsync()));
                }

                await Task.WhenAll(tasks); // Tüm parçalar tamamlanana kadar bekle

                if (!_isStopped)
                {
                    FileMerger.MergeFiles(_destinationFolder, _downloadItem.FileName, _partCount);
                    _downloadItem.Status = DownloadStatus.Completed;
                    StatusChanged?.Invoke(_downloadItem);
                }
            }
            catch (Exception ex)
            {
                _downloadItem.Status = DownloadStatus.Failed;
                StatusChanged?.Invoke(_downloadItem);
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }
        private void OnPartProgressChanged(int partIndex, long bytesWritten)
        {
            long totalBytesDownloaded = _downloadParts.Sum(p => p.GetBytesDownloaded());
            int overallProgress = (int)((totalBytesDownloaded * 100) / _downloadItem.FileSize);

            _downloadItem.Progress = overallProgress;
            ProgressChanged?.Invoke(_downloadItem); // Notify UI or other listeners
        }

        public void PauseDownload()
        {
            _isPaused = true;
            _downloadItem.Status = DownloadStatus.Paused;
            StatusChanged?.Invoke(_downloadItem);

            foreach (var part in _downloadParts)
            {
                part.Pause();
            }
        }

        public void ResumeDownload()
        {
            _isPaused = false;
            _downloadItem.Status = DownloadStatus.InProgress;
            StatusChanged?.Invoke(_downloadItem);

            foreach (var part in _downloadParts)
            {
                part.Resume();
            }
        }

        public void StopDownload()
        {
            _isStopped = true;
            _downloadItem.Status = DownloadStatus.Stopped;
            StatusChanged?.Invoke(_downloadItem);

            foreach (var part in _downloadParts)
            {
                part.Stop();
            }

            CleanupTemporaryFiles();
        }

        private void OnPartCompleted(int partIndex)
        {
            int completedParts = _downloadParts.Count(p => p.IsCompleted);
            int progress = (completedParts * 100) / _partCount;

            _downloadItem.Progress = progress;
            ProgressChanged?.Invoke(_downloadItem);
        }

        private void CleanupTemporaryFiles()
        {
            foreach (var part in _downloadParts)
            {
                string tempFilePath = Path.Combine(_destinationFolder, $"part_{part.PartIndex}.tmp");
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        private long GetFileSize(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.ContentLength;
            }
        }
    }
}