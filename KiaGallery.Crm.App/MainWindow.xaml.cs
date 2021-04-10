using Hardcodet.Wpf.TaskbarNotification;
using KiaGallery.Common;
using KiaGallery.Crm.App.Core;
using KiaGallery.Crm.App.Forms;
using KiaGallery.Crm.App.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KiaGallery.Crm.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserDetailViewModel user;
        TaskbarIcon notifyIcon;

        public MainWindow(TaskbarIcon _notifyIcon)
        {
            InitializeComponent();

            notifyIcon = _notifyIcon;

            var file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "data");
            if (File.Exists(file))
            {

                var json = File.ReadAllText(file);
                var _data = JsonConvert.DeserializeObject<UserDetailViewModel>(json);

                user = _data;

                lblUsername.Content = _data.firstName + " " + _data.lastName;
                lblBranchName.Content = _data.branchName;

                txtBarcode.Focus();
            }
            else
            {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoadCustomer();
            }
        }

        private void Loading(bool status)
        {
            if (!status)
            {
                txtBarcode.IsEnabled = true;
            }
            else
            {
                txtBarcode.IsEnabled = false;
            }
        }

        private async void LoadCustomer()
        {
            if (string.IsNullOrEmpty(txtBarcode.Text))
            {
                MessageBox.Show("کد وارد شده صحیح نمی باشد.");
                return;
            }

            string barcode = txtBarcode.Text;

            Loading(true);
            Response<CustomerViewModel> response = await GetCustomer(barcode);
            if (response.status == 404)
            {
                RegisterWindow window = new RegisterWindow(user, barcode);
                window.ShowDialog();
            }
            else if (response.status == 403)
            {
                LoginWindow login = new LoginWindow();
                login.Show();
                Close();
            }
            else
            {
                CustomerMainWindow window = new CustomerMainWindow(user, response.data);
                window.ShowDialog();
            }
            Loading(false);
            txtBarcode.Text = "";
            txtBarcode.Focus();
        }


        public async Task<Response<CustomerViewModel>> GetCustomer(string barcode)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Config.GetCustomerUrl + "?barcode=" + barcode);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                request.Headers.Add("Authorization", user.token);

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<Response<CustomerViewModel>>(result);
                }
            }
            catch (Exception)
            {
                return new Response<CustomerViewModel>() { status = 403 };
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();

            string title = "مدیریت مشتری گالری کیا";
            string text = "با کلیک روی آیکن میتوانید برنامه را مجدد راه اندازی کنید";

            //show balloon with built-in icon
            notifyIcon.ShowBalloonTip(title, text, BalloonIcon.Info);

            //notifyIcon.HideBalloonTip();
        }

    }


}
