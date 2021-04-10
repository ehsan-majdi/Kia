using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.BranchesPayments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace KiaGallery.BotBranchesPayments
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool Running;
        private TelegramBotClient Bot;
        private int Offset;

        List<BranchesPaymentsSettings> Settings;

        string RootPath = @"C:\Files\KiaGalleryBotBranchesPayments\";

        public MainWindow()
        {
            InitializeComponent();
            GetSettings();
            TxtApiCode.Text = Settings.Single(x => x.Key == "token").Value;
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            Bot = new TelegramBotClient(TxtApiCode.Text.Trim());
            BtnConnect.IsEnabled = false;
            BtnDisconnect.IsEnabled = true;
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
            Offset = int.Parse(Settings.Single(x => x.Key == "last-offset").Value);
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
                            if (item.Type == UpdateType.MessageUpdate)
                                ResponseMessage(item);
                            if (item.Type == UpdateType.CallbackQueryUpdate)
                                ResponseCallback(item);
                        }
                        catch (Exception ex)
                        {
                            SaveException(1, ex);
                        }
                        Offset = item.Id + 1;
                    }
                    UpdateOffset(Offset);
                    await Task.Delay(1000);
                    i++;
                }
                catch (Exception ex)
                {
                    SaveException(0, ex);
                }
            }
        }
        
        

        private void ResponseMessage(Update item)
        {
            var Chat = item.Message.Chat;
            bool hasMore = false;
            int? branchId = null;
            UserData user = new UserData()
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
            branchId = user.BranchId;
            var MainKeyBoard = new ReplyKeyboardMarkup();
            if(user.BranchId != 19)
            {
                MainKeyBoard.Keyboard = new KeyboardButton[][] {
                new KeyboardButton[] { "ته حساب", Enums.GetTitle(TypePayments.Deposits) },
                new KeyboardButton[] { Enums.GetTitle(TypePayments.Returned), Enums.GetTitle(TypePayments.Sale) },
                new KeyboardButton[] { Enums.GetTitle(TypePayments.DifferentReturns), Enums.GetTitle(TypePayments.DifferentSale) },
                new KeyboardButton[] { Enums.GetTitle(TypePayments.DifferentGoldReturns)}
                };
            }
            else
            {
                MainKeyBoard.Keyboard = new KeyboardButton[][] {
                new KeyboardButton[] { "ته حساب", Enums.GetTitle(TypePayments.Deposits) },
                new KeyboardButton[] { Enums.GetTitle(TypePayments.Returned), Enums.GetTitle(TypePayments.Sale) },
                new KeyboardButton[] { Enums.GetTitle(TypePayments.DifferentReturns), Enums.GetTitle(TypePayments.DifferentSale) },
                new KeyboardButton[] { Enums.GetTitle(TypePayments.DifferentGoldReturns), "منو" },
                };
            }

            var StartKeyBoard = new ReplyKeyboardMarkup();
            StartKeyBoard.Keyboard = new KeyboardButton[][] {new KeyboardButton[] {"منو"} };
            StartKeyBoard.ResizeKeyboard = true;
            MainKeyBoard.ResizeKeyboard = true;
            var TextMessage = item.Message.Text;
            if (user.BranchId == null &&  TextMessage == "/start" || TextMessage == "/keyboard")
            {
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "welcome-message").Value, ParseMode.Html, false, false, 0, StartKeyBoard);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            if (user.BranchId == null && TextMessage == "منو")
            {
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "active-message").Value, ParseMode.Html, false, false, 0, StartKeyBoard);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId == 19 && (TextMessage == "/start" || TextMessage == "/keyboard" || TextMessage == "منو"))
            {
                var BranchCityKeyboard = GenerateKeyBoard(GetBranchList(), 4, false, 0);
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "welcome-message").Value, ParseMode.Html, false, false, 0, BranchCityKeyboard);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && user.BranchId != 19 && (TextMessage == "/start" || TextMessage == "/keyboard" || TextMessage == "منو"))
            {
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "welcome-message").Value, ParseMode.Html, false, false, 0, MainKeyBoard);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && TextMessage == "ته حساب")
            {
                string result = GetFinancial(item.Message.Chat.Id, user.BranchId.GetValueOrDefault(), out branchId);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && TextMessage == Enums.GetTitle(TypePayments.Deposits))
            {
                var SelectedData = GetBranchesPayments(item.Message.Chat.Id, TypePayments.Deposits,user.BranchId,out branchId, 0, out hasMore);
                SendText(item.Message.Chat.Id, SelectedData, TypePayments.Deposits, 0, hasMore);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && TextMessage == Enums.GetTitle(TypePayments.Returned))
            {
                var SelectedData = GetBranchesPayments(item.Message.Chat.Id, TypePayments.Returned, user.BranchId, out branchId, 0, out hasMore);
                SendText(item.Message.Chat.Id, SelectedData, TypePayments.Returned, 0, hasMore);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && TextMessage == Enums.GetTitle(TypePayments.Sale))
            {
                var SelectedData = GetBranchesPayments(item.Message.Chat.Id, TypePayments.Sale, user.BranchId, out branchId, 0, out hasMore);
                SendText(item.Message.Chat.Id, SelectedData, TypePayments.Sale, 0, hasMore);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && TextMessage == Enums.GetTitle(TypePayments.DifferentReturns))
            {
                var SelectedData = GetBranchesPayments(item.Message.Chat.Id, TypePayments.DifferentReturns, user.BranchId, out branchId, 0, out hasMore);
                SendText(item.Message.Chat.Id, SelectedData, TypePayments.DifferentReturns, 0, hasMore);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && TextMessage == Enums.GetTitle(TypePayments.DifferentSale))
            {
                var SelectedData = GetBranchesPayments(item.Message.Chat.Id, TypePayments.DifferentSale, user.BranchId, out branchId, 0, out hasMore);
                SendText(item.Message.Chat.Id, SelectedData, TypePayments.DifferentSale, 0, hasMore);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && TextMessage == Enums.GetTitle(TypePayments.DifferentGoldReturns))
            {
                var SelectedData = GetBranchesPayments(item.Message.Chat.Id, TypePayments.DifferentGoldReturns, user.BranchId, out branchId, 0, out hasMore);
                SendText(item.Message.Chat.Id, SelectedData, TypePayments.DifferentGoldReturns, 0, hasMore);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else if (user.BranchId != null && TextMessage == Enums.GetTitle(TypePayments.DifferentGoldSale))
            {
                var SelectedData = GetBranchesPayments(item.Message.Chat.Id, TypePayments.DifferentGoldSale, user.BranchId, out branchId, 0, out hasMore);
                SendText(item.Message.Chat.Id, SelectedData, TypePayments.DifferentGoldSale, 0, hasMore);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, branchId);
            }
            else
            {
                if(user.BranchId == 19)
                {
                    int? selectedBranch = GetbranchId(TextMessage);
                    if (selectedBranch != null)
                        branchId = selectedBranch;
                    Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "type-payments").Value, ParseMode.Html, false, false, 0, MainKeyBoard);
                }
                else if(user.BranchId != null)
                {
                    Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "type-payments").Value, ParseMode.Html, false, false, 0, MainKeyBoard);
                }
                else
                {
                    Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "welcome-message").Value, ParseMode.Html, false, false, 0);
                }
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, true, branchId);
            }
        }
        private void SendText(long chatId ,List<string> list, TypePayments typePayments, int offset, bool hasMore)
        {
            Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string item = list[i];
                    InlineKeyboardMarkup CollectionKeyboard = new InlineKeyboardMarkup();
                    string moreData = "More:" + typePayments + ":" + (offset + 15);
                    if (hasMore && i + 1 == list.Count)
                    {
                        CollectionKeyboard.InlineKeyboard = new InlineKeyboardCallbackButton[][] {
                            new InlineKeyboardCallbackButton[] {new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "More").Value , moreData) }
                        };
                        await Bot.SendTextMessageAsync(chatId, item, ParseMode.Html, false, false, 0, CollectionKeyboard);
                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(chatId, item, ParseMode.Html, false, false, 0);
                    }
                    await Task.Delay(200);
                }
            });
        }


        private void ResponseCallback(Update item)
        {
            var QueryData = item.CallbackQuery.Data;
            var userItem = GetUserData(item.CallbackQuery.Message.Chat.Id);
            var data = QueryData.Split(':');
            if (data[0].ToLower() == "more")
            {
                int typePayments = int.Parse( data[1]);
                int from = int.Parse(data[2]);
                int? branchId = int.Parse(data[3]);

                bool hasMore = false;
                var SelectedData = GetBranchesPayments(userItem.ChatId, (TypePayments)typePayments, branchId, out branchId, from, out hasMore);
                SendText(userItem.ChatId, SelectedData, (TypePayments)typePayments, from, hasMore);
                AddMessage(userItem.ChatId, -1, "More " + typePayments + " " + from, item.CallbackQuery.Message.Date, false, branchId);

            }
        }


        private ReplyKeyboardMarkup GenerateKeyBoard(List<string> Titles, int ColumnSize, bool MenuButton, int More = 0, string CollectionType = "")
        {
            ReplyKeyboardMarkup Keyboard = new ReplyKeyboardMarkup();
            Keyboard.ResizeKeyboard = true;

            int RowSize = Titles.Count / ColumnSize;

            if (More > 0)
                RowSize++;

            if (Titles.Count % ColumnSize > 0)
                RowSize++;

            if (MenuButton)
                RowSize++;

            KeyboardButton[][] keys = new KeyboardButton[RowSize][];

            if (More > 0)
            {
                keys[0] = new KeyboardButton[] { Settings.First(x => x.Key == "More").Value + " - " + CollectionType + " - " + More + "-" + (More + 15) };
            }

            for (int i = 0; i < Titles.Count; i = i + ColumnSize)
            {
                List<string> key = new List<string>();
                for (int j = 0; j < ColumnSize; j++)
                {
                    if (i + j >= Titles.Count)
                        continue;
                    key.Add(Titles[i + j]);
                }
                var tempKey = key.ToArray();
                for (int x = 0; x < tempKey.Length; x++)
                {
                    if (More > 0)
                    {
                        if (keys[(i / ColumnSize) + 1] == null)
                            keys[(i / ColumnSize) + 1] = new KeyboardButton[tempKey.Length];
                        keys[(i / ColumnSize) + 1][x] = tempKey[x];
                    }
                    else
                    {
                        if (keys[i / ColumnSize] == null)
                            keys[i / ColumnSize] = new KeyboardButton[tempKey.Length];
                        keys[i / ColumnSize][x] = tempKey[x];
                    }
                }
            }

            if (MenuButton)
            {
                keys[RowSize - 1] = new KeyboardButton[] { Settings.First(x => x.Key == "Menu").Value };
            }

            Keyboard.Keyboard = keys;

            return Keyboard;
        }

        private UserData AddUser(UserData user)
        {
            using (var db = new KiaGalleryContext())
            {
                var userEntity = db.UserData.FirstOrDefault(x => x.ChatId == user.ChatId);
                if (userEntity == null)
                {
                    user.CreatedDate = DateTime.Now;
                    db.UserData.Add(user);
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

        private string GetFinancial(long chatId, int id,out int? branchId)
        {
            Branch item;
            using (var db = new KiaGalleryContext())
            {
                if (id == 19)
                {
                    var selected = db.BranchesPaymentsMessage.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ChatId == chatId).BranchId.GetValueOrDefault();
                    id = selected;
                    branchId = selected;
                }  
                else
                    branchId = id;
                item =  db.Branch.First(x => x.Id == id);
            }
            string text = "🌕 مانده بدهی طلایی: " + item.GoldDebt + " گرم "+ "\n";
            text += "💵 مانده بدهی: " + Core.ToSeparator(item.RialDebt) +" ریال "+ "\n";
           
            Bot.SendTextMessageAsync(chatId, text, ParseMode.Html, false, false, 0);
            return text;
        }

        /// <summary>
        /// گردش حساب شعب
        /// </summary>
        private List<string> GetBranchesPayments(long chatId, TypePayments typePayments,int? id, out int? branchId, int offset, out bool hasMore)
        {
            List<BranchesPayments> branchesPayments;
            List<string> textList = new List<string>();
            int count;
            string branch = "";
            using (var db = new KiaGalleryContext())
            {
                if (id == 19)
                {
                    branchId = db.BranchesPaymentsMessage.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ChatId == chatId).BranchId.GetValueOrDefault();
                    id = branchId;
                }
                else
                {
                    branchId = id;
                }
                branch = db.Branch.Single(x => x.Id == id).Name;
                var query = db.BranchesPayments.Where(x => x.BranchId == id && x.TypePayments == typePayments).OrderByDescending(x=>x.Id).Skip(offset);
                count = query.Count();
                branchesPayments = query.Take(15).ToList();
            }
            if (Offset + 15 >= count)
                hasMore = false;
            else
                hasMore = true;
            int index = 0;
            string date;
            string text, floatString="", details="";
            long totalAmount = 0;
            double totalGoldWage = 0;
            float totalGoldWeights = 0;
            double goldWeights;
            switch (typePayments)
                {
                    case TypePayments.Deposits:
                        branchesPayments.ForEach(x => {
                            date = DateUtility.GetPersianDate(x.CreateDate);
                            text = date + "🗓" + "  kia-" + x.Id.ToString() + "\n" + branch + " - " + Enums.GetTitle(x.TypePayments) + "\n";
                            double GoldWeights = Math.Round((x.GoldAmount.GetValueOrDefault() * 4.3318) / x.GoldFee.GetValueOrDefault(), 2);
                            text += "💵💵💵" + "\n";
                            if (GoldWeights.ToString().Split('.').Count() == 1)
                            {
                                floatString = ".00";
                            }
                            text += "مبلغ واریزی " + Core.ToSeparator(x.Deposits.GetValueOrDefault()) + " ریال " + "\n";
                            text += "مبلغ " + Core.ToSeparator(x.GoldAmount.GetValueOrDefault()) + " ریال با مظنه ی " + Core.ToSeparator(x.GoldFee.GetValueOrDefault()) + " ریال ";
                            text += "به " + GoldWeights + floatString + " گرم طلا تبدیل شد." + "\n";
                            text += "مبلغ " + Core.ToSeparator(x.GoldWage.GetValueOrDefault()) + " ریال " + "از اجرت کسر شد.";
                            text += "\n"; 
                            text += "🌕 مانده بدهی طلایی: " + x.GoldDebt + " گرم" + "\n";
                            text += "💵 مانده بدهی: " + Core.ToSeparator(x.RialDebt) + " ریال" + "\n";
                            if (x.Description != null && x.Description != "")
                                text += "توضیح: " + x.Description + "\n";
                            textList.Add(text);
                        });
                        break;
                    case TypePayments.DifferentGoldReturns:
                        branchesPayments.ForEach(x =>
                        {
                            date = DateUtility.GetPersianDate(x.CreateDate);
                            text = date + "🗓" + "  kia-" + x.Id.ToString() + "\n" + branch + " - " + Enums.GetTitle(x.TypePayments) + "\n";
                            text += "🌕↩️" + "\n";
                            goldWeights = Math.Round((x.GoldWeights.GetValueOrDefault() * x.GoldCarat.GetValueOrDefault()) / 750, 2);
                            if (goldWeights.ToString().Split('.').Count() == 1)
                            {
                                floatString = ".00";
                            }
                            text += "وزن " + x.GoldWeights + " گرم طلای متفرقه عیار " + Core.ToSeparator(x.GoldCarat.GetValueOrDefault()) + " به " + goldWeights + floatString + " گرم طلای عیار 750 تبدیل شد و از حساب طلایی کسر شد. ";
                            text += "\n";
                            text += "🌕 مانده بدهی طلایی: " + x.GoldDebt + " گرم" + "\n";
                            text += "💵 مانده بدهی: " + Core.ToSeparator(x.RialDebt) + " ریال" + "\n";
                            if (x.Description != null && x.Description != "")
                                text += "توضیح: " + x.Description + "\n";
                            textList.Add(text);
                        });
                        break;
                    case TypePayments.DifferentGoldSale:
                        branchesPayments.ForEach(x =>
                        {
                            date = DateUtility.GetPersianDate(x.CreateDate);
                            text = date + "🗓" + "  kia-" + x.Id.ToString() + "\n" + branch + " - " + Enums.GetTitle(x.TypePayments) + "\n";
                            text += "🌕⬆️" + "\n";
                            goldWeights = Math.Round((x.GoldWeights.GetValueOrDefault() * x.GoldCarat.GetValueOrDefault()) / 750, 2);
                            if (goldWeights.ToString().Split('.').Count() == 1)
                            {
                                floatString = ".00";
                            }
                            text += "وزن " + x.GoldWeights + " گرم طلای متفرقه عیار " + Core.ToSeparator(x.GoldCarat.GetValueOrDefault()) + " به " + goldWeights + floatString + " گرم طلای عیار 750 تبدیل شد و به حساب طلایی اضافه شد. ";
                            text += "\n";
                            text += "🌕 مانده بدهی طلایی: " + x.GoldDebt + " گرم" + "\n";
                            text += "💵 مانده بدهی: " + Core.ToSeparator(x.RialDebt) + " ریال" + "\n";
                            if (x.Description != null && x.Description != "")
                                text += "توضیح: " + x.Description + "\n";
                            textList.Add(text);
                        });
                        break;
                    case TypePayments.DifferentReturns:
                        index = 0;
                        branchesPayments.ForEach(x =>
                        {
                            date = DateUtility.GetPersianDate(x.CreateDate);
                            text = date + "🗓" + "  kia-" + x.Id.ToString() + "\n" + branch + " - " + Enums.GetTitle(x.TypePayments) + "\n";
                            text += "🎁↩️" + "\n";
                            long amount = 0;
                            x.BranchesPaymentsDetails.ForEach(y => {
                                index += 1;
                                details += index + " - ";
                                if (y.Number != null && y.Number != 0)
                                {
                                    details += "تعداد " + y.Number + " عدد ";
                                    amount += y.Amount.GetValueOrDefault() * y.Number.GetValueOrDefault();
                                }
                                else
                                {
                                    amount += y.Amount.GetValueOrDefault();
                                }
                                details += y.Title + " به ارزش " + Core.ToSeparator(y.Amount.GetValueOrDefault()) + " ریال" + "\n";
                            });
                            text += Core.ToSeparator(amount) + "ریال از حساب اجرت کسر شد.";
                            text += "\n" + "توضیحات:" + "\n" + details;
                            text += "\n";
                            text += "🌕 مانده بدهی طلایی: " + x.GoldDebt + " گرم" + "\n";
                            text += "💵 مانده بدهی: " + Core.ToSeparator(x.RialDebt) + " ریال" + "\n";
                            if (x.Description != null && x.Description != "")
                                text += "توضیح: " + x.Description + "\n";
                            textList.Add(text);
                        });
                    break;
                    case TypePayments.DifferentSale:
                        branchesPayments.ForEach(x =>
                        {
                            date = DateUtility.GetPersianDate(x.CreateDate);
                            text = date + "🗓" + "  kia-" + x.Id.ToString() + "\n" + branch + " - " + Enums.GetTitle(x.TypePayments) + "\n";
                            text += "🎁⬆️" + "\n";
                            long Amount = 0;
                            x.BranchesPaymentsDetails.ForEach(y => {
                                index += 1;
                                details += index + " - ";
                                if (y.Number != null && y.Number != 0)
                                {
                                    details += "تعداد " + x.Number + " عدد";
                                    Amount += y.Amount.GetValueOrDefault() * y.Number.GetValueOrDefault();
                                }
                                else
                                {
                                    Amount += y.Amount.GetValueOrDefault();
                                }
                                details += y.Title + " به ارزش " + Core.ToSeparator(y.Amount.GetValueOrDefault()) + " ریال" + "\n";
                            });
                            text += Core.ToSeparator(Amount) + "ریال به حساب اجرت اضافه شد.";
                            text += "\n" + "توضیحات:" + "\n" + details;
                            text += "\n";
                            text += "🌕 مانده بدهی طلایی: " + x.GoldDebt + " گرم" + "\n";
                            text += "💵 مانده بدهی: " + Core.ToSeparator(x.RialDebt) + " ریال" + "\n";
                            if (x.Description != null && x.Description != "")
                                text += "توضیح: " + x.Description + "\n";
                            textList.Add(text);
                        });
                        break;
                    case TypePayments.Returned:
                        branchesPayments.ForEach(x =>
                        {
                            date = DateUtility.GetPersianDate(x.CreateDate);
                            text = date + "🗓" + "  kia-" + x.Id.ToString() + "\n" + branch + " - " + Enums.GetTitle(x.TypePayments) + "\n";
                            text += "↩️↩️↩️" + "\n";
                            double goldWage = 0;
                            index = 0;
                            x.BranchesPaymentsDetails.ForEach(y => {
                                index += 1;
                                goldWage = y.GoldWeights.GetValueOrDefault() * y.GoldWage.GetValueOrDefault();
                                totalGoldWage += goldWage;
                                totalAmount += y.Amount.GetValueOrDefault();
                                totalGoldWeights += y.GoldWeights.GetValueOrDefault();
                                details += index + " - " + y.Title + " به وزن " + y.GoldWeights + "گرم با اجرت " + Core.ToSeparator(goldWage) + " ریال " + "(" + " اجرت واحد " + Core.ToSeparator(y.GoldWage.GetValueOrDefault()) + " ریال " + ") " + " و سنگ و چرم " + Core.ToSeparator(y.Amount.GetValueOrDefault()) + " ریال - " + " کدشخصی:  " + y.Code + "\n";
                            });
                            if (totalGoldWeights.ToString().Split('.').Count() == 1)
                            {
                                floatString = ".00";
                            }
                            text += "مجموع " + Math.Round(totalGoldWeights, 2) + floatString + " گرم طلا با اجرت " + Core.ToSeparator(totalGoldWage) + " ریال و " + Core.ToSeparator(totalAmount) + "ریال سنگ و چرم";
                            text += " از حساب کسر شد. ";
                            text += "\n" + "توضیحات:" + "\n" + details;
                            text += "\n";
                            text += "🌕 مانده بدهی طلایی: " + x.GoldDebt + " گرم" + "\n";
                            text += "💵 مانده بدهی: " + Core.ToSeparator(x.RialDebt) + " ریال" + "\n";
                            if (x.Description != null && x.Description != "")
                                text += "توضیح: " + x.Description + "\n";
                            textList.Add(text);
                        });
                    break;
                    case TypePayments.Sale:
                        branchesPayments.ForEach(x =>
                        {
                            date = DateUtility.GetPersianDate(x.CreateDate);
                            text = date + "🗓" + "  kia-" + x.Id.ToString() + "\n" + branch + " - " + Enums.GetTitle(x.TypePayments) + "\n";
                            text += "⬆️⬆️⬆️" + "\n";
                            double GoldWage = 0;
                            index = 0;
                            x.BranchesPaymentsDetails.ForEach(y => {
                                index += 1;
                                GoldWage = y.GoldWeights.GetValueOrDefault() * y.GoldWage.GetValueOrDefault();
                                totalGoldWage += GoldWage;
                                totalAmount += y.Amount.GetValueOrDefault();
                                totalGoldWeights += y.GoldWeights.GetValueOrDefault();
                                details += index + " - " + y.Title + " به وزن " + y.GoldWeights + "گرم با اجرت " + Core.ToSeparator(GoldWage) + " ریال " + "(" + " اجرت واحد " + Core.ToSeparator(y.GoldWage.GetValueOrDefault()) + " ریال " + ") " + " و سنگ و چرم " + Core.ToSeparator(y.Amount.GetValueOrDefault()) + " ریال - " + " کدشخصی:  " + y.Code + "\n";
                            });
                            if (totalGoldWeights.ToString().Split('.').Count() == 1)
                            {
                                floatString = ".00";
                            }
                            text += "مجموع " + Math.Round(totalGoldWeights, 2) + floatString + " گرم طلا با اجرت " + Core.ToSeparator(totalGoldWage) + " ریال و " + Core.ToSeparator(totalAmount) + "ریال سنگ و چرم";
                            text += " به حساب اضافه شد. ";
                            text += "\n" + "توضیحات:" + "\n" + details;
                            text += "\n";
                            text += "🌕 مانده بدهی طلایی: " + x.GoldDebt + " گرم" + "\n";
                            text += "💵 مانده بدهی: " + Core.ToSeparator(x.RialDebt) + " ریال" + "\n";
                            if (x.Description != null && x.Description != "")
                                text += "توضیح: " + x.Description + "\n";
                            textList.Add(text);
                        });
                    break;
                }
            return textList;
        }

        public static void AddMessage(long chatId, int messageId, string text, DateTime date, bool unknown, int? branchId)
        {
            using (var db = new KiaGalleryContext())
            {
                var _Message = new BranchesPaymentsMessage()
                {
                    ChatId = chatId,
                    MessageId = messageId,
                    Text = text,
                    CreateDate = date.ToUniversalTime(),
                    Unknown = unknown,
                    BranchId = branchId
                };
                db.BranchesPaymentsMessage.Add(_Message);
                db.SaveChanges();
            }
        }

        private UserData GetUserData(long id)
        {
            using (var db = new KiaGalleryContext())
            {
                return db.UserData.First(x => x.ChatId == id);
            }
        }

        private int? GetbranchId(string name)
        {
            using (var db = new KiaGalleryContext())
            {
                return db.Branch.FirstOrDefault(x => x.Name.Contains(name)).Id;
            }
        }

        private void SaveException(int Code, Exception ex)
        {
            string filePath = RootPath + @"KiaGalleryBotBranchesPayments.txt";

            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Code: " + Code + Environment.NewLine + "Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-" + Environment.NewLine);
            }
        }

        private void UpdateOffset(int offset)
        {
            using (var db = new KiaGalleryContext())
            {
                var entity = db.BranchesPaymentsSettings.SingleOrDefault(x => x.Key == "last-offset");
                entity.Value = offset.ToString();
                db.SaveChanges();
            }
        }

        private void GetSettings()
        {
            using (var db = new KiaGalleryContext())
            {
                Settings = db.BranchesPaymentsSettings.ToList();
            }
        }
       
        private List<string> GetBranchList()
        {
            List<string> branchNameList;
            using (var db = new KiaGalleryContext())
            {
                branchNameList = db.Branch.Where(x=> x.City.Name !="شهر تهران").Select(x=> x.Name).ToList();
            }
            return branchNameList;
        }
        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Running = false;
            BtnConnect.IsEnabled = true;
            BtnDisconnect.IsEnabled = false;
            LblStatus.Text = "Disconnect.";
        }
    }
}