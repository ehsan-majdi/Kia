using System.Windows;

namespace KiaGallery.DailyReport.App
{
    /// <summary>
    /// Interaction logic for AppStart.xaml
    /// </summary>
    public partial class AppStart : Window
    {
        public AppStart()
        {
            //    InitializeComponent();
            new KiaGallery.DailyReport.Forms.FrmLogin().Show();
           // new KiaGallery.DailyReport.Forms.FrmMain().Show();
            this.Close();
        }
    }
}
