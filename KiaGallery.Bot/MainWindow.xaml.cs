using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Bot;
using KiaGallery.Model.Context.BranchesPayments;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace KiaGallery.Bot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // OrderStatus: 0:None 1:Pending Call 2: Reject Call 3:Pending Prepayment 4:Under Construction 5:Pending Payment 6:Sent 7:Canceled

        private bool Running;
        private bool SendingBroadcast = false;
        private TelegramBotClient Bot;
        private int Offset;

        List<BotSettings> Settings;

        string RootPath = "C:/Files/KiaGalleryBot/";

        //        string workingHourText = @"به اطلاع شما دوستان عزيز مي رساند ساعت كاري شعب گالري كيا در ايام نوروز به شرح زير مي باشد: 
        //📍تهران(فقط شهرك غرب ٥ تا ١٠ فروردين از ساعت ١١ تا ١٣:٣٠ و ١٦:٣٠ تا ٢٠)
        //📍لواسان(١ تا ١٢ فروردين از ساعت ١٥ تا ٢١)
        //📍كرج(از ٥ تا ١٠ فروردين از ساعت ١٠:٣٠ تا ١٣:٣٠ و ١٦:٣٠ تا ٢١:٣٠)
        //📍ساري(٨ و ٩ فروردين از ساعت ١٠ تا ١٣ و ١٧ تا ٢٠:٣٠ - ١٠ فروردين از ساعت ١٠ تا ١٣:٣٠)
        //📍ايزدشهر(١ تا ١٣ فروردين از ساعت ١١ تا ٢٢:٣٠)
        //📍رشت(٢ تا ١١ فروردين از ساعت ١٠ تا ١٤ و ١٧ تا ٢٢)
        //📍شيراز(٣ تا ١٠ فروردين از ساعت ١٠ تا ١٣:٣٠ و ١٧:٣٠ تا ٢١:٣٠)
        //📍اصفهان(٥ تا ١٠ فروردين از ساعت ١٠:٣٠ تا ١٣:٣٠ و ١٧:٣٠ تا ٢١)
        //📍كيش(١ تا ١٣ فروردين از ساعت ١٠ تا ١٣:٣٠ و ١٨ تا ٠٠:٠٠)
        //📍مشهد(٢ و ٣ فروردين و ٥ تا ١٠ فروردين از ساعت ١٠ تا ١٤ و ١٧ تا ٢١)
        //📍تبريز(١ تا ١٢ فروردين از ساعت ١٠ تا ٢٣)
        //در خدمت شما مي باشند.";
        public MainWindow()
        {
            InitializeComponent();

            Settings = DbContext.GetSettings();
            TxtApiCode.Text = Settings.First(x => x.Key == "BotApi").Value;
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
                //UpdateInstagram();
            });
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Running = false;
            BtnConnect.IsEnabled = true;
            BtnDisconnect.IsEnabled = false;
            LblStatus.Text = "Disconnect.";
        }

        private async void GetMe()
        {
            var me = await Bot.GetMeAsync();
            LblStatus.Text = me.FirstName + " (" + me.Username + ")";
        }

        private async void GetUpdates()
        {
            Offset = DbContext.GetLastOffset();
            long i = 0;
            while (Running)
            {
                try
                {
                    if (i % 5 == 0)
                    {
                        Settings = DbContext.GetSettings();
                        Dispatcher.Invoke(() =>
                        {
                            CheckBroadCast();
                        });
                    }
                    //if (i % (60 * 5) == 0)
                    //{
                    //    Dispatcher.Invoke(() =>
                    //    {
                    //        GetInstagramUpdates();
                    //    });
                    //}
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
                    DbContext.UpdateOffset(Offset);
                    await Task.Delay(1000);
                    i++;
                }
                catch (Exception ex)
                {
                    SaveException(0, ex);
                }
            }
        }

        private void CheckBroadCast()
        {
            if (SendingBroadcast) return;

            List<Broadcast> data = DbContext.GetNotSentBroadcast();

            if (data.Count == 0) return;

            SendingBroadcast = true;
            LblBroadcastStatus.Text = "Sending Broadcast";
            Task.Factory.StartNew(async () =>
            {
                var users = DbContext.GetAllUsers();

                var PersianMainKeyBoard = new ReplyKeyboardMarkup();
                PersianMainKeyBoard.Keyboard = new KeyboardButton[][] {
                    //new KeyboardButton[] { Settings.First(x => x.Key == "SpecialOffer").ValueFa },
                    //new KeyboardButton[] { Settings.First(x => x.Key == "WorkingHour").ValueFa },
                    new KeyboardButton[] { Settings.First(x => x.Key == "ContactUs").ValueFa, Settings.First(x => x.Key == "Branch").ValueFa },
                    new KeyboardButton[] { Settings.First(x => x.Key == "Collection").ValueFa, Settings.First(x => x.Key == "News").ValueFa },
                    new KeyboardButton[] { Settings.First(x => x.Key == "Persian").Value, Settings.First(x => x.Key == "English").Value }
                };
                PersianMainKeyBoard.ResizeKeyboard = true;

                var EnglishMainKeyBoard = new ReplyKeyboardMarkup();
                EnglishMainKeyBoard.Keyboard = new KeyboardButton[][] {
                    //new KeyboardButton[] { Settings.First(x => x.Key == "SpecialOffer").Value },
                    //new KeyboardButton[] { Settings.First(x => x.Key == "WorkingHour").Value },
                    new KeyboardButton[] { Settings.First(x => x.Key == "ContactUs").Value, Settings.First(x => x.Key == "Branch").Value },
                    new KeyboardButton[] { Settings.First(x => x.Key == "Collection").Value, Settings.First(x => x.Key == "News").Value },
                    new KeyboardButton[] { Settings.First(x => x.Key == "Persian").Value, Settings.First(x => x.Key == "English").Value }
                };
                EnglishMainKeyBoard.ResizeKeyboard = true;

                for (int i = 0; i < data.Count; i++)
                {
                    int dateTime = DateTime.Now.Hour;
                    if (dateTime >= 23 || (dateTime >= 0 && dateTime <= 8))
                        break;
                    var item = data[i];

                    switch (item.BroadcastType)
                    {
                        case BotType.Text:
                            for (int j = 0; j < users.Count; j++)
                            {
                                dateTime = DateTime.Now.Hour;
                                if (dateTime >= 23 || (dateTime >= 0 && dateTime <= 8))
                                    break;
                                try
                                {
                                    var useritem = users[j];
                                    if (useritem.Language == 0)
                                    {
                                        await Bot.SendTextMessageAsync(useritem.ChatId, item.TextFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                                    }
                                    else
                                    {
                                        await Bot.SendTextMessageAsync(useritem.ChatId, item.Text, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                                    }
                                    Dispatcher.Invoke(() =>
                                    {
                                        LblBroadcastStatus.Text = "Sending Broadcast " + j;
                                    });
                                    await Task.Delay(50);
                                }
                                catch (Exception ex)
                                {
                                    SaveException(10, ex);
                                }
                            }
                            break;
                        case BotType.Image:
                            string fileId = item.FileId;
                            var filePath = RootPath + "Broadcast/" + item.FileName;
                            for (int j = 0; j < users.Count; j++)
                            {
                                dateTime = DateTime.Now.Hour;
                                if (dateTime >= 23 || (dateTime >= 0 && dateTime <= 8))
                                    break;
                                try
                                {
                                    var userItem = users[j];
                                    if (string.IsNullOrEmpty(fileId))
                                    {
                                        if (string.IsNullOrEmpty(item.FileName)) break;

                                        Stream stream = System.IO.File.OpenRead(filePath);
                                        FileToSend file = new FileToSend(System.IO.Path.GetFileName(filePath), stream);
                                        Task<Telegram.Bot.Types.Message> result = null;

                                        if (userItem.Language == 0)
                                        {
                                            if (item.TextFa.Length <= 199)
                                            {
                                                result = Bot.SendPhotoAsync(userItem.ChatId, file, item.TextFa, false, 0, PersianMainKeyBoard);
                                            }
                                            else
                                            {
                                                result = Bot.SendPhotoAsync(userItem.ChatId, file, "-");
                                                await Bot.SendTextMessageAsync(userItem.ChatId, item.TextFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                                            }
                                        }
                                        else
                                        {
                                            if (item.TextFa.Length <= 199)
                                            {
                                                result = Bot.SendPhotoAsync(userItem.ChatId, file, item.Text, false, 0, EnglishMainKeyBoard);
                                            }
                                            else
                                            {
                                                result = Bot.SendPhotoAsync(userItem.ChatId, file, "-");
                                                await Bot.SendTextMessageAsync(userItem.ChatId, item.Text, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                                            }
                                        }

                                        fileId = result.Result.Photo[0].FileId;
                                        item.FileId = fileId;
                                        DbContext.UpdateBroadCastFileId(item, fileId);
                                    }
                                    else
                                    {
                                        FileToSend file = new FileToSend(fileId);
                                        if (userItem.Language == 0)
                                        {
                                            
                                            if (item.TextFa.Length <= 199)
                                            {
                                                await Bot.SendPhotoAsync(userItem.ChatId, file, item.TextFa, false, 0, PersianMainKeyBoard);
                                            }
                                            else
                                            {
                                                await Bot.SendPhotoAsync(userItem.ChatId, file, "-");
                                                await Bot.SendTextMessageAsync(userItem.ChatId, item.TextFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                                            }
                                        }
                                        else
                                        {
                                            if (item.TextFa.Length <= 199)
                                            {
                                                await Bot.SendPhotoAsync(userItem.ChatId, file, item.Text, false, 0, EnglishMainKeyBoard);
                                            }
                                            else
                                            {
                                                await Bot.SendPhotoAsync(userItem.ChatId, file, "-");
                                                await Bot.SendTextMessageAsync(userItem.ChatId, item.Text, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                                            }
                                        }
                                    }

                                    Dispatcher.Invoke(() =>
                                    {
                                        LblBroadcastStatus.Text = "Sending Broadcast " + j;
                                    });
                                    await Task.Delay(50);
                                }
                                catch (Exception ex)
                                {
                                    SaveException(11, ex);
                                }
                            }
                            break;
                        //case BroadcastType.Video:
                        //    string videoFileId = item.FileId;
                        //    var videoFilePath = RootPath + "Broadcast/" + item.FileName;
                        //    for (int j = 0; j < users.Count; j++)
                        //    {
                        //        dateTime = DateTime.Now.Hour;
                        //        if (dateTime >= 23 || (dateTime >= 0 && dateTime <= 8))
                        //            break;
                        //        var userItem = users[j];
                        //        if (string.IsNullOrEmpty(videoFileId))
                        //        {
                        //            if (string.IsNullOrEmpty(item.FileName)) break;

                        //            Stream stream = System.IO.File.OpenRead(videoFilePath);
                        //            FileToSend file = new FileToSend(System.IO.Path.GetFileName(videoFilePath), stream);

                        //            Task<Telegram.Bot.Types.Message> result = null;
                        //            if (userItem.Language == 0)
                        //            {
                        //                if (item.TextFa.Length <= 199)
                        //                {
                        //                    result = Bot.SendVideoAsync(userItem.ChatId, file, 0, item.TextFa, false, 0, PersianMainKeyBoard);
                        //                }
                        //                else
                        //                {
                        //                    result = Bot.SendVideoAsync(userItem.ChatId, file, 0, "-");
                        //                    await Bot.SendTextMessageAsync(userItem.ChatId, item.TextFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                if (item.TextFa.Length <= 199)
                        //                {
                        //                    result = Bot.SendVideoAsync(userItem.ChatId, file, 0, item.Text, false, 0, EnglishMainKeyBoard);
                        //                }
                        //                else
                        //                {
                        //                    result = Bot.SendVideoAsync(userItem.ChatId, file, 0, "-");
                        //                    await Bot.SendTextMessageAsync(userItem.ChatId, item.Text, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                        //                }
                        //            }

                        //            videoFileId = result.Result.Video.FileId;
                        //            item.FileId = videoFileId;
                        //            DbContext.UpdateBroadCastFileId(item, videoFileId);
                        //        }
                        //        else
                        //        {
                        //            if (userItem.Language == 0)
                        //            {
                        //                if (item.TextFa.Length <= 199)
                        //                {
                        //                    await Bot.SendVideoAsync(userItem.ChatId, videoFileId, 0, item.TextFa, false, 0, PersianMainKeyBoard);
                        //                }
                        //                else
                        //                {
                        //                    await Bot.SendVideoAsync(userItem.ChatId, videoFileId, 0, "-");
                        //                    await Bot.SendTextMessageAsync(userItem.ChatId, item.TextFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                if (item.TextFa.Length <= 199)
                        //                {
                        //                    await Bot.SendVideoAsync(userItem.ChatId, videoFileId, 0, item.Text, false, 0, EnglishMainKeyBoard);
                        //                }
                        //                else
                        //                {
                        //                    await Bot.SendVideoAsync(userItem.ChatId, videoFileId, 0, "-");
                        //                    await Bot.SendTextMessageAsync(userItem.ChatId, item.Text, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                        //                }
                        //            }
                        //        }

                        //        Dispatcher.Invoke(() =>
                        //        {
                        //            LblBroadcastStatus.Text = "Sending Broadcast " + j;
                        //        });
                        //        await Task.Delay(50);
                        //    }
                        //    break;
                        //case BroadcastType.DailyOffer:

                        //    InlineKeyboardMarkup CollectionKeyboard = new InlineKeyboardMarkup();
                        //    string orderData = "Daily:" + item.ProductId + ":" + item.Id;

                        //    CollectionKeyboard.InlineKeyboard = new InlineKeyboardCallbackButton[][] {
                        //    new InlineKeyboardCallbackButton[] {new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "Order").ValueFa, orderData) }
                        //    };
                        //    string dailyFileId = item.FileId;
                        //    var dailyFilePath = RootPath + "Broadcast/" + item.FileName;
                        //    for (int j = 0; j < users.Count; j++)
                        //    {
                        //        dateTime = DateTime.Now.Hour;
                        //        if (dateTime >= 23 || (dateTime >= 0 && dateTime <= 8))
                        //            break;
                        //        try
                        //        {
                        //            var userItem = users[j];
                        //            if (string.IsNullOrEmpty(dailyFileId))
                        //            {
                        //                if (string.IsNullOrEmpty(item.FileName)) break;

                        //                Stream stream = System.IO.File.OpenRead(dailyFilePath);
                        //                FileToSend file = new FileToSend(System.IO.Path.GetFileName(dailyFilePath), stream);
                        //                Task<Telegram.Bot.Types.Message> result = null;

                        //                if (userItem.Language == 0)
                        //                {
                        //                    if (item.TextFa.Length <= 199)
                        //                    {
                        //                        result = Bot.SendPhotoAsync(userItem.ChatId, file, item.TextFa, false, 0, CollectionKeyboard);
                        //                    }
                        //                    else
                        //                    {
                        //                        result = Bot.SendPhotoAsync(userItem.ChatId, file, "-");
                        //                        await Bot.SendTextMessageAsync(userItem.ChatId, item.TextFa, ParseMode.Html, false, false, 0, CollectionKeyboard);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    if (item.TextFa.Length <= 199)
                        //                    {
                        //                        result = Bot.SendPhotoAsync(userItem.ChatId, file, item.Text, false, 0, CollectionKeyboard);
                        //                    }
                        //                    else
                        //                    {
                        //                        result = Bot.SendPhotoAsync(userItem.ChatId, file, "-");
                        //                        await Bot.SendTextMessageAsync(userItem.ChatId, item.Text, ParseMode.Html, false, false, 0, CollectionKeyboard);
                        //                    }
                        //                }

                        //                dailyFileId = result.Result.Photo[0].FileId;
                        //                item.FileId = dailyFileId;
                        //                DbContext.UpdateBroadCastFileId(item, dailyFileId);
                        //            }
                        //            else
                        //            {
                        //                if (userItem.Language == 0)
                        //                {
                        //                    if (item.TextFa.Length <= 199)
                        //                    {
                        //                        await Bot.SendPhotoAsync(userItem.ChatId, dailyFileId, item.TextFa, false, 0, CollectionKeyboard);
                        //                    }
                        //                    else
                        //                    {
                        //                        await Bot.SendPhotoAsync(userItem.ChatId, dailyFileId, "-");
                        //                        await Bot.SendTextMessageAsync(userItem.ChatId, item.TextFa, ParseMode.Html, false, false, 0, CollectionKeyboard);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    if (item.TextFa.Length <= 199)
                        //                    {
                        //                        await Bot.SendPhotoAsync(userItem.ChatId, dailyFileId, item.Text, false, 0, CollectionKeyboard);
                        //                    }
                        //                    else
                        //                    {
                        //                        await Bot.SendPhotoAsync(userItem.ChatId, dailyFileId, "-");
                        //                        await Bot.SendTextMessageAsync(userItem.ChatId, item.Text, ParseMode.Html, false, false, 0, CollectionKeyboard);
                        //                    }
                        //                }
                        //            }

                        //            Dispatcher.Invoke(() =>
                        //            {
                        //                LblBroadcastStatus.Text = "Sending Broadcast " + j;
                        //            });
                        //            await Task.Delay(50);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            SaveException(11, ex);
                        //        }
                        //    }
                        //    break;
                    }
                }

                SendingBroadcast = false;
                Dispatcher.Invoke(() =>
                {
                    LblBroadcastStatus.Text = "";
                });
            });
        }

        private void ResponseMessage(Update item)
        {
            var PersianMainKeyBoard = new ReplyKeyboardMarkup();
            PersianMainKeyBoard.Keyboard = new KeyboardButton[][] {
                //new KeyboardButton[] { Settings.First(x => x.Key == "SpecialOffer").ValueFa },
                //new KeyboardButton[] { Settings.First(x => x.Key == "WorkingHour").ValueFa },
                new KeyboardButton[] { Settings.First(x => x.Key == "ContactUs").ValueFa, Settings.First(x => x.Key == "Branch").ValueFa },
                new KeyboardButton[] { Settings.First(x => x.Key == "Collection").ValueFa, Settings.First(x => x.Key == "News").ValueFa },
                new KeyboardButton[] { Settings.First(x => x.Key == "Persian").Value, Settings.First(x => x.Key == "English").Value }
            };
            PersianMainKeyBoard.ResizeKeyboard = true;

            var EnglishMainKeyBoard = new ReplyKeyboardMarkup();
            EnglishMainKeyBoard.Keyboard = new KeyboardButton[][] {
                //new KeyboardButton[] { Settings.First(x => x.Key == "SpecialOffer").Value },
                //new KeyboardButton[] { Settings.First(x => x.Key == "WorkingHour").Value },
                new KeyboardButton[] { Settings.First(x => x.Key == "ContactUs").Value, Settings.First(x => x.Key == "Branch").Value },
                new KeyboardButton[] { Settings.First(x => x.Key == "Collection").Value, Settings.First(x => x.Key == "News").Value },
                new KeyboardButton[] { Settings.First(x => x.Key == "Persian").Value, Settings.First(x => x.Key == "English").Value }
            };
            EnglishMainKeyBoard.ResizeKeyboard = true;


            var Chat = item.Message.Chat;

            BotUserData user = new BotUserData()
            {
                UserType = (int)item.Message.Chat.Type,
                UserId = item.Message.From?.Id,
                ChatId = Chat.Id,
                FirstName = Chat.FirstName,
                LastName = Chat.LastName,
                Username = Chat.Username,
                Stoped = false
            };
            user = DbContext.AddUser(user);

            if (item.Message.Contact != null)
            {
                if (DbContext.GetPhoneNumber(item.Message.Contact))
                {
                    if (user.Language == 0)
                        Bot.SendTextMessageAsync(item.Message.Chat.Id, Settings.First(x => x.Key == "OrderDone").ValueFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                    else
                        Bot.SendTextMessageAsync(item.Message.Chat.Id, Settings.First(x => x.Key == "OrderDone").Value, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                }
                else
                {
                    if (user.Language == 0)
                    {
                        Bot.SendTextMessageAsync(item.CallbackQuery.Message.Chat.Id, Settings.First(x => x.Key == "OutOfStock").ValueFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                    }
                    else
                    {
                        Bot.SendTextMessageAsync(item.CallbackQuery.Message.Chat.Id, Settings.First(x => x.Key == "OutOfStock").Value, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                    }
                }

                return;
            }

            var TextMessage = item.Message.Text;
            if (TextMessage == "/start")
            {
                var LanguageKeyBoard = new ReplyKeyboardMarkup();
                LanguageKeyBoard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { Settings.First(x => x.Key == "Persian").Value, Settings.First(x => x.Key == "English").Value } };
                LanguageKeyBoard.ResizeKeyboard = true;

                Bot.SendTextMessageAsync(item.Message.Chat.Id, Settings.First(x => x.Key == "SelectLanguageText").Value, ParseMode.Html, false, false, 0, LanguageKeyBoard);
                DbContext.AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == "/keyboard" || TextMessage == Settings.First(x => x.Key == "Menu").ValueFa || TextMessage == Settings.First(x => x.Key == "Menu").Value)
            {
                if (user.Language == 0) // Persian
                {
                    Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "WelcomeMessage").ValueFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                }
                else if (user.Language == 1) // English
                {
                    Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "WelcomeMessage").Value, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                }
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }

            else if (TextMessage == Settings.First(x => x.Key == "Cancel").ValueFa)
            {
                DbContext.CancelOrder(user);
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "CancelText").ValueFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "Cancel").Value)
            {
                DbContext.CancelOrder(user);
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "CancelText").Value, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "Persian").Value)
            {
                DbContext.ChangeLanguage(user, 0);
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "WelcomeMessage").ValueFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "English").Value)
            {
                DbContext.ChangeLanguage(user, 1);
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "WelcomeMessage").Value, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "Branch").ValueFa)
            {
                var BranchCityKeyboard = GenerateKeyBoard(DbContext.GetCityList().Select(x => x.Name).ToList(), 4, true, 0);
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "ChooseCity").ValueFa, ParseMode.Html, false, false, 0, BranchCityKeyboard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "Branch").Value)
            {
                var BranchCityKeyboard = GenerateKeyBoard(DbContext.GetCityList().Select(x => x.EnglishName).ToList(), 4, true, 1);
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "ChooseCity").Value, ParseMode.Html, false, false, 0, BranchCityKeyboard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "ContactUs").ValueFa)
            {
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "ContactData").ValueFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "ContactUs").Value)
            {
                Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "ContactData").Value, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "SpecialOffer").ValueFa)
            {
                bool hasMore = false;
                ProductType? productType = GetProductType(TextMessage);
                if(productType != null)
                {
                    var SelectedCollection = DbContext.GetData(productType.GetValueOrDefault(), 0, out hasMore);
                    if (SelectedCollection != null && SelectedCollection.Count > 0)
                    {
                        SendFiles(user, SelectedCollection, TextMessage, 0, hasMore);
                        DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
                        return;
                    }
                }
                
            }
            else if (TextMessage == Settings.First(x => x.Key == "SpecialOffer").Value)
            {
                bool hasMore = false;
                ProductType? productType = GetProductType(TextMessage);
                if(productType != null)
                {
                    var SelectedCollection = DbContext.GetData(productType.GetValueOrDefault(), 0, out hasMore);
                    if (SelectedCollection != null && SelectedCollection.Count > 0)
                    {
                        SendFiles(user, SelectedCollection, TextMessage, 0, hasMore);
                        DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
                        return;
                    }
                }
            }
            //else if (TextMessage == Settings.First(x => x.Key == "WorkingHour").ValueFa)
            //{
            //    Bot.SendTextMessageAsync(Chat.Id, workingHourText, false, false, 0, PersianMainKeyBoard);
            //    DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            //}
            //else if (TextMessage == Settings.First(x => x.Key == "WorkingHour").Value)
            //{
            //    Bot.SendTextMessageAsync(Chat.Id, workingHourText, false, false, 0, EnglishMainKeyBoard);
            //    DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            //}
            else if (TextMessage == Settings.First(x => x.Key == "Collection").ValueFa)
            {
                List<string> SelectedTypeKeyBoard = GetProductTypeTitle();
                var TypeKeyboard = GenerateKeyBoard(SelectedTypeKeyBoard, 3, true, 0);
                Bot.SendTextMessageAsync(item.Message.Chat.Id, Settings.First(x => x.Key == "SelectCollectionType").ValueFa, ParseMode.Html, false, false, 0, TypeKeyboard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "Collection").Value)
            {
                List<string> SelectedTypeKeyBoard = GetEnglishProductTypeTitle();
                var TypeKeyboard = GenerateKeyBoard(SelectedTypeKeyBoard, 3, true, 1);
                Bot.SendTextMessageAsync(item.Message.Chat.Id, Settings.First(x => x.Key == "SelectCollectionType").Value, ParseMode.Html, false, false, 0, TypeKeyboard);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "News").ValueFa)
            {
                foreach (var itemNews in DbContext.GetActiveNews())
                {
                    if (string.IsNullOrEmpty(itemNews.FileName))
                    {
                        Bot.SendTextMessageAsync(Chat.Id, itemNews.TextFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                    }
                    else // image news with caption
                    {
                        if (string.IsNullOrEmpty(itemNews.FileId))
                        {
                            string filePath = RootPath + "News/" + itemNews.FileName;
                            Stream stream = System.IO.File.OpenRead(filePath);
                            FileToSend file = new FileToSend(System.IO.Path.GetFileName(filePath), stream);
                            var msg = Bot.SendPhotoAsync(item.Message.Chat.Id, file, itemNews.TextFa, false, 0, PersianMainKeyBoard);
                            string FileId = msg.Result.Photo[0].FileId;
                            DbContext.UpdateNewsFileId(itemNews.Id, FileId);
                        }
                        else
                        {
                            FileToSend file = new FileToSend(itemNews.FileId);
                            Bot.SendPhotoAsync(item.Message.Chat.Id, file, itemNews.TextFa, false, 0, PersianMainKeyBoard);
                        }
                    }
                }
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage == Settings.First(x => x.Key == "News").Value)
            {
                foreach (var itemNews in DbContext.GetActiveNews())
                {
                    if (string.IsNullOrEmpty(itemNews.FileName))
                    {
                        Bot.SendTextMessageAsync(Chat.Id, itemNews.Text, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);
                    }
                    else // image news with caption
                    {
                        if (string.IsNullOrEmpty(itemNews.FileId))
                        {
                            string filePath = RootPath + "News/" + itemNews.FileName;
                            Stream stream = System.IO.File.OpenRead(filePath);
                            FileToSend file = new FileToSend(System.IO.Path.GetFileName(filePath), stream);
                            var msg = Bot.SendPhotoAsync(item.Message.Chat.Id, file, itemNews.Text, false, 0, EnglishMainKeyBoard);
                            string FileId = msg.Result.Photo[0].FileId;
                            DbContext.UpdateNewsFileId(itemNews.Id, FileId);
                        }
                        else
                        {
                            FileToSend file = new FileToSend(itemNews.FileId);
                            Bot.SendPhotoAsync(item.Message.Chat.Id, file, itemNews.Text, false, 0, EnglishMainKeyBoard);
                        }
                    }
                }
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else if (TextMessage.StartsWith(Settings.First(x => x.Key == "More").ValueFa) || TextMessage.StartsWith(Settings.First(x => x.Key == "More").Value))
            {
                var data = TextMessage.Split('-');
                var collectionType = data[1].Trim();
                var from = int.Parse(data[2].Trim());
                var to = int.Parse(data[3].Trim());

                bool hasMore = false;
                var SelectedData = DbContext.GetData((ProductType)int.Parse(collectionType), from, out hasMore);
                SendFiles(user, SelectedData, collectionType, from, hasMore);
                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
            }
            else
            {
                var BranchList = DbContext.GetBranches(TextMessage);
                if (BranchList.Count > 0)
                {
                    foreach (var itemBrach in BranchList)
                    {
                        if (user.Language == 0)
                            Bot.SendVenueAsync(item.Message.Chat.Id, float.Parse(itemBrach.Latitude), float.Parse(itemBrach.Longitude), itemBrach.Name, itemBrach.Address.Trim());
                        else
                            Bot.SendVenueAsync(item.Message.Chat.Id, float.Parse(itemBrach.Latitude), float.Parse(itemBrach.Longitude), itemBrach.EnglishName, itemBrach.EnglishAddress.Trim());
                    }
                    DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
                    return;
                }

                var codeItem = DbContext.FindCode(TextMessage);
                if (codeItem != null)
                {
                    SendFiles(user, codeItem);
                    DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage.ToUpper(), item.Message.Date, false);
                    return;
                }

                ProductType? productType = GetProductType(TextMessage);
                if(productType != null)
                {
                    var SelectedCollection = DbContext.GetCollectionType(productType.GetValueOrDefault());
                    if (SelectedCollection != null && SelectedCollection.Count > 0)
                    {
                        SendFiles(user, SelectedCollection, TextMessage, 0, true);
                        DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false);
                        return;
                    }
                }

                if (user.Language == 0)
                    Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "WelcomeMessage").ValueFa, ParseMode.Html, false, false, 0, PersianMainKeyBoard);
                else
                    Bot.SendTextMessageAsync(Chat.Id, Settings.First(x => x.Key == "WelcomeMessage").Value, ParseMode.Html, false, false, 0, EnglishMainKeyBoard);

                DbContext.AddMessage(Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, true);
            }
        }

        private void ResponseCallback(Update item)
        {
            var QueryData = item.CallbackQuery.Data;
            var userItem = DbContext.GetUserData(item.CallbackQuery.Message.Chat.Id);
            var data = QueryData.Split(':');

            if (data[0].ToLower() == "order")
            {
                int itemId = int.Parse(data[1]);
                Product selectedItem = DbContext.LoadDataItem(itemId);

                DbContext.AddOrder(userItem, selectedItem);

                if (userItem.Language == 0)
                {
                    string orderText = Settings.First(x => x.Key == "OrderText").ValueFa.Replace("{Code}", selectedItem.Code);
                    var orderKeyboard = new ReplyKeyboardMarkup();
                    KeyboardButton KeyboardButtonSendPhoneNumber = new KeyboardButton(Settings.First(x => x.Key == "SendPhoneNumber").ValueFa);
                    KeyboardButtonSendPhoneNumber.RequestContact = true;
                    orderKeyboard.Keyboard = new KeyboardButton[][] {
                        new KeyboardButton[] { KeyboardButtonSendPhoneNumber },
                        new KeyboardButton[] { Settings.First(x => x.Key == "Cancel").ValueFa }
                    };
                    orderKeyboard.ResizeKeyboard = true;

                    Bot.SendTextMessageAsync(item.CallbackQuery.Message.Chat.Id, orderText, ParseMode.Html, false, false, 0, orderKeyboard);
                }
                else
                {
                    string orderText = Settings.First(x => x.Key == "OrderText").Value.Replace("{Code}", selectedItem.Code);
                    var orderKeyboard = new ReplyKeyboardMarkup();
                    KeyboardButton KeyboardButtonSendPhoneNumber = new KeyboardButton(Settings.First(x => x.Key == "SendPhoneNumber").Value);
                    KeyboardButtonSendPhoneNumber.RequestContact = true;
                    orderKeyboard.Keyboard = new KeyboardButton[][] {
                        new KeyboardButton[] { KeyboardButtonSendPhoneNumber },
                        new KeyboardButton[] { Settings.First(x => x.Key == "Cancel").Value }
                    };
                    orderKeyboard.ResizeKeyboard = true;
                    Bot.SendTextMessageAsync(item.CallbackQuery.Message.Chat.Id, orderText, ParseMode.Html, false, false, 0, orderKeyboard);
                }
                //DbContext.AddMessage(userItem.ChatId, -1, "Order " + selectedItem.Code, item.CallbackQuery.Message.Date, false);
                //Bot.SendTextMessageAsync(item.CallbackQuery.Message.Chat.Id, "مشتري گرامي\n" +
                //                                                            "به علت حجم بالاي سفارشات در روزهاي آخر سال خواهشمنديم جهت هماهنگي و ثبت سفارش خود با شماره 22771934, 22793793, 22768953, 22768962 تماس حاصل فرماييد.\n" +
                //                                                            "با تشكر");
            }
            else if (data[0].ToLower() == "more")
            {
                string collectionType = data[1];
                int from = int.Parse(data[2]);
                int itemId = int.Parse(data[3]);

                bool hasMore = false;
                var SelectedData = DbContext.GetData((ProductType)int.Parse(collectionType), from, out hasMore);
                SendFiles(userItem, SelectedData, collectionType, from, hasMore);
                DbContext.AddMessage(userItem.ChatId, -1, "More " + collectionType + " " + from, item.CallbackQuery.Message.Date, false);

            }
        }

        private void SendFiles(BotUserData user, Product item)
        {
            Task.Factory.StartNew(async () =>
            {
                InlineKeyboardMarkup CollectionKeyboard = new InlineKeyboardMarkup();

                string orderData = "Order:" + item.Id;
                if (user.Language == 0)
                {
                    CollectionKeyboard.InlineKeyboard = new InlineKeyboardCallbackButton[][] { new InlineKeyboardCallbackButton[]
                        { new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "Order").ValueFa, orderData) }
                    };
                }
                else
                {
                    CollectionKeyboard.InlineKeyboard = new InlineKeyboardCallbackButton[][] { new InlineKeyboardCallbackButton[]
                        { new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "Order").Value, orderData) }
                    };
                }

                double Price = (((item.Wage + int.Parse(Settings.First(x => x.Key == "GoldPrice").Value)) * item.Weight) + (item.LeatherPrice + item.StonePrice)).GetValueOrDefault();
                var TotalPrice = Roundup(Price + (Price * 0.07) + ((Price + (Price * 0.07)) / 100 * 9), 4);
                string Message = "";
                if (user.Language == 0)
                    Message = Settings.First(x => x.Key == "DataCaption").ValueFa.Replace("{Code}", item.Code).Replace("{Weight}", item.Weight.ToString()).Replace("{Price}", TotalPrice.ToString());
                else if (user.Language == 1)
                    Message = Settings.First(x => x.Key == "DataCaption").Value.Replace("{Code}", item.Code).Replace("{Weight}", item.Weight.ToString()).Replace("{Price}", TotalPrice.ToString());

                if (string.IsNullOrEmpty(item.ProductFileList.FirstOrDefault(x=> x.FileType == Model.FileType.Bot).FileId))
                {
                    string filePath = RootPath + "/Data/" + item.ProductFileList.FirstOrDefault(x => x.FileType == Model.FileType.Bot).FileName;
                    Stream stream = System.IO.File.OpenRead(filePath);
                    FileToSend file = new FileToSend(System.IO.Path.GetFileName(filePath), stream);
                    var msg = Bot.SendPhotoAsync(user.ChatId, file, Message, false, 0, CollectionKeyboard);
                    string FileId = msg.Result.Photo[0].FileId;
                    DbContext.UpdateDataFileId(item.Id, FileId);
                }
                else
                {
                    FileToSend file = new FileToSend(item.ProductFileList.FirstOrDefault(x => x.FileType == Model.FileType.Bot).FileId);
                    await Bot.SendPhotoAsync(user.ChatId, file, Message, false, 0, CollectionKeyboard);
                }
            });
        }

        private void SendFiles(BotUserData user, List<Product> SelectedData, string CollectionType, int offset, bool hasMore)
        {
            Task.Factory.StartNew(async () =>
            {
                //List<string> SelectedTypeKeyBoard = new List<string>();
                //if (user.Language == 0)
                //    SelectedTypeKeyBoard = DbConxt.GetAllType().Select(x => x.TitleFa).ToList();
                //else if (user.Language == 1)
                //    SelectedTypeKeyBoard = DbConxt.GetAllType().Select(x => x.Title).ToList();

                //ReplyKeyboardMarkup CollectionKeyboard = GenerateKeyBoard(SelectedTypeKeyBoard, 3, true, user.Language.GetValueOrDefault(), SelectedData.Count == 10 ? offset + 10 : 0, CollectionType);

                for (int i = 0; i < SelectedData.Count; i++)
                {
                    var item = SelectedData[i];
                    InlineKeyboardMarkup CollectionKeyboard = new InlineKeyboardMarkup();

                    string orderData = "Order:" + item.Id;
                    string moreData = "More:" + CollectionType + ":" + (offset + 15) + ":" + item.Id;
                    if (user.Language == 0)
                    {
                        if (hasMore && i + 1 == SelectedData.Count)
                            CollectionKeyboard.InlineKeyboard = new InlineKeyboardCallbackButton[][] {
                                new InlineKeyboardCallbackButton[] { new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "Order").ValueFa, orderData) },
                                new InlineKeyboardCallbackButton[] { new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "More").ValueFa, moreData) }
                            };
                        else
                            CollectionKeyboard.InlineKeyboard = new InlineKeyboardCallbackButton[][] { new InlineKeyboardCallbackButton[] {
                                new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "Order").ValueFa, orderData) }
                            };
                    }
                    else
                    {
                        if (hasMore && i + 1 == SelectedData.Count)
                            CollectionKeyboard.InlineKeyboard = new InlineKeyboardCallbackButton[][] {
                                new InlineKeyboardCallbackButton[] { new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "Order").Value, orderData) },
                                new InlineKeyboardCallbackButton[] { new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "More").Value, moreData) }
                            };
                        else
                            CollectionKeyboard.InlineKeyboard = new InlineKeyboardCallbackButton[][] { new InlineKeyboardCallbackButton[] {
                                new InlineKeyboardCallbackButton(Settings.First(x => x.Key == "Order").Value, orderData) }
                            };
                    }

                    double Price = (((item.Wage + int.Parse(Settings.First(x => x.Key == "GoldPrice").Value)) * item.Weight) + (item.LeatherPrice + item.StonePrice)).GetValueOrDefault();
                    var TotalPrice = Roundup(Price + (Price * 0.07) + ((Price + (Price * 0.07)) / 100 * 9), 4);
                    string Message = "";
                    if (user.Language == 0)
                        Message = Settings.First(x => x.Key == "DataCaption").ValueFa.Replace("{Code}", item.Code).Replace("{Weight}", item.Weight.ToString()).Replace("{Price}", TotalPrice.ToString());
                    else if (user.Language == 1)
                        Message = Settings.First(x => x.Key == "DataCaption").Value.Replace("{Code}", item.Code).Replace("{Weight}", item.Weight.ToString()).Replace("{Price}", TotalPrice.ToString());

                    if (string.IsNullOrEmpty(item.ProductFileList.FirstOrDefault(x => x.FileType == Model.FileType.Bot).FileId))
                    {
                        string filePath = RootPath + "/Data/" + item.ProductFileList.FirstOrDefault(x => x.FileType == Model.FileType.Bot).FileName;
                        Stream stream = System.IO.File.OpenRead(filePath);
                        FileToSend file = new FileToSend(System.IO.Path.GetFileName(filePath), stream);
                        var msg = Bot.SendPhotoAsync(user.ChatId, file, Message, false, 0, CollectionKeyboard);
                        string FileId = msg.Result.Photo[0].FileId;
                        DbContext.UpdateDataFileId(item.Id, FileId);
                    }
                    else
                    {
                        FileToSend file = new FileToSend(item.ProductFileList.FirstOrDefault(x => x.FileType == Model.FileType.Bot).FileId);
                        await Bot.SendPhotoAsync(user.ChatId, file, Message, false, 0, CollectionKeyboard);
                    }

                    //await Bot.SendTextMessageAsync(user.ChatId, Message, false, false, 0, CollectionKeyboard, ParseMode.Html);
                    await Task.Delay(200);
                }
            });
        }

        private void GetInstagramUpdates()
        {
            Task.Factory.StartNew(() =>
            {
                Settings = DbContext.GetSettings();

                if (!Directory.Exists(RootPath + "Instagram"))
                {
                    Directory.CreateDirectory(RootPath + "Instagram");
                }

                string[] urls = new string[]
                {
                "https://api.instagram.com/v1/users/self/media/recent/?access_token=193612177.67ef6ad.1af2d888ce354d3dbe489516e1ed4ab2",
                "https://api.instagram.com/v1/users/self/media/recent/?access_token=1939915172.67ef6ad.514c79531a4644d9a4eb333216b596fe",
                "https://api.instagram.com/v1/users/self/media/recent/?access_token=2313995180.67ef6ad.e9415842886f40c086fd60b6364574c2"
                };

                foreach (var item in urls)
                {
                    using (var webClient = new WebClient())
                    {
                        var data = webClient.DownloadString(item);
                        var json = JObject.Parse(data);
                        var mediaData = json["data"];
                        foreach (var token in mediaData)
                        {

                            var Id = token["id"].ToString();
                            var TypeTitle = token["type"].ToString();
                            int TypeIndex = -1;
                            string Url = null;
                            if (TypeTitle.ToLower().Equals("image"))
                            {
                                Url = token["images"]["standard_resolution"]["url"].ToString();
                                TypeIndex = 0;
                            }
                            else if (TypeTitle.ToLower().Equals("video"))
                            {
                                Url = token["videos"]["standard_resolution"]["url"].ToString();
                                TypeIndex = 1;
                            }
                            var Caption = "-";
                            try
                            {
                                Caption = token["caption"]["text"].ToString();
                            }
                            catch { }
                            DbContext.AddInstagramFile(Id, TypeIndex, Url, Caption);
                        }
                    }
                }
            });

        }

        public static double Roundup(double number, int digits)
        {
            int num = (int)number;
            if (num % 10000 > 0)
            {
                num = num - (num % 10000) + 10000;
            }
            return num;
        }

        private ReplyKeyboardMarkup GenerateKeyBoard(List<string> Titles, int ColumnSize, bool MenuButton, byte langId, int More = 0, string CollectionType = "")
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
                if (langId == 0)
                    keys[0] = new KeyboardButton[] { Settings.First(x => x.Key == "More").ValueFa + " - " + CollectionType + " - " + More + "-" + (More + 15) };
                else if (langId == 1)
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
                if (langId == 0)
                {
                    keys[RowSize - 1] = new KeyboardButton[] { Settings.First(x => x.Key == "Menu").ValueFa };
                }
                else if (langId == 1)
                {
                    keys[RowSize - 1] = new KeyboardButton[] { Settings.First(x => x.Key == "Menu").Value };
                }
            }

            Keyboard.Keyboard = keys;

            return Keyboard;
        }

        public void SaveException(int Code, Exception ex)
        {
            string filePath = @"C:\Files\KiaGalleryBot\KiaGalleryBotError.txt";

            if (!Directory.Exists(@"C:\Files\KiaGalleryBot\"))
            {
                Directory.CreateDirectory(@"C:\Files\KiaGalleryBot\");
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Code: " + Code + Environment.NewLine + "Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-" + Environment.NewLine);
            }
        }

        public void SaveExceptionData(string Code)
        {
            string filePath = @"C:\Files\KiaGalleryBot\FileCodeError.txt";
            if (!Directory.Exists(@"C:\Files\KiaGalleryBot\"))
            {
                Directory.CreateDirectory(@"C:\Files\KiaGalleryBot\");
            }
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Code: " + Code + Environment.NewLine);
            }
        }

        public string GetStatusText(int status, int language)
        {
            if (language == 0)
            {
                switch (status)
                {
                    case 0:
                        return "نامشخص";
                    case 1:
                        return "در انتظار تماس";
                    case 2:
                        return "رد تماس";
                    case 3:
                        return "در انتظار پیش پرداخت";
                    case 4:
                        return "در حال ساخت";
                    case 5:
                        return "در انتظار پرداخت";
                    case 6:
                        return "ارسال شده";
                    case 7:
                        return "لغو شده";
                    case 8:
                        return "ارجاع داده شده";
                    case 9:
                        return "در انتظار تایید مشتری";
                    default:
                        throw new Exception("not found in enums");
                }
            }
            else
            {
                switch (status)
                {
                    case 0:
                        return "None";
                    case 1:
                        return "Pending Call";
                    case 2:
                        return "Reject Call";
                    case 3:
                        return "Pending Prepayment";
                    case 4:
                        return "Under Construction";
                    case 5:
                        return "Pending Payment";
                    case 6:
                        return "Sent";
                    case 7:
                        return "Canceled";
                    case 8:
                        return "Referred To";
                    case 9:
                        return "Pending Customer";
                    default:
                        throw new Exception("not found in enums");
                }
            }

        }

        //private void BtnImport_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        OpenFileDialog dialog = new OpenFileDialog();
        //        bool? result = dialog.ShowDialog();
        //        if (result == true)
        //        {
        //            string filename = dialog.FileName;
        //            string ServerPath = Path.GetDirectoryName(filename) + "\\";
        //            var databaseConnection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1;\"");

        //            var dataAdapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", databaseConnection);
        //            var dataTable = new DataTable();
        //            dataAdapter.Fill(dataTable);

        //            var listData = (from DataRow item in dataTable.Rows
        //                            select new
        //                            {
        //                                Code = item[0].ToString(),
        //                                TypeTitle = item[1].ToString(),
        //                                Weight = item[3].ToString(),
        //                                CollectionTitle = item[4].ToString(),
        //                                LetherPrice = item[5].ToString(),
        //                                StonePrice = item[6].ToString(),
        //                                Wage = item[7].ToString()
        //                            }).ToList();

        //            using (KiaGalleryContext db = new KiaGalleryContext())
        //            {
        //                try
        //                {
        //                    foreach (var item in listData)
        //                    {
        //                        var Type = db.CollectionType.FirstOrDefault(x => x.Title == item.TypeTitle);
        //                        if (Type == null)
        //                        {
        //                            Type = new CollectionType()
        //                            {
        //                                Title = item.TypeTitle,
        //                                OrderNo = 1000
        //                            };
        //                            db.CollectionType.Add(Type);
        //                            db.SaveChanges();
        //                        }
        //                        var CollectionName = db.CollectionName.FirstOrDefault(x => x.Title == item.CollectionTitle);
        //                        if (CollectionName == null)
        //                        {
        //                            CollectionName = new CollectionName()
        //                            {
        //                                Title = item.CollectionTitle,
        //                                OrderNo = 1000
        //                            };
        //                            db.CollectionName.Add(CollectionName);
        //                            db.SaveChanges();
        //                        }

        //                        DirectoryInfo info = new DirectoryInfo(ServerPath);
        //                        var Files = info.GetFiles().Where(x => x.Name.StartsWith(item.Code)).ToList();

        //                        if (Files.Count > 0)
        //                        {
        //                            var SourceFile = Files[0];

        //                            var DataItem = new Data()
        //                            {
        //                                Code = item.Code,
        //                                Weight = float.Parse(item.Weight),
        //                                LetherPrice = int.Parse(item.LetherPrice),
        //                                StonePrice = int.Parse(item.StonePrice),
        //                                Wage = int.Parse(item.Wage),
        //                                FileName = SourceFile.Name,
        //                                TypeId = Type.Id,
        //                                CollectionId = CollectionName.Id,
        //                            };

        //                            string DataPath = "C:/Files/KiaGalleryBot/Data";
        //                            string SourceDataFileName = SourceFile.FullName;
        //                            string DataFileName = DataPath + "/" + SourceFile.Name;

        //                            if (!Directory.Exists(DataPath))
        //                            {
        //                                Directory.CreateDirectory(DataPath);
        //                            }
        //                            if (System.IO.File.Exists(DataFileName))
        //                            {
        //                                System.IO.File.Delete(DataFileName);
        //                            }
        //                            System.IO.File.Copy(SourceDataFileName, DataFileName);

        //                            db.Data.Add(DataItem);
        //                            db.SaveChanges();
        //                        }
        //                        else
        //                        {
        //                            SaveExceptionData(item.Code);
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    SaveException(10, ex);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveException(11, ex);
        //    }
        //}

        public List<string> GetProductTypeTitle() {
            List<string> ListProductType = new List<string> {
                "دستبند زنجير طلا",
                "گردنبند",
                "گوشواره",
                "انگشتر",
                "آویز ساعت",
                "سنجاق بچگانه",
                "ست",
                "پابند",
                "آویز",
                "النگو",
                "دستبند چرم",
                "دستبند ریلی",
                "دستبند سنگی"
            };
            return ListProductType;
        }

        public List<string> GetEnglishProductTypeTitle()
        {
            List<string> ListProductType = new List<string> {
                "Gold Bracelet",
                "Necklace",
                "Earring",
                "Ring",
                "Watch Pendent",
                "Brooch",
                "Set",
                "Anklet",
                "Pendant",
                "Bangle",
                "Leather Bracelet",
                "Rail Bracelet",
                "Stone Bracelet"
            };
            return ListProductType;
        }

        public static ProductType? GetProductType(string title)
        {
            switch (title)
            {
                case "دستبند زنجير طلا":
                    return ProductType.GoldBracelet;
                case "گردنبند":
                    return ProductType.Necklace;
                case "گوشواره":
                    return ProductType.Earring;
                case "انگشتر":
                    return ProductType.Ring;
                case "آویز ساعت":
                    return ProductType.WatchPendent;
                case "سنجاق بچگانه":
                    return ProductType.Brooch;
                case "ست":
                    return ProductType.Set;
                case "پابند":
                    return ProductType.Anklet;
                case "آویز":
                    return ProductType.Pendant;
                case "النگو":
                    return ProductType.Bangle;
                case "خرج کار":
                    return ProductType.OuterWerk;
                case "دستبند چرمی":
                    return ProductType.LeatherBracelet;
                case "دستبند ریلی":
                    return ProductType.RailBracelet;
                case "دستبند سنگی":
                    return ProductType.StoneBracelet;
                case "Gold Bracelet":
                    return ProductType.GoldBracelet;
                case "Necklace":
                    return ProductType.Necklace;
                case "Earring":
                    return ProductType.Earring;
                case "Ring":
                    return ProductType.Ring;
                case "Watch Pendent":
                    return ProductType.WatchPendent;
                case "Brooch":
                    return ProductType.Brooch;
                case "Set":
                    return ProductType.Set;
                case "Anklet":
                    return ProductType.Anklet;
                case "Pendant":
                    return ProductType.Pendant;
                case "Bangle":
                    return ProductType.Bangle;
                case "OuterWerk":
                    return ProductType.OuterWerk;
                case "Leather Bracelet":
                    return ProductType.LeatherBracelet;
                case "Rail Bracelet":
                    return ProductType.RailBracelet;
                case "Stone Bracelet":
                    return ProductType.StoneBracelet;
                default:
                    return null;
            }
        }
    }

    public class DbContext
    {
        public static int GetLastOffset()
        {
            int _Offset = 0;
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                _Offset = int.Parse(db.Settings.FirstOrDefault(x => x.Key == "LastOffset").Value);
            }
            return _Offset;
        }

        public static List<BotSettings> GetSettings()
        {
            List<BotSettings> _Settings = null;
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                _Settings = db.BotSettings.ToList();
            }
            return _Settings;
        }

        public static BotUserData AddUser(BotUserData user)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var userEntity = db.BotUserData.FirstOrDefault(x => x.ChatId == user.ChatId);
                if (userEntity == null)
                {
                    user.Language = 0; // Default language persian
                    user.CreatedDate = DateTime.Now.ToUniversalTime();
                    db.BotUserData.Add(user);
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
                return userEntity;
            }
        }

        public static BotUserData GetUserData(long ChatId)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                return db.BotUserData.First(x => x.ChatId == ChatId);
            }
        }

        public static void AddMessage(long chatId, int messageId, string text, DateTime date, bool unknown)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var _Message = new BotMessage()
                {
                    ChatId = chatId,
                    MessageId = messageId,
                    Text = text,
                    CreatedDate = date.ToUniversalTime(),
                    Unknown = unknown
                };
                db.BotMessage.Add(_Message);
                db.SaveChanges();
            }
        }

        public static List<TopData> GetTop10()
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var data = db.BotBroadcast.Take(10).OrderByDescending(x => x.CreatedDate).Select(x => new TopData()
                {
                    Type = string.IsNullOrEmpty(x.FileName) ? 0 : 1, //0: text, 1: image, 2: video
                    Text = x.Text,
                    FileId = x.FileId,
                    CreatedDate = x.CreatedDate
                }).ToList();

                data.AddRange(db.BotInstagram.Take(10).OrderByDescending(x => x.CreatedDate).Select(y => new TopData()
                {
                    Type = y.Type + 1,
                    Text = y.Caption,
                    FileId = y.FileId,
                    CreatedDate = y.CreatedDate
                }).ToList());

                return data.OrderByDescending(x => x.CreatedDate).Take(10).ToList();
            }
        }

        public static List<Model.Context.Location> GetCityList()
        {
            List<Model.Context.Location> _data = null;
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                _data = db.Location.Where(x=> x.LocationType == LocationType.City && x.BranchList.Count()>0).ToList();
            }
            return _data;
        }

        //public static List<CollectionName> GetAllCollection()
        //{
        //    using (KiaGalleryContext db = new KiaGalleryContext())
        //    {
        //        return db.CollectionName.Where(x => x.Data.Count > 0).OrderBy(x => x.OrderNo).ToList();
        //    }
        //}
        
        public static List<BotNews> GetActiveNews()
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                return db.BotNews.ToList();
            }
        }

        public static void UpdateNewsFileId(int id, string fileId)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var news = db.BotNews.FirstOrDefault(x => x.Id == id);
                news.FileId = fileId;
                db.SaveChanges();
            }
        }

        public static List<Branch> GetBranches(string BranchTitle)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                return db.Branch.Where(x => x.City.EnglishName == BranchTitle || x.City.Name == BranchTitle).ToList();
            }
        }

        public static Product FindCode(string Code)
        {
            if (string.IsNullOrEmpty(Code)) return null;

            using (var db = new KiaGalleryContext())
            {
                return db.Product.FirstOrDefault(x => x.Code.ToLower() == Code.ToLower());
            }
        }

        public static List<Product> GetAllData()
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                return db.Product.Include("CollectionName").Include("CollectionType").Include("Season").Distinct().ToList();
            }
        }

        public static List<Product> GetData(ProductType productType, int Offset, out bool hasMore)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var query = db.Product.Where(x => x.ProductType == productType);
                var count = query.Count();
                var data = query.OrderByDescending(x => x.Id).Skip(Offset).Take(15).ToList();
                if (Offset + 15 >= count)
                    hasMore = false;
                else
                    hasMore = true;

                return data;
            }
        }

        public static void UpdateDataFileId(int id, string fileId)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var data = db.Product.Include(x=> x.ProductFileList).FirstOrDefault(x => x.Id == id);
                data.ProductFileList.FirstOrDefault(x=> x.FileType == Model.FileType.Bot).FileId = fileId;
                db.SaveChanges();
            }
        }

        public static List<Broadcast> GetNotSentBroadcast()
        {
            List<Broadcast> data = null;
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                data = db.BotBroadcast.Where(x => x.Sended == false || x.Sended == null).ToList();
                data.ForEach(x => x.Sended = true);
                db.SaveChanges();
            }
            return data;
        }

        public static List<BotUserData> GetAllUsers()
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                return db.BotUserData.Where(x => x.ChatId > 0 && x.Stoped != true).ToList();
            }
        }

        public static void UpdateBroadCastFileId(Broadcast model, string fileId)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var entity = db.BotBroadcast.FirstOrDefault(x => x.Id == model.Id);
                entity.FileId = fileId;
                db.SaveChanges();
            }
        }

        public static void AddInstagramFile(string id, int typeIndex, string url, string caption)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                if (!(db.BotInstagram.Count(x => x.InstagramId == id) > 0))
                {
                    db.BotInstagram.Add(new Instagram()
                    {
                        InstagramId = id,
                        Type = typeIndex,
                        Url = url,
                        Caption = caption,
                        Sended = false,
                        CreatedDate = DateTime.Now
                    });

                    var startName = url.LastIndexOf("/", StringComparison.Ordinal) + 1;
                    var fileName = url.Substring(startName, url.IndexOf(".", startName, StringComparison.Ordinal) - startName + 4);
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(url, "C:/Files/KiaGalleryBot/Broadcast/" + fileName);
                    }

                    var item = new Broadcast()
                    {
                        BroadcastType = (BotType)(byte.Parse((typeIndex + 1).ToString())),
                        Text = caption,
                        TextFa = caption,
                        FileName = fileName,
                        FileId = null,
                        Sended = false,
                        CreatedDate = DateTime.Now
                    };
                    db.BotBroadcast.Add(item);
                    db.SaveChanges();
                }
            }
        }

        public static List<Instagram> GetInstagramNewPost()
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                return db.BotInstagram.Where(x => x.Sended == false).ToList();
            }
        }

        public static void UpdateInstagramFileId(int id, string fileId)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var item = db.BotInstagram.SingleOrDefault(x => x.Id == id);
                item.FileId = fileId;
                item.Sended = true;
                db.SaveChanges();
            }
        }

        public static void UpdateOffset(int offset)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var entity = db.BotSettings.SingleOrDefault(x => x.Key == "LastOffset");
                entity.Value = offset.ToString();
                db.SaveChanges();
            }
        }

        //public static Context.Message GetLastMessage(long id, string message = "")
        //{
        //    using (KiaGalleryContext db = new KiaGalleryContext())
        //    {
        //        return db.Message.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ChatId == id && !x.Text.Contains(message));
        //    }
        //}

        public static List<Product> GetCollectionType(ProductType productType)
        {
            List<Product> data;
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                data = db.Product.Where(x => x.ProductType == productType).OrderByDescending(x => x.Id).Take(15).ToList();
            }

            if (data != null && data.Count > 0)
                return data;
            else
                return null;
        }

        public static void ChangeLanguage(BotUserData user, byte languageId)
        {
            try
            {
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var userEntity = db.BotUserData.First(x => x.ChatId == user.ChatId);
                    userEntity.Language = languageId;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public static Product LoadDataItem(int itemId)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                return db.Product.First(x => x.Id == itemId);
            }
        }

        public static void AddOrder(BotUserData userItem, Product product)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var allNoneOrder = db.BotOrder.Where(x => x.ChatId == userItem.ChatId && x.Status == 0).ToList();
                allNoneOrder.ForEach(x => x.Status = BotOrderStatus.Canceled);

                int lastNumber = 0;

                var lastItem = db.BotOrder.OrderByDescending(x => x.Id).FirstOrDefault();
                if (lastItem != null)
                {
                    Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                    Match result = re.Match(lastItem.OrderSerial);
                    lastNumber = int.Parse(result.Groups[2].Value);
                }
                lastNumber++;

                BotOrder order = new BotOrder();
                order.UserId = userItem.UserId.GetValueOrDefault();
                order.ChatId = userItem.ChatId;
                order.ProductId = product.Id;
                order.Status = 0;
                order.OrderSerial = "KIA" + lastNumber.ToString("D5");
                order.CreatedDate = DateTime.Now;

                db.BotOrder.Add(order);
                db.SaveChanges();
            }
        }
        public static void AddDaily(BotUserData userItem, Product product, int dailyId)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var allNoneOrder = db.BotOrder.Where(x => x.ChatId == userItem.ChatId && x.Status == 0).ToList();
                allNoneOrder.ForEach(x => x.Status = BotOrderStatus.Canceled);

                int lastNumber = 0;

                var lastItem = db.BotOrder.OrderByDescending(x => x.Id).FirstOrDefault();
                if (lastItem != null)
                {
                    Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                    Match result = re.Match(lastItem.OrderSerial);
                    lastNumber = int.Parse(result.Groups[2].Value);
                }
                lastNumber++;

                BotOrder order = new BotOrder();
                order.UserId = userItem.UserId.GetValueOrDefault();
                order.ChatId = userItem.ChatId;
                order.ProductId = product.Id;
                order.Status = 0;
                order.OrderSerial = "KIA" + lastNumber.ToString("D4");
                order.CreatedDate = DateTime.Now;
                db.BotOrder.Add(order);
                db.SaveChanges();
            }
        }

        public static bool GetPhoneNumber(Contact contact)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var orderItem = db.BotOrder.OrderByDescending(x => x.Id).FirstOrDefault(x => x.UserId == contact.UserId && x.Status == 0);
                
                orderItem.Status = BotOrderStatus.PendingCall;
                orderItem.FirstName = contact.FirstName;
                orderItem.LastName = contact.LastName;
                orderItem.PhoneNumber = contact.PhoneNumber;

                db.SaveChanges();
            }
            return true;
        }

        public static void CancelOrder(BotUserData user)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var allNoneOrder = db.BotOrder.Where(x => x.ChatId == user.ChatId && x.Status == 0).ToList();
                allNoneOrder.ForEach(x => x.Status = BotOrderStatus.Canceled);
                db.SaveChanges();
            }
        }

        //public static List<BotOrder> GetAllOrder(BotUserData user)
        //{
        //    using (KiaGalleryContext db = new KiaGalleryContext())
        //    {
        //        return db.BotOrder.Include(x=> x.Product).Where(x => x.ChatId == user.ChatId && (x.Status == BotOrderStatus.PendingCall || x.Status == BotOrderStatus.RejectCall || x.Status == BotOrderStatus.PendingPrepayment || x.Status == BotOrderStatus.UnderConstruction || x.Status == BotOrderStatus.PendingPayment)).ToList();
        //    }
        //}

        //public static bool AvailableDailyOffer(int dailyId)
        //{
        //    using (KiaGalleryContext db = new KiaGalleryContext())
        //    {
        //        var dailyOffer = db.BotBroadcast.Single(x => x.Id == dailyId);
        //        var orderCount = db.BotOrder.Count(x => x.DailyOfferId == dailyId && x.Status != 0 && x.Status != BotOrderStatus.Canceled);
        //        if (orderCount >= dailyOffer.Count)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //}

        //public static bool AvailableInCart(UserData userItem, int dailyId)
        //{
        //    using (KiaGalleryContext db = new KiaGalleryContext())
        //    {
        //        return db.BotOrder.Count(x => x.UserId == userItem.UserId && x.DailyOfferId == dailyId && x.Status != BotOrderStatus.Canceled) == 0;
        //    }
        //}
    }

    public class TopData
    {
        public int Type { get; set; }
        public string Text { get; set; }
        public string FileId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
