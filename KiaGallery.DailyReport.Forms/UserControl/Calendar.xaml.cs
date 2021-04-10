using KiaGallery.DailyReport.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KiaGallery.DailyReport.Forms.UserControl
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// </summary>
    public partial class Calendar
    {
        private System.Globalization.PersianCalendar _PC = new System.Globalization.PersianCalendar();
        string[] MountsName = new string[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
        private int CurrentMount;
        private IList<Common.Model.CalendarColor> cColors;
        //private List<MaterialDesignThemes.Wpf.Card> cardList = new List<MaterialDesignThemes.Wpf.Card>();;

        public Calendar()
        {
            //Task.Factory.StartNew(() => LoadBaseData());
            InitializeComponent();

        }

        #region Calendar Color Methods

        private IList<Common.Model.CalendarColor> LoadCalendarColors()
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ColorSet.inf");
                if (!System.IO.File.Exists(filePath))
                {
                    System.IO.File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(DefaultColors));
                    return DefaultColors;
                }
                else
                {
                    string fileColor = System.IO.File.ReadAllText(filePath);
                    List<Common.Model.CalendarColor> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Common.Model.CalendarColor>>(fileColor);
                    if (list != null)
                        return list;
                    else
                    {
                        System.IO.File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(DefaultColors));
                        return DefaultColors;
                    }
                }
            }
            catch (Exception)
            {
                return DefaultColors;
            }
        }

        private IList<Common.Model.CalendarColor> DefaultColors
        {
            get
            {
                return new List<Common.Model.CalendarColor>() {
                    new Common.Model.CalendarColor (){ Name= Common.Model.CalendarColor.ColorName.NoDate, NameString  = CalendarColor.ColorName.NoDate.ToString(),Color = "f7f7f7" },
                    new Common.Model.CalendarColor (){ Name= Common.Model.CalendarColor.ColorName.Editable,NameString= CalendarColor.ColorName.Editable.ToString(),Color = "bbffb8" },
                    new Common.Model.CalendarColor (){ Name= Common.Model.CalendarColor.ColorName.Readonly,NameString= CalendarColor.ColorName.Readonly.ToString(),Color = "f1fff0" },
                };
            }
        }

        private Color GetColor(Common.Model.CalendarColor.ColorName ColorName)
        {
            if (cColors == null)
                cColors = LoadCalendarColors();

            Common.Model.CalendarColor calendarColor = cColors.FirstOrDefault(x => x.Name == ColorName);
            if (calendarColor == null)
            {
                calendarColor = DefaultColors.FirstOrDefault(x => x.Name == ColorName);
                if (calendarColor == null)
                {
                    calendarColor = new Common.Model.CalendarColor() { Color = "#ffffff" };
                }
            }

            return HexToColor(calendarColor.Color);
        }

        public System.Windows.Media.Color HexToColor(String hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }



        #endregion


        private Task<bool> LoadBaseData()
        {
            return Task.Run(() =>
            {
                KiaGallery.Common.Response response = Common.Services.CallService(Common.Services.ServiceType.GetBaseData, new System.Collections.Specialized.NameValueCollection() { ["token"] = Common.Cache.CurrentUser?.token ?? "f7bdb768-c11f-4443-b8d8-ba7d90f5e93e" /* Common.Cache.CurrentUser.token */ });

                if (response?.status == 200)
                {
                    Common.Model.Invoice invoice = Newtonsoft.Json.JsonConvert.DeserializeObject<Common.Model.Invoice>(response.data.ToString());
                    Common.Cache.CalendarDays = invoice.calendarList;
                    Common.Cache.CurrencyList = invoice.currencyList;
                    Common.Cache.BankList = invoice.bankList;

                    return true;
                }
                else
                {
                    return false;
                }

            });

            //if (Common.Cache.BankList == null || Common.Cache.BankList?.Length == 0)
            //{
            //    Common.Cache.BankList = new Common.Model.Banklist[] {
            //        new Common.Model.Banklist() { id = 1, order = 2, name = "بانک ملت" },
            //        new Common.Model.Banklist() { id = 2, order = 1, name = "بانک صادرات" },
            //        new Common.Model.Banklist() { id = 3, order = 3, name = "بانک پاسارکاد" }
            //    };
            //}

            //if (Common.Cache.CurrencyList == null || Common.Cache.CurrencyList?.Length == 0)
            //{
            //    Common.Cache.CurrencyList = new Common.Model.Currencylist[] {
            //        new Common.Model.Currencylist() { id = 1, name = "دلار", order = 2 },
            //        new Common.Model.Currencylist() { id = 2, name = "یورو", order = 1 } ,
            //        new Common.Model.Currencylist() { id = 3, name = "لیر ترکیه", order = 3 } };
            //}

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadData_ActionClick(this, new RoutedEventArgs());
        }

        private void DrawCalendar(int Year, int Mount)
        {
            grdCalendar.Children.RemoveRange(7, grdCalendar.Children.Count - 7);

            CurrentMount = Mount;
            txtMountName.Text = MountsName[Mount - 1];

            Style cardTemplate = this.FindResource("tpCard") as Style;

            int days = _PC.GetDaysInMonth(Year, Mount);
            DayOfWeek dW = _PC.GetDayOfWeek(_PC.ToDateTime(Year, Mount, 1, 0, 0, 0, 0));

            int row = 1; int col = (int)dW;

            if (dW == DayOfWeek.Saturday)
            {
                col = 0;
            }
            else
            {
                col++;
            }

            for (int i = 1; i <= days; i++)
            {
                string pDate = Year + "/" + Mount.ToString().PadLeft(2, '0') + "/" + i.ToString().PadLeft(2, '0');
                Calendarlist calendar = Common.Cache.CalendarDays?.FirstOrDefault(x => x.date == pDate);

                if (calendar == null)
                {
                    calendar = new Calendarlist() { date = Year + "/" + Mount + "/" + i, canEdit = false, status = -1 };
                }

                Color color = GetColor(CalendarColor.ColorName.NoDate);
                switch (calendar.status)
                {
                    case -1:
                        color = GetColor(CalendarColor.ColorName.NoDate);
                        break;
                    case 0:
                        color = GetColor(CalendarColor.ColorName.Readonly);
                        break;
                    case 1:
                        color = GetColor(CalendarColor.ColorName.Editable);
                        break;
                    default:
                        break;
                }

                MaterialDesignThemes.Wpf.Card card = new MaterialDesignThemes.Wpf.Card
                {
                    Style = cardTemplate,
                    DataContext = calendar,
                    Content = calendar,
                    Background = new SolidColorBrush(color)
                };

                grdCalendar.Children.Add(card);
                Grid.SetRow(card, row);
                Grid.SetColumn(card, col);

                card.MouseLeftButtonUp += Card_MouseLeftButtonUp;

                if (col > 0 && col % 6 == 0)
                {
                    row++;
                    col = 0;
                }
                else
                {
                    col++;
                }
            }


        }

        private void BtnPreMonth_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMount == 1)
                DrawCalendar(_PC.GetYear(DateTime.Now) - 1, 12);
            else
                DrawCalendar(_PC.GetYear(DateTime.Now), CurrentMount - 1);
        }

        private void BtnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMount == 12)
                DrawCalendar(_PC.GetYear(DateTime.Now) + 1, 1);
            else
                DrawCalendar(_PC.GetYear(DateTime.Now), CurrentMount + 1);

            //KiaGallery.Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel   save = new Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel ();
            //save.

            //KiaGallery.Web.Areas.DailyReportFinancial.Models.DailyReportViewModel dailyReportView = new Web.Areas.DailyReportFinancial.Models.DailyReportViewModel();


            //KiaGallery.Web.Areas.DailyReportFinancial.Controllers.DailyReporttController dailyReporttController = new Web.Areas.DailyReportFinancial.Controllers.DailyReporttController();
            //dailyReporttController.


            // 
        }

        private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Common.Model.Calendarlist cList = ((Common.Model.Calendarlist)((MaterialDesignThemes.Wpf.Card)sender).DataContext);

            if (cList.status == -1)
                return;

            // برای بستن از داخل فرم
            ((DayInputDialog)DlgInput.DialogContent).DialogParent = DlgInput;
            ((DayInputDialog)DlgInput.DialogContent).CalendarData = cList;
            DlgInput.IsOpen = true;
        }

        private async void ReloadData_ActionClick(object sender, RoutedEventArgs e)
        {
            dhWait.IsOpen = true;

            //    msgInternetError.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromSeconds(8));
            bool isValiedLoad = await LoadBaseData();

            if (isValiedLoad)
                DrawCalendar(_PC.GetYear(DateTime.Now), _PC.GetMonth(DateTime.Now));
            else
            {
                msgInternetError.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromHours(1));
                msgInternetError.MessageQueue.Enqueue("خطا در ارتباط با اینترنت، لطفا اتصال به اینترنت را بررسی کنید.", "تلاش دوباره", () => ReloadData_ActionClick(this, new RoutedEventArgs()), false);

            }

            dhWait.IsOpen = false;
        }
    }
}
