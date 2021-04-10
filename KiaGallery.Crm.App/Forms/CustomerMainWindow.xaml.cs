using KiaGallery.Common;
using KiaGallery.Crm.App.Core;
using KiaGallery.Crm.App.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CustomerMainWindow.xaml
    /// </summary>
    public partial class CustomerMainWindow : Window
    {
        UserDetailViewModel user;
        CustomerViewModel customer;
        List<DiscountSettingViewModel> discountList;
        List<BarcodeViewModel> barcodeList = new List<BarcodeViewModel>();
        List<BarcodeViewModel> revocationBarcodeList = new List<BarcodeViewModel>();

        int invoiceType = 0;

        public CustomerMainWindow(UserDetailViewModel _user, CustomerViewModel _customer)
        {
            InitializeComponent();

            user = _user;
            customer = _customer;

            lblFirstName.Content = _customer.firstName;
            lblLastName.Content = _customer.lastName;
            lblNationalityCode.Content = string.IsNullOrEmpty(_customer.nationalityCode) ? "-" : _customer.nationalityCode;
            lblMobileNumber.Content = _customer.mobileNo;
            lblBirthDate.Content = _customer.birthDate;
            lblWeddingDate.Content = _customer.weddingDate;
            lblBalance.Content = _customer.balance.ToString("N0") + " ریال";

            if (_customer.balance <= 0)
                txtDiscount.IsEnabled = false;

            LoadData();
        }

        private void txtPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPrice.Text))
            {
                var price = long.Parse(txtPrice.Text.Replace(",", ""));

                txtPrice.Text = price.ToString("N0");

                lblPriceText.Content = NumberToString.NumToString(price.ToString()) + " ریال";

                txtPrice.SelectionStart = txtPrice.Text.Length;
                txtPrice.SelectionLength = 0;
            }
            else
            {
                lblPriceText.Content = "";
            }

            CalculateDiscount();
        }
        private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDiscount.Text) && int.Parse(txtDiscount.Text.Replace(",", "")) > 0)
            {
                var price = long.Parse(txtDiscount.Text.Replace(",", ""));

                lblDiscount.Content = NumberToString.NumToString(price.ToString()) + " ریال";

                txtDiscount.Text = price.ToString("N0"); txtDiscount.SelectionStart = txtDiscount.Text.Length;
                txtDiscount.SelectionLength = 0;
            }
            else
            {
                lblDiscount.Content = "";
            }

            CalculateDiscount();
        }

        private void LoadData()
        {
            lblLoading.Visibility = Visibility.Visible;
            lblGoldPrice.Visibility = Visibility.Hidden;
            Loading(true);
            Task.Factory.StartNew(async () =>
            {
                Response<SettingsViewModel> response;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Config.BaseUrl);
                    client.DefaultRequestHeaders.Add("Authorization", user.token);

                    var result = await client.GetAsync(Config.GetSettingsUrl);
                    var resultContent = await result.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<Response<SettingsViewModel>>(resultContent);
                }
                Dispatcher.Invoke(() =>
                {
                    if (response.status == 200)
                    {
                        discountList = response.data.discountList;
                        lblGoldPrice.Content = "قیمت طلا: " + response.data.goldPrice.ToString("N0") + " ریال";
                    }
                    else
                    {
                        MessageBox.Show(response.message);
                    }
                    Loading(false);
                    lblLoading.Visibility = Visibility.Hidden;
                    lblGoldPrice.Visibility = Visibility.Visible;
                });
            });
        }

        private void CalculateDiscount()
        {
            if (discountList != null && !string.IsNullOrEmpty(txtPrice.Text))
            {
                var discount = string.IsNullOrEmpty(txtDiscount.Text) ? 0 : int.Parse(txtDiscount.Text.Replace(",", ""));
                var price = int.Parse(txtPrice.Text.Replace(",", "")) - discount;

                var persentDiscount = discountList.SingleOrDefault(x => x.fromPrice <= price && x.toPrice >= price)?.discount;
                if (persentDiscount != null)
                {
                    txtDiscountEarn.Text = (((double)(price * persentDiscount)) / 100).ToString("N0");
                }
                else
                {
                    txtDiscountEarn.Text = "0";
                }
            }
        }

        private void Loading(bool status)
        {
            if (!status)
            {
                txtPrice.IsEnabled = true;
                txtDiscount.IsEnabled = true;
                txtBarcode.IsEnabled = true;
                txtRevocationPrice.IsEnabled = true;
                txtRevocationBarcode.IsEnabled = true;
                btnSave.IsEnabled = true;
                btnRevocation.IsEnabled = true;
            }
            else
            {
                txtPrice.IsEnabled = false;
                txtDiscount.IsEnabled = false;
                txtBarcode.IsEnabled = false;
                txtRevocationPrice.IsEnabled = false;
                txtRevocationBarcode.IsEnabled = false;
                btnSave.IsEnabled = false;
                btnRevocation.IsEnabled = false;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DigitValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("وارد کردن قیمت اجباری است.");
                return;
            }

            var customerId = customer.id;
            var price = int.Parse(txtPrice.Text.Replace(",", ""));
            var discount = string.IsNullOrEmpty(txtDiscount.Text) ? 0 : int.Parse(txtDiscount.Text.Replace(",", ""));
            var discountPersentage = 0f;
            var persentDiscount = discountList.SingleOrDefault(x => x.fromPrice <= price && x.toPrice >= price)?.discount;
            if (persentDiscount != null)
            {
                discountPersentage = persentDiscount.GetValueOrDefault();
            }

            Loading(true);
            var data = new InvoiceViewModel()
            {
                customerId = customer.id,
                amount = price,
                invoiceType = 0,
                discount = discount,
                detailList = barcodeList.Select(x => new InvoiceDetailViewModel() { barcode = x.barcode }).ToList()
            };

            Task.Factory.StartNew(() =>
            {
                Response response;
                var dataString = JsonConvert.SerializeObject(data);

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add("Authorization", user.token);
                    var responseString = client.UploadString(new Uri(Config.SaveInvoiceUrl), "POST", dataString);
                    response = JsonConvert.DeserializeObject<Response>(responseString);
                }

                Dispatcher.Invoke(() =>
                {
                    Loading(false);
                    if (response.status == 200)
                    {
                        MessageBox.Show(response.message);
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(response.message);
                    }
                });
            });
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AddBarcode();
            }
        }

        private void AddBarcode()
        {
            var barcode = txtBarcode.Text;
            if (!string.IsNullOrEmpty(barcode))
            {
                if (barcodeList.Where(x => x.barcode == barcode).Count() == 0)
                {
                    barcodeList.Add(new BarcodeViewModel() { barcode = barcode });

                    GridBarcode.ItemsSource = barcodeList;
                    GridBarcode.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("بارکد وارد شده در لیست موجود است.");
                }
                txtBarcode.Text = "";
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var SelectedBarcode = (BarcodeViewModel)((Image)sender).DataContext;
            var item = barcodeList.Single(x => x.barcode == SelectedBarcode.barcode);
            barcodeList.Remove(item);

            GridBarcode.ItemsSource = barcodeList;
            GridBarcode.Items.Refresh();
        }

        private void txtRevocationPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRevocationPrice.Text))
            {
                var price = long.Parse(txtRevocationPrice.Text.Replace(",", ""));

                txtRevocationPrice.Text = price.ToString("N0");

                lblRevocationPriceText.Content = NumberToString.NumToString(price.ToString()) + " ریال";

                txtRevocationPrice.SelectionStart = txtRevocationPrice.Text.Length;
                txtRevocationPrice.SelectionLength = 0;
            }
            else
            {
                lblRevocationPriceText.Content = "";
            }
        }

        private void txtRevocationBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var barcode = txtRevocationBarcode.Text;
                Loading(true);
                Task.Factory.StartNew(async () =>
                {
                    Response<BarcodeViewModel> response;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(Config.BaseUrl);
                        client.DefaultRequestHeaders.Add("Authorization", user.token);

                        var result = await client.GetAsync(Config.GetBarcodeUrl + "?barcode=" + barcode);
                        var resultContent = await result.Content.ReadAsStringAsync();
                        response = JsonConvert.DeserializeObject<Response<BarcodeViewModel>>(resultContent);
                    }
                    Dispatcher.Invoke(() =>
                    {
                        if (response.status == 200)
                        {
                            AddRevocationBarcode(response.data);
                        }
                        else
                        {
                            MessageBox.Show(response.message);
                        }
                        Loading(false);
                        txtRevocationBarcode.Text = "";
                    });
                });
            }
        }

        private void AddRevocationBarcode(BarcodeViewModel model)
        {
            if (model.revocation)
            {
                MessageBox.Show("این بارکد قبلا باطل شده است.");
                txtRevocationBarcode.Focus();
            }
            else if (revocationBarcodeList.Count(x => x.barcode == model.barcode) == 0)
            {
                revocationBarcodeList.Add(model);

                GridRevocationBarcode.ItemsSource = revocationBarcodeList;
                GridRevocationBarcode.Items.Refresh();
                txtRevocationBarcode.Focus();
            }
            else
            {
                MessageBox.Show("این بارکد در لیست موجود می باشد");
                txtRevocationBarcode.Focus();
            }
        }

        private void ImageRevocation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var SelectedBarcode = (BarcodeViewModel)((Image)sender).DataContext;
            var item = revocationBarcodeList.Single(x => x.barcode == SelectedBarcode.barcode);
            revocationBarcodeList.Remove(item);

            GridRevocationBarcode.ItemsSource = revocationBarcodeList;
            GridRevocationBarcode.Items.Refresh();
        }

        private void btnRevocation_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(txtRevocationPrice.Text))
            {
                MessageBox.Show("وارد کردن مبلغ مرجوعی اجباری است.");
                return;
            }

            var customerId = customer.id;
            var price = int.Parse(txtRevocationPrice.Text.Replace(",", ""));

            Loading(true);
            var data = new InvoiceViewModel()
            {
                customerId = customer.id,
                amount = price,
                invoiceType = 1,
                discount = 0,
                discountPercent = 0,
                detailList = revocationBarcodeList.Select(x => new InvoiceDetailViewModel() { barcode = x.barcode }).ToList()
            };

            Task.Factory.StartNew(() =>
            {
                Response response;
                var dataString = JsonConvert.SerializeObject(data);

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add("Authorization", user.token);
                    var responseString = client.UploadString(new Uri(Config.SaveInvoiceUrl), "POST", dataString);
                    response = JsonConvert.DeserializeObject<Response>(responseString);
                }

                Dispatcher.Invoke(() =>
                {
                    Loading(false);
                    if (response.status == 200)
                    {
                        MessageBox.Show(response.message);
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(response.message);
                    }
                });
            });
        }
    }
}
