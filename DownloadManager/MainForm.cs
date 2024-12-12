using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DownloadManager
{
    public partial class MainForm : Form
    {
        private BindingSource bindingSource;
        private List<DownloadItem> downloadItems;
        private Downloader downloader;

        public MainForm()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            downloadItems = new List<DownloadItem>();
            string historyPath = "download_history.json";
            if (File.Exists(historyPath))
            {
                var history = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DownloadItem>>(File.ReadAllText(historyPath));
                if (history != null)
                {
                    downloadItems = history;
                }
            }

            bindingSource = new BindingSource();
            bindingSource.DataSource = downloadItems;

            dgvDownloads.DataSource = bindingSource;

            dgvDownloads.Refresh(); // DataGridView'i yenile
        }

        private void dgvDownloads_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedItem = dgvDownloads.Rows[e.RowIndex].DataBoundItem as DownloadItem;

                if (selectedItem != null && selectedItem.Status == DownloadStatus.Completed)
                {
                    // Ayarlar formundan alınan hedef dizin
                    string destinationPath = SettingsManager.GetDestinationPath();
                    string filePath = Path.Combine(destinationPath, selectedItem.FileName);

                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        MessageBox.Show("Dosya bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Yalnızca tamamlanmış dosyaları açabilirsiniz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void DgvDownloads_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var row = dgvDownloads.Rows[e.RowIndex];
            var downloadItem = row.DataBoundItem as DownloadItem;

            if (downloadItem == null) return;

            // Renk kodlama duruma göre
            switch (downloadItem.Status)
            {
                case DownloadStatus.Completed:
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    break;
                case DownloadStatus.Failed:
                case DownloadStatus.Stopped:
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    break;
                case DownloadStatus.InProgress:
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    break;
                default:
                    row.DefaultCellStyle.BackColor = Color.White;
                    break;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                string url = txtUrl.Text.Trim();

                if (string.IsNullOrEmpty(url) || !Utilities.IsValidUrl(url))
                {
                    LogMessage("Geçersiz bir URL girdiniz.");
                    MessageBox.Show("Lütfen geçerli bir URL giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ayarları yükle
                AppSettings settings = Utilities.LoadConfig();

                // İndirici oluştur
                var downloader = new Downloader(url, settings.PartCount);

                // Event'leri bağla
                downloader.ProgressChanged += Downloader_ProgressChanged;
                downloader.StatusChanged += Downloader_StatusChanged;

                // Yeni indirme öğesini ekle
                var downloadItem = downloader.GetDownloadItem();
                downloadItems.Add(downloadItem);
                bindingSource.ResetBindings(false);

                // İndirmenin asenkron başlatılması
                downloadItem.DownloadDate = DateTime.Now;
                LogMessage($"İndirme işlemi başladı: {url}");
                UpdateButtonStates(isDownloading: true, isPaused: false);

                await downloader.StartDownloadAsync();

                // İndirme sonrası durum güncellenir
                if (downloadItem.Status == DownloadStatus.Completed)
                {
                    LogMessage($"İndirme tamamlandı: {url}");
                }
                else if (downloadItem.Status == DownloadStatus.Failed)
                {
                    LogMessage($"İndirme başarısız oldu: {url}");
                    MessageBox.Show("İndirme başarısız oldu, lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Hata: {ex.Message}");
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (downloader != null)
            {
                downloader.PauseDownload();
                LogMessage("İndirme işlemi duraklatıldı.");
                UpdateButtonStates(isDownloading: true, isPaused: true);
            }
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            if (downloader != null)
            {
                downloader.ResumeDownload();
                LogMessage("İndirme işlemi devam ediyor.");
                UpdateButtonStates(isDownloading: true, isPaused: false);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (downloader != null)
            {
                downloader.StopDownload();
                LogMessage("İndirme işlemi durduruldu.");
                UpdateButtonStates(isDownloading: false, isPaused: false);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDownloads.SelectedRows.Count > 0)
            {
                var selectedRow = dgvDownloads.SelectedRows[0];
                var downloadItem = selectedRow.DataBoundItem as DownloadItem;

                if (downloadItem != null)
                {
                    // İndirilen dosyayı sil
                    string filePath = Path.Combine(SettingsManager.GetDestinationPath(), downloadItem.FileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    // İndirme listesinden kaldır
                    downloadItems.Remove(downloadItem);
                    bindingSource.ResetBindings(false);
                    SaveDownloadHistory();
                    LogMessage($"İndirme silindi: {downloadItem.FileName}");
                }
            }
            else
            {
                MessageBox.Show("Silmek için bir satır seçiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Downloader_ProgressChanged(DownloadItem item)
        {
            Invoke(new Action(() =>
            {
                var downloadItem = downloadItems.FirstOrDefault(d => d.FileName == item.FileName);
                if (downloadItem != null)
                {
                    downloadItem.Progress = item.Progress;
                    downloadItem.Status = item.Status;
                }

                // Yalnızca değişen satırı güncelle
                int rowIndex = downloadItems.IndexOf(item);
                if (rowIndex >= 0)
                {
                    dgvDownloads.Rows[rowIndex].Cells["Progress"].Value = $"{item.Progress}";
                }

                // ProgressBar ve Label güncelle
                progressBar.Value = Math.Min(item.Progress, 100);
                lblProgress.Text = item.Progress < 100 ? $"İlerleme: {item.Progress}%" : "Tamamlandı";
            }));
        }

        private void Downloader_StatusChanged(DownloadItem item)
        {
            Invoke(new Action(() =>
            {
                // İlgili öğenin durumunu güncelle
                var downloadItem = downloadItems.FirstOrDefault(d => d.FileName == item.FileName);
                if (downloadItem != null)
                {
                    downloadItem.Status = item.Status;
                }

                // DataGridView'i güncelle
                bindingSource.ResetBindings(false);

                // Log ve geçmiş kaydetme
                LogMessage($"Durum güncellendi: {item.FileName} - {item.Status}");
                SaveDownloadHistory();

                // Durum Tamamlandı, Başarısız veya Durduruldu ise butonları güncelle
                if (item.Status == DownloadStatus.Completed ||
                    item.Status == DownloadStatus.Failed ||
                    item.Status == DownloadStatus.Stopped)
                {
                    UpdateButtonStates(isDownloading: false, isPaused: false);

                    // Tamamlandıysa birleştirme işlemini başlat
                    if (item.Status == DownloadStatus.Completed)
                    {
                        LogMessage($"Birleştirme işlemi başlatılıyor: {item.FileName}");
                        string destinationPath = SettingsManager.GetDestinationPath(); // Dinamik yol alındı
                        FileMerger.MergeFiles(destinationPath, item.FileName, item.PartCount);
                        LogMessage($"Birleştirme tamamlandı: {item.FileName}");
                    }
                }
            }));
        }

        private void SaveDownloadHistory()
        {
            string historyPath = "download_history.json";
            File.WriteAllText(historyPath, Newtonsoft.Json.JsonConvert.SerializeObject(downloadItems));
        }

        private void LogMessage(string message)
        {
            rtbLogs.AppendText($"{DateTime.Now}: {message}\n");
        }

        private void UpdateButtonStates(bool isDownloading, bool isPaused)
        {
            btnStart.Enabled = !isDownloading;
            btnPause.Enabled = isDownloading && !isPaused;
            btnResume.Enabled = isDownloading && isPaused;
            btnStop.Enabled = isDownloading;
        }
    }
}
