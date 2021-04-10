using KiaGallery.Common;
using KiaGallery.GoldReport.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace KiaGallery.GoldReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Timer
        private string _currenttime;
        private TimeZoneInfo _selectedTimeZone;
        public List<TimeZoneInfo> TimeZones
        {
            get { return TimeZoneInfo.GetSystemTimeZones().ToList(); }
        }

        public string CurrentTime
        {
            get { return _currenttime; }
            set { _currenttime = value; OnPropertyChanged("CurrentTime"); }
        }

        public TimeZoneInfo SelectedTimeZone
        {
            get { return _selectedTimeZone; }
            set
            {
                _selectedTimeZone = value;
                OnPropertyChanged("SelectedTimeZone");
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            CurrentTime = SelectedTimeZone == null
                   ? DateTime.Now.ToLongTimeString()
                   : DateTime.UtcNow.AddHours(SelectedTimeZone.BaseUtcOffset.TotalHours).ToLongTimeString();

            txtTime.Text = CurrentTime;

            if (CurrentTime == goldReport1 || CurrentTime == goldReport2 || CurrentTime == goldReport3)
            {
                Task.Factory.StartNew(() =>
                {
                    sendGoldReport();
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion

        string goldReport1 = "12:30:00 PM";
        string goldReport2 = "3:30:00 PM";
        string goldReport3 = "6:30:00 PM";

        private bool Running;
        private TelegramBotClient Bot;
        public MainWindow()
        {
            InitializeComponent();

            TxtApiCode.Text = "919052134:AAGcYm2Z_j4fYChAHkjW5pd4n-GlF9Qy9dI";

            #region Timer
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsEnabled = true;
            timer.Tick += (s, e) =>
            {
                UpdateTime();
            };
            #endregion
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            Bot = new TelegramBotClient(TxtApiCode.Text.Trim());
            BtnConnect.IsEnabled = false;
            LblStatus.Text = "Connecting...";
            Running = true;
            GetMe();
            Task.Factory.StartNew(() =>
            {
                GetUpdates();
            });
        }

        private async void GetMe()
        {
            var me = await Bot.GetMeAsync();
            LblStatus.Text = me.FirstName + " (" + me.Username + ")";
        }

        private async void GetUpdates()
        {
            int Offset = 0;
            long i = 0;

            while (Running)
            {
                try
                {
                    var updates = await Bot.GetUpdatesAsync(Offset);
                    foreach (var item in updates)
                    {
                        try
                        {
                            if (item.Type == UpdateType.Message)
                                ResponseMessage(item);
                        }
                        catch (Exception ex)
                        { }
                        Offset = item.Id + 1;
                    }
                    await Task.Delay(1000);
                    i++;
                }
                catch (Exception)
                { }
            }
        }

        private void sendGoldReport()
        {
            using (var db = new KiaGalleryMainEntities())
            {
                var data = db.BranchGolds.Where(x => x.CreateDate >= DateTime.Today).GroupBy(x => x.Branches).Select(y => new
                {
                    weight = y.OrderByDescending(x => x.Id).Select(x => x.Weight).FirstOrDefault(),
                    price = y.OrderByDescending(x => x.Id).Select(x => x.Price).FirstOrDefault(),
                    branch = y.Key.Name
                }).ToList();

                var persianDate = DateUtility.GetPersianDate(DateTime.Now);
                double sum = data.Sum(x => x.weight);
                long priceSum = data.Sum(x => x.price);
                var rate = priceSum / sum * 4.3318;
                string message = persianDate + "\nجمع کل: " + sum + " گرم" + "\n جمع قیمت: " + Core.ToSeparator(priceSum) + " ریال " + " \nمیانگین مظنه: " + Core.ToSeparator(rate) + " ریال " + "\n";

                foreach (var item in data)
                {
                    message += "شعبه " + item.branch + ": " + item.weight + " گرم " + Core.ToSeparator(item.price) + " ریال \n";
                }

                if (sum > 0)
                {
                    var user = db.BotGoldReportUserData.Where(x => x.BotUserType == 1).ToList();
                    user.ForEach(x =>
                    {
                        var task = Bot.SendTextMessageAsync(x.ChatId, message.Trim());
                        task.Wait();
                    });
                }
            }
        }

        private void ResponseMessage(Update item)
        {
            var Chat = item.Message.Chat;
            BotGoldReportUserData user = new BotGoldReportUserData()
            {
                UserType = (int)item.Message.Chat.Type,
                UserId = item.Message.From.Id,
                ChatId = Chat.Id,
                FirstName = Chat.FirstName,
                LastName = Chat.LastName,
                Username = Chat.Username,
                ModifyUserId = 1,
                Stoped = false
            };
            user = AddUser(user);
        }


        private BotGoldReportUserData AddUser(BotGoldReportUserData user)
        {
            using (var db = new KiaGalleryMainEntities())
            {
                var userEntity = db.BotGoldReportUserData.FirstOrDefault(x => x.ChatId == user.ChatId);
                if (userEntity == null)
                {
                    user.CreatedDate = DateTime.Now;
                    user.BotUserType = 0;
                    db.BotGoldReportUserData.Add(user);
                }
                else
                {
                    userEntity.UserType = user.UserType;
                    userEntity.UserId = user.UserId;
                    userEntity.ChatId = user.ChatId;
                    userEntity.FirstName = user.FirstName;
                    userEntity.LastName = user.LastName;
                    userEntity.Username = user.Username;
                }
                db.SaveChanges();
                user.BranchId = userEntity.BranchId;
                return userEntity;
            }
        }
    }
}
