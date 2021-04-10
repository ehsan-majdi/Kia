using KiaGallery.Common;
using KiaGallery.Crm.App.Core;
using KiaGallery.Crm.App.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KiaGallery.Crm.App.Forms
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {

        UserDetailViewModel user;

        public RegisterWindow(UserDetailViewModel _user, string barcode)
        {
            InitializeComponent();

            lblBarcode.Content = barcode;

            user = _user;

            txtFirstName.Text = "اسماعیل";
            txtLastName.Text = "عابدی";
            txtNationalityCode.Text = "0013243152";
            txtMobileNo.Text = "09122424519";
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterClick();
        }


        private async void RegisterClick(bool showInvoice = false)
        {
            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                MessageBox.Show("وارد کردن نام اجباری است.");
                return;
            }

            if (string.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("وارد کردن نام خانوادگی اجباری است.");
                return;
            }

            if (string.IsNullOrEmpty(txtMobileNo.Text))
            {
                MessageBox.Show("وارد کردن تلفن همراه اجباری است.");
                return;
            }

            string birthDate = "";
            if ((!string.IsNullOrEmpty(txtBirthDateYear.Text) || !string.IsNullOrEmpty(txtBirthDateMonth.Text) || !string.IsNullOrEmpty(txtBirthDateDay.Text)) && !CheckDate(txtBirthDateYear.Text + "/" + txtBirthDateMonth.Text + "/" + txtBirthDateDay.Text))
            {
                MessageBox.Show("تاریخ تولد وارد شده صحیح نمی باشد.\nنمونه: 1370/01/01");
                return;
            }
            else
            {
                birthDate = txtBirthDateYear.Text + "/" + txtBirthDateMonth.Text + "/" + txtBirthDateDay.Text;
            }

            string weddingDate = "";
            if ((!string.IsNullOrEmpty(txtWeddingDateYear.Text) || !string.IsNullOrEmpty(txtWeddingDateMonth.Text) || !string.IsNullOrEmpty(txtWeddingDateDay.Text)) && !CheckDate(txtWeddingDateYear.Text + "/" + txtWeddingDateMonth.Text + "/" + txtWeddingDateDay.Text))
            {
                MessageBox.Show("تاریخ ازدواج وارد شده صحیح نمی باشد.\nنمونه: 1370/01/01");
                return;
            }
            else
            {
                weddingDate = txtWeddingDateYear.Text + "/" + txtWeddingDateMonth.Text + "/" + txtWeddingDateDay.Text;
            }

            string barcode = lblBarcode.Content.ToString();
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string nationalityCode = txtNationalityCode.Text;
            string mobileNo = txtMobileNo.Text;
            int sex = rdoWoman.IsChecked == true ? 1 : 0;

            Loading(true);
            Response<CustomerViewModel> response = await Register(barcode, firstName, lastName, nationalityCode, mobileNo, birthDate, weddingDate, sex);
            Loading(false);

            if (response.status == 200)
            {
                if (showInvoice)
                {
                    CustomerMainWindow window = new CustomerMainWindow(user, response.data);
                    window.ShowDialog();
                }
                else
                {
                    MessageBox.Show(response.message);
                    Close();
                }
            }
            else
            {
                MessageBox.Show(response.message);
            }
        }

        private async Task<Response<CustomerViewModel>> Register(string barcode, string firstName, string lastName, string nationalityCode, string mobileNo, string birthDate, string weddingDate, int sex)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Config.BaseUrl);
                client.DefaultRequestHeaders.Add("Authorization", user.token);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("barcode", barcode),
                    new KeyValuePair<string, string>("firstName", firstName),
                    new KeyValuePair<string, string>("lastName", lastName),
                    new KeyValuePair<string, string>("nationalityCode", nationalityCode),
                    new KeyValuePair<string, string>("mobileNo", mobileNo),
                    new KeyValuePair<string, string>("birthDate", birthDate),
                    new KeyValuePair<string, string>("weddingDate", weddingDate),
                    new KeyValuePair<string, string>("sex", sex.ToString())
                });
                var result = await client.PostAsync(Config.SaveCustomerUrl, content);
                string resultContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Response<CustomerViewModel>>(resultContent);
            }
        }

        /// <summary>
        /// بررسی صحت تاریخ شمسی وارد شده توسط کاربر
        /// </summary>
        /// <param name="date">تاریخ شمسی</param>
        /// <returns>نتیجه درست بودن</returns>
        private bool CheckDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                return false;

            var datePart = date.Split('/');

            if (datePart.Length != 3)
                return false;

            if (datePart[0].Length != 4)
                return false;

            if (string.IsNullOrEmpty(datePart[0]))
                return false;

            if (string.IsNullOrEmpty(datePart[1]))
                return false;

            if (string.IsNullOrEmpty(datePart[2]))
                return false;

            if (!int.TryParse(datePart[0], out int year))
                return false;

            if (year <= 1200 || year >= 1400)
                return false;

            if (!int.TryParse(datePart[1], out int month))
                return false;

            if (month < 1 || month > 12)
                return false;

            if (!int.TryParse(datePart[2], out int day))
                return false;

            if (month < 6 && day > 31)
                return false;

            if (month > 6 && day > 30)
                return false;

            return true;
        }

        private void Loading(bool status)
        {
            if (!status)
            {
                txtFirstName.IsEnabled = true;
                txtLastName.IsEnabled = true;
                txtNationalityCode.IsEnabled = true;
                txtMobileNo.IsEnabled = true;
                txtBirthDateYear.IsEnabled = true;
                txtBirthDateMonth.IsEnabled = true;
                txtBirthDateDay.IsEnabled = true;
                txtWeddingDateYear.IsEnabled = true;
                txtWeddingDateMonth.IsEnabled = true;
                txtWeddingDateDay.IsEnabled = true;
                rdoMan.IsEnabled = true;
                rdoWoman.IsEnabled = true;
                btnRegister.IsEnabled = true;
            }
            else
            {
                txtFirstName.IsEnabled = false;
                txtLastName.IsEnabled = false;
                txtNationalityCode.IsEnabled = false;
                txtMobileNo.IsEnabled = false;
                txtBirthDateYear.IsEnabled = false;
                txtBirthDateMonth.IsEnabled = false;
                txtBirthDateDay.IsEnabled = false;
                txtWeddingDateYear.IsEnabled = false;
                txtWeddingDateMonth.IsEnabled = false;
                txtWeddingDateDay.IsEnabled = false;
                rdoMan.IsEnabled = false;
                rdoWoman.IsEnabled = false;
                btnRegister.IsEnabled = false;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
