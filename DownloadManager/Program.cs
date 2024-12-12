using System;
using System.Windows.Forms;

namespace DownloadManager
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana giriş noktasıdır.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Global hata yakalama için bir handler tanımla
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // MainForm'u başlat
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// İş parçacığı seviyesindeki istisnaları ele alır.
        /// </summary>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        /// <summary>
        /// Uygulama alanındaki istisnaları ele alır.
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception);
            }
        }

        /// <summary>
        /// Hataları ele alır ve kullanıcıya bildirir.
        /// </summary>
        private static void HandleException(Exception ex)
        {
            // Hata mesajını log dosyasına yaz
            Utilities.WriteLog($"Beklenmeyen bir hata oluştu: {ex.Message}\n{ex.StackTrace}");

            // Hata mesajını kullanıcıya göster
            MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
