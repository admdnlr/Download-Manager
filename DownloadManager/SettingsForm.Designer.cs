using System.Windows.Forms;

namespace DownloadManager
{
    partial class SettingsForm
    {
        private NumericUpDown numPartCount;
        private TextBox txtDestinationPath;
        private Button btnBrowse;
        private NumericUpDown numTimeout;
        private Button btnSave;
        private Label lblPartCount;
        private Label lblDestinationPath;
        private Label lblTimeout;

        private void InitializeComponent()
        {
            this.numPartCount = new NumericUpDown();
            this.txtDestinationPath = new TextBox();
            this.btnBrowse = new Button();
            this.numTimeout = new NumericUpDown();
            this.btnSave = new Button();
            this.lblPartCount = new Label();
            this.lblDestinationPath = new Label();
            this.lblTimeout = new Label();

            ((System.ComponentModel.ISupportInitialize)(this.numPartCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            this.SuspendLayout();

            // 
            // lblPartCount
            // 
            this.lblPartCount.AutoSize = true;
            this.lblPartCount.Location = new System.Drawing.Point(20, 20);
            this.lblPartCount.Size = new System.Drawing.Size(100, 20);
            this.lblPartCount.Text = "Parça Sayısı:";

            // 
            // numPartCount
            // 
            this.numPartCount.Location = new System.Drawing.Point(140, 20);
            this.numPartCount.Minimum = 1;
            this.numPartCount.Maximum = 20;
            this.numPartCount.Value = 5;
            this.numPartCount.Size = new System.Drawing.Size(120, 20);

            // 
            // lblDestinationPath
            // 
            this.lblDestinationPath.AutoSize = true;
            this.lblDestinationPath.Location = new System.Drawing.Point(20, 60);
            this.lblDestinationPath.Size = new System.Drawing.Size(110, 20);
            this.lblDestinationPath.Text = "Hedef Dizin:";

            // 
            // txtDestinationPath
            // 
            this.txtDestinationPath.Location = new System.Drawing.Point(140, 60);
            this.txtDestinationPath.Size = new System.Drawing.Size(200, 20);

            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(350, 58);
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.Text = "Gözat";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(20, 100);
            this.lblTimeout.Size = new System.Drawing.Size(100, 20);
            this.lblTimeout.Text = "Zaman Aşımı (sn):";

            // 
            // numTimeout
            // 
            this.numTimeout.Location = new System.Drawing.Point(140, 100);
            this.numTimeout.Minimum = 5;
            this.numTimeout.Maximum = 300;
            this.numTimeout.Value = 30;
            this.numTimeout.Size = new System.Drawing.Size(120, 20);

            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(140, 140);
            this.btnSave.Size = new System.Drawing.Size(120, 30);
            this.btnSave.Text = "Kaydet";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(450, 200);
            this.Controls.Add(this.lblPartCount);
            this.Controls.Add(this.numPartCount);
            this.Controls.Add(this.lblDestinationPath);
            this.Controls.Add(this.txtDestinationPath);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblTimeout);
            this.Controls.Add(this.numTimeout);
            this.Controls.Add(this.btnSave);
            this.Text = "Ayarlar";
            ((System.ComponentModel.ISupportInitialize)(this.numPartCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
