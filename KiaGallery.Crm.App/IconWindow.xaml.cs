using KiaGallery.Crm.App.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KiaGallery.Crm.App
{
    /// <summary>
    /// Interaction logic for IconWindow.xaml
    /// </summary>
    public partial class IconWindow : Window
    {
        public static MainWindow mainWindow;
        public IconWindow()
        {
            InitializeComponent();

            var file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "data");
            File.Delete(file);

            CheckLogin();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void CheckLogin()
        {
            var file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "data");
            if (File.Exists(file))
            {
                mainWindow = new MainWindow(NotifyIcon);
                mainWindow.Show();
            }
            else
            {
                LoginWindow login = new LoginWindow();
                login.ShowDialog();
                CheckLogin();
            }
        }
    }
}
