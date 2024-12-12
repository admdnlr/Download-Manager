using System.Drawing;
using System.Windows.Forms;

namespace DownloadManager
{
    partial class MainForm
    {
        private Label lblUrl;
        private TextBox txtUrl;
        private Button btnStart;
        private Button btnPause;
        private Button btnResume;
        private Button btnStop;
        private Button btnDelete;
        private ProgressBar progressBar;
        private Label lblProgress;
        private RichTextBox rtbLogs;
        private Button btnSettings;
        private DataGridView dgvDownloads;

        private void InitializeComponent()
        {
            this.dgvDownloads = new DataGridView();
            this.lblUrl = new Label();
            this.txtUrl = new TextBox();
            this.btnStart = new Button();
            this.btnPause = new Button();
            this.btnResume = new Button();
            this.btnStop = new Button();
            this.btnDelete = new Button();
            this.progressBar = new ProgressBar();
            this.lblProgress = new Label();
            this.btnSettings = new Button();
            this.rtbLogs = new RichTextBox();

            //
            // Delete Button
            //
            this.btnDelete.Location = new Point(342, 210);
            this.btnDelete.Size = new Size(100, 30);
            this.btnDelete.Text = "Sil";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.Controls.Add(this.btnDelete);

            //
            // btnSettings
            //
            this.btnSettings.Location = new Point(715, 10);
            this.btnSettings.Size = new Size(100, 30);
            this.btnSettings.Text = "Ayarlar";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            this.Controls.Add(this.btnSettings);

            // 
            // dgvDownloads
            // 
            this.dgvDownloads.Location = new Point(12, 250);
            this.dgvDownloads.Size = new Size(800, 200);
            this.dgvDownloads.AutoGenerateColumns = false;
            this.dgvDownloads.AllowUserToAddRows = false;
            this.dgvDownloads.AllowUserToDeleteRows = false;
            this.dgvDownloads.ReadOnly = true;
            this.dgvDownloads.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvDownloads.MultiSelect = false;

            // Sütunlar
            this.dgvDownloads.Columns.Add(new DataGridViewTextBoxColumn { Name = "Url", DataPropertyName = "Url", HeaderText = "URL", Width = 220 });
            this.dgvDownloads.Columns.Add(new DataGridViewTextBoxColumn { Name = "FileName", DataPropertyName = "FileName", HeaderText = "Dosya Adı", Width = 150 });
            this.dgvDownloads.Columns.Add(new DataGridViewTextBoxColumn { Name = "FileSizeFormatted", DataPropertyName = "FileSizeFormatted", HeaderText = "Boyut", Width = 120 });
            this.dgvDownloads.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = "Durum", Width = 100 });
            this.dgvDownloads.Columns.Add(new DataGridViewTextBoxColumn { Name = "Progress", DataPropertyName = "Progress", HeaderText = "İlerleme (%)", Width = 100 });
            this.dgvDownloads.Columns.Add(new DataGridViewTextBoxColumn { Name = "DownloadDate", DataPropertyName = "DownloadDate", HeaderText = "Tarih", Width = 150 });
            this.dgvDownloads.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvDownloads_CellDoubleClick);
            this.Controls.Add(this.dgvDownloads);

            // 
            // lblUrl
            // 
            this.lblUrl.Location = new Point(12, 15);
            this.lblUrl.Size = new Size(80, 20);
            this.lblUrl.Text = "URL Girin:";

            // 
            // txtUrl
            // 
            this.txtUrl.Location = new Point(100, 12);
            this.txtUrl.Size = new Size(500, 20);

            // 
            // btnStart
            // 
            this.btnStart.Location = new Point(620, 10);
            this.btnStart.Size = new Size(90, 30);
            this.btnStart.Text = "Başlat";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            this.btnStart.Enabled = true;

            // 
            // btnPause
            // 
            this.btnPause.Location = new Point(12, 210);
            this.btnPause.Size = new Size(100, 30);
            this.btnPause.Text = "Duraklat";
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            this.btnPause.Enabled = false;

            // 
            // btnResume
            // 
            this.btnResume.Location = new Point(120, 210);
            this.btnResume.Size = new Size(100, 30);
            this.btnResume.Text = "Devam Et";
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            this.btnResume.Enabled = false;

            // 
            // btnStop
            // 
            this.btnStop.Location = new Point(230, 210);
            this.btnStop.Size = new Size(100, 30);
            this.btnStop.Text = "Durdur";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            this.btnStop.Enabled = false;

            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(12, 80);
            this.progressBar.Size = new Size(800, 20);

            // 
            // lblProgress
            // 
            this.lblProgress.Location = new Point(12, 110);
            this.lblProgress.Size = new Size(150, 20);
            this.lblProgress.Text = "İlerleme: 0%";

            // 
            // rtbLogs
            // 
            this.rtbLogs.Location = new Point(12, 140);
            this.rtbLogs.Size = new Size(800, 60);
            this.rtbLogs.ReadOnly = true;

            // 
            // MainForm
            // 
            this.ClientSize = new Size(825, 475);
            this.Controls.Add(this.lblUrl);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.rtbLogs);
            this.Text = "İnternet Download Manager";
        }
    }
}
