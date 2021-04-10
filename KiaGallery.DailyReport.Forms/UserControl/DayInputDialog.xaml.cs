using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KiaGallery.DailyReport.Forms.UserControl
{
    /// <summary>
    /// Interaction logic for DayInputDialog.xaml
    /// </summary>
    public partial class DayInputDialog
    {
        public MaterialDesignThemes.Wpf.DialogHost DialogParent;
        public Common.Model.Calendarlist CalendarData = null;

        private SaveDailyReportViewModel BaseObject;

        public DayInputDialog()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogParent.IsOpen = false;
        }

        private async void BtnPreSave_Click(object sender, RoutedEventArgs e)
        {
            dhWait.IsOpen = true;
            KiaGallery.Common.Response result = await PreSaveAsync();

            dhWait.IsOpen = false;
        }


        private Task<KiaGallery.Common.Response> PreSaveAsync()
        {
            return Task.Run(() =>
            {
                SaveDailyReportViewModel save = new SaveDailyReportViewModel()
                {
                    date = CalendarData.date,
                    report = ((DailyReportViewModel)BaseObject.report),
                    token = Common.Cache.CurrentUser.token
                };

                //KiaGallery.Web.Areas.DailyReportFinancial.Controllers.DailyReporttController dailyReporttController = new Web.Areas.DailyReportFinancial.Controllers.DailyReporttController();

                System.Collections.Specialized.NameValueCollection param = new System.Collections.Specialized.NameValueCollection
                {
                    ["model"] = Newtonsoft.Json.JsonConvert.SerializeObject(save)
                };

                return Common.Services.CallService(Common.Services.ServiceType.SaveDraft, param);
            });
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            dhWait.IsOpen = true;
            KiaGallery.Common.Response result = await SaveAsync();

            dhWait.IsOpen = false;
        }

        private Task<KiaGallery.Common.Response> SaveAsync()
        {
            return Task.Run(() =>
            {
                SaveDailyReportViewModel save = new SaveDailyReportViewModel()
                {
                    date = CalendarData.date,
                    report = BaseObject.report,
                    token = Common.Cache.CurrentUser.token
                };

                System.Collections.Specialized.NameValueCollection param = new System.Collections.Specialized.NameValueCollection
                {
                    ["model"] = Newtonsoft.Json.JsonConvert.SerializeObject(save)
                };

                return Common.Services.CallService(Common.Services.ServiceType.Save, param);
            });
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            #region Draw Base objects

            if (CalendarData == null)
                return;


            if (CalendarData.canEdit && CalendarData.status == 0) // قابل ویرایش بود و پیش نویسی موجود نبود 
            {
                BaseObject = new SaveDailyReportViewModel()
                {
                    date = CalendarData.date,
                    token = Common.Cache.CurrentUser.token,
                    report = new Common.Model.DailyReportViewExtendModel()
                    {
                        dailyReportBankList = new List<DailyReportBankViewModel>(),
                        dailyReportCurrencyList = new List<DailyReportCurrencyViewModel>()
                    }
                };
            }
            else if (CalendarData.canEdit && (CalendarData.status == 1 || CalendarData.status == 2)) // قابل ویرایش بود و پیش نویس وجود داشت
            {
                BaseObject = await LoadCalendarReportData();
            }
            else if (!CalendarData.canEdit && CalendarData.status == 2)// قابل ویرایش نبود و اطلاعات ذخیره شده وجود داشت
            {
                BaseObject = await LoadCalendarReportData();
            }
            else if (!CalendarData.canEdit && CalendarData.status == 0) // اگر قابل ویرایش نبود و نه پیش نویس داشت نه ذخیره :|
            {
                BaseObject = new Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel()
                {
                    date = CalendarData.date,
                    token = Common.Cache.CurrentUser.token,
                    report = new Common.Model.DailyReportViewExtendModel()
                    {
                        dailyReportBankList = new List<Web.Areas.DailyReportFinancial.Models.DailyReportBankViewModel>(),
                        dailyReportCurrencyList = new List<Web.Areas.DailyReportFinancial.Models.DailyReportCurrencyViewModel>()
                    }
                };
            }

            this.DataContext = BaseObject.report;
            #endregion


            #region Bank List
            if (GrdBanks.RowDefinitions.Count == 0)
            {
                int i = 0;
                if (Common.Cache.BankList != null)
                {
                    ControlTemplate BankTemplate = GrdBanks.Resources["BankTemplate"] as ControlTemplate;

                    BaseObject.report.dailyReportBankList.Clear();
                    foreach (var bank in Common.Cache.BankList.OrderBy(x => x.id))
                    {
                        GrdBanks.RowDefinitions.Add(new RowDefinition());

                        Grid grid = BankTemplate.LoadContent() as Grid;
                        Grid.SetRow(grid, GrdBanks.RowDefinitions.Count - 1);
                        GrdBanks.Children.Add(grid);

                        Border brdTitle = (Border)grid.Children[0];
                        ((TextBlock)brdTitle.Child).Text = bank.name;

                        BaseObject.report.dailyReportBankList.Add(new Common.Model.DailyReportBankViewExtendModel((Common.Model.DailyReportViewExtendModel)BaseObject.report)
                        {
                            bankId = bank.id
                        });

                        Border brdExit = (Border)grid.Children[1];
                        BindingOperations.SetBinding(brdExit.Child, TextBox.TextProperty, new Binding($"dailyReportBankList[{i}].exit") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                        Border brdEnter = (Border)grid.Children[2];
                        BindingOperations.SetBinding(brdEnter.Child, TextBox.TextProperty, new Binding($"dailyReportBankList[{i}].entry") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                        Border brdUm = (Border)grid.Children[3];
                        BindingOperations.SetBinding(brdUm.Child, TextBlock.TextProperty, new Binding($"dailyReportBankList[{i++}].bankSum") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    }
                }
            }
            #endregion

            #region Currency List

            if (GrdCurrency.RowDefinitions.Count == 0)
            {
                int i = 0;
                if (Common.Cache.BankList != null)
                {
                    ControlTemplate CurrencyTemplate = GrdCurrency.Resources["CurrencyTemplate"] as ControlTemplate;

                    BaseObject.report.dailyReportCurrencyList.Clear();
                    foreach (var cur in Common.Cache.CurrencyList.OrderBy(x => x.id))
                    {
                        GrdCurrency.RowDefinitions.Add(new RowDefinition());

                        Grid grid = CurrencyTemplate.LoadContent() as Grid;
                        Grid.SetRow(grid, GrdCurrency.RowDefinitions.Count - 1);
                        GrdCurrency.Children.Add(grid);

                        Grid grd = (Grid)grid.Children[0];
                        Border brdTitle = (Border)grd.Children[0];
                        ((TextBlock)brdTitle.Child).Text = cur.name;

                        BaseObject.report.dailyReportCurrencyList.Add(new Common.Model.DailyReportCurrencyViewExtendModel((Common.Model.DailyReportViewExtendModel)BaseObject.report)
                        {
                            currencyId = cur.id
                            ,
                        });


                        Border brdValue = (Border)grd.Children[1];
                        BindingOperations.SetBinding(brdValue.Child, TextBox.TextProperty, new Binding($"dailyReportCurrencyList[{i}].value") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });


                        Border brdExit = (Border)grid.Children[1];
                        BindingOperations.SetBinding(brdExit.Child, TextBox.TextProperty, new Binding($"dailyReportCurrencyList[{i}].rialExit") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                        Border brdEnter = (Border)grid.Children[2];
                        BindingOperations.SetBinding(brdEnter.Child, TextBox.TextProperty, new Binding($"dailyReportCurrencyList[{i}].rialEntry") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                        Border brdUm = (Border)grid.Children[3];
                        BindingOperations.SetBinding(brdUm.Child, TextBlock.TextProperty, new Binding($"dailyReportCurrencyList[{i++}].CurrencySum") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    }
                }
            }

            #endregion

        }


        private async Task<Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel> LoadCalendarReportData()
        {
            if (CalendarData.ReportData != null)
            {
                return new Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel()
                {
                    date = CalendarData.date,
                    report = CalendarData.ReportData,
                    token = Common.Cache.CurrentUser.token
                };
            }
            else
            {
                dhWait.IsOpen = true;
                Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel draft = await LoadData(CalendarData.date);
                if (draft != null)
                {
                    CalendarData.ReportData = (Common.Model.DailyReportViewExtendModel)draft.report;
                }

                dhWait.IsOpen = false;
                return draft;
            }
        }


        private Task<Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel> LoadData(string Date)
        {
            return Task.Run(() =>
            {
                System.Collections.Specialized.NameValueCollection param = new System.Collections.Specialized.NameValueCollection
                {
                    ["date"] = CalendarData.date,
                    ["token"] = Common.Cache.CurrentUser.token
                };

                KiaGallery.Common.Response response = Common.Services.CallService(Common.Services.ServiceType.Load, param);
                if (response.status == 200)
                {
                    Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel viewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Web.Areas.DailyReportFinancial.Models.SaveDailyReportViewModel>(response.data.ToString());
                    if (viewModel != null)
                    {
                        return viewModel;
                    }

                }

                return null;

            });
        }
    }
}
