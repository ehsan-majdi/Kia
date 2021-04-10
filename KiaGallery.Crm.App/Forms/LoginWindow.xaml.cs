using KiaGallery.Common;
using KiaGallery.Crm.App.Core;
using KiaGallery.Crm.App.Model;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KiaGallery.Crm.App.Forms
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Loading(bool status)
        {
            if (!status)
            {
                txtUsername.IsEnabled = true;
                txtPassword.IsEnabled = true;
                btnLogin.IsEnabled = true;
            }
            else
            {
                txtUsername.IsEnabled = false;
                txtPassword.IsEnabled = false;
                btnLogin.IsEnabled = false;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("وارد کردن نام کاربری اجباری است.");
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("وارد کردن گذرواژه اجباری است.");
                return;
            }

            string username = txtUsername.Text;
            string password = txtPassword.Password;

            Loading(true);
            Task.Factory.StartNew(() =>
            {
                DoLogin(username, password);
            });
        }

        private void DoLogin(string username, string password)
        {
            var request = (HttpWebRequest)WebRequest.Create(Config.LoginUrl);

            var postData = "username=" + username;
            postData += "&password=" + password;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
            var response = JsonConvert.DeserializeObject<Response<UserDetailViewModel>>(responseString);

            Dispatcher.Invoke(() =>
            {
                if (response.status == 200)
                {
                    var file = Path.Combine(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "data");
                    if (File.Exists(file))
                        File.Delete(file);

                    File.WriteAllText(file, JsonConvert.SerializeObject(response.data));
                    Close();
                }
                else
                {
                    MessageBox.Show(response.message);
                }
                Loading(false);
            });
        }
    }
}
