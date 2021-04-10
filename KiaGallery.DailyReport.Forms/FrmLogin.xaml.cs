using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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

namespace KiaGallery.DailyReport.Forms
{
    /// <summary>
    /// Interaction logic for FrmLogin.xaml
    /// </summary>
    public partial class FrmLogin
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private async void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            //KiaGallery.Common.Response response = await Login(TxtUserName.Text, TxtPassword.Password);

            if (TxtUserName.Text == "" || TxtPassword.Password == "")
            {
                MainSnackbar.MessageQueue.Enqueue("نام کاربری و گذرواژه را وارد کنید.");
                return;
            }

            dhWait.IsOpen = true;
            KiaGallery.Common.Response response = await Login(TxtUserName.Text, TxtPassword.Password);

            if (response.status == 200)
            {
                try
                {
                    Common.Cache.CurrentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<Common.Model.User>(response.data.ToString());
                }
                catch (Exception)
                {
                    MainSnackbar.MessageQueue.Enqueue("خطا در پردازش کاربر. به مدیر سیستم گزارش دهید.");
                }

                new FrmMain().Show();
                Close();
            }
            else
            {
                MainSnackbar.MessageQueue.Enqueue(response.message);
            }

            dhWait.IsOpen = false;

            //Task.Factory.StartNew(() => Login(TxtUserName.Text, TxtPassword.Password));
        }

        private Task<KiaGallery.Common.Response> Login(string UserName, string Password)
        {
            return Task.Run(() =>
            {
                System.Collections.Specialized.NameValueCollection param = new System.Collections.Specialized.NameValueCollection
                {
                    ["username"] = UserName,
                    ["password"] = Password
                };

                KiaGallery.Common.Response Response = Common.Services.CallService(Common.Services.ServiceType.Login, param);
                return Response;
            });
        }

    }
}
