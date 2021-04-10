using KiaGallery.Common;
using KiaGallery.Web.SmsHandler;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SmsSchedule
{
    public class Program
    {
        static void Main(string[] args)
        {
            var nowHour = DateTime.Now.Hour;
            var excellReminder1 = 10;
            var excellReminder2 = 16;
            var postSmsReminder = 21;
            var numbers = new List<string>() { "09354047788", "09124254257", "09122196672", "09128639319", "09122010861", "09126046937", "09124598925", "09126904936", "09361604970" };
            //if (nowHour == postSmsReminder)
            //{
            //    Console.WriteLine("Time Is Correct");
            //    NikSmsWebServiceClient.SendSmsNik("لطفا کدهای رهگیری مرسولات پستی گالری کیا را به شماره 09903160575 ارسال بفرمایید. باتشکر", "09122400802");
            //}
            //if (nowHour == excellReminder1)
            //{
            //    Console.WriteLine("Time Is Correct");
            //    NikSmsWebServiceClient.SendSmsNik("همکار گرامی لطفا فایل اکسل موجودی شعبه را در پورتال وارد نماید.", numbers);
            //}

            //if (nowHour == excellReminder2)
            //{
            //    Console.WriteLine("Time Is Correct");
            //    NikSmsWebServiceClient.SendSmsNik("همکار گرامی لطفا فایل اکسل موجودی شعبه را در پورتال وارد نماید.", numbers);
            //}

            using (var db = new KiaGalleryMainEntities())
            {
                var todayDate = DateTime.Now.Date;
                var yesterday = DateTime.Now.AddDays(-1);
                var DayOfWeek = DateTime.Today.DayOfWeek;
                var DayOfMonth = DateUtility.GetPersianDay(DateTime.Now);
                var todayTime = DateTime.Now.TimeOfDay;
                var currentHour = DateTime.Now.Hour;
                var timeOfDayMinutes = (int)todayTime.TotalMinutes;
                var ticks = DateTime.Now.Ticks;
                var list = db.Sms.Where(x => x.TimeTotalMinutes == timeOfDayMinutes && x.CreateDate > yesterday).Select(x => new
                {
                    id = x.Id,
                    time = x.Time,
                    sendingDate = x.SendingDate,
                    timeTotalMinutes = x.TimeTotalMinutes,
                    text = x.Text,
                    destinationNumber = x.DestinationNumber,
                    userId = x.UserId,
                    branchId = x.BranchId,
                    sent = x.Sent,
                    sendingTimeMethod = x.SendingTimeMethod,
                    dayOfWeek = x.DayOfWeek,
                    dayOfMonth = x.DayOfMonth,

                }).ToList();

                if (list.Count > 0 && list != null)
                {

                    foreach (var item in list)
                    {
                        var entity = db.Sms.Where(x => x.Id == item.id).Single();
                        if (item.sendingTimeMethod == 0)
                        {

                            if (!string.IsNullOrEmpty(item.branchId))
                            {
                                var branchId = item.branchId.Split('-');
                                List<int> branchIdList = branchId.Select(int.Parse).ToList();
                                var personelNumber = db.UserProfile.Where(y => branchIdList.Contains(y.BranchId.Value) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                                NikSmsWebServiceClient.SendSmsNik(item.text, personelNumber);
                                entity.Sent = true;
                                db.SaveChanges();
                            }
                            if (!string.IsNullOrEmpty(item.userId))
                            {
                                var userId = item.userId.Split('-');
                                List<int> userIdList = userId.Select(int.Parse).ToList();
                                var personelNumber = db.UserProfile.Where(y => userIdList.Contains(y.Id) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                                NikSmsWebServiceClient.SendSmsNik(item.text, personelNumber);
                                entity.Sent = true;
                                db.SaveChanges();
                            }
                            if (item.destinationNumber != null && item.destinationNumber != "" && !item.destinationNumber.Contains('-'))
                            {
                                NikSmsWebServiceClient.SendSmsNik(item.text, item.destinationNumber);
                                entity.Sent = true;
                                db.SaveChanges();
                            }
                            if (item.destinationNumber != null && item.destinationNumber != "" && item.destinationNumber.Contains('-'))
                            {
                                Console.WriteLine(string.Format("Sending DaGgte is {0} SendingTime Is {1}", item.sendingDate.Date, item.time));
                                var numberList = item.destinationNumber.Split('-');
                                var destinationNumbers = new List<string>();
                                destinationNumbers.AddRange(numberList);
                                NikSmsWebServiceClient.SendSmsNik(item.text, destinationNumbers);
                                Console.WriteLine("Message Sent");
                                entity.Sent = true;
                                db.SaveChanges();
                            }
                        }
                        if (item.sendingTimeMethod == 1 && DayOfWeek.ToString() == item.dayOfWeek)
                        {

                            if (!string.IsNullOrEmpty(item.branchId))
                            {
                                var branchId = item.branchId.Split('-');
                                List<int> branchIdList = branchId.Select(int.Parse).ToList();
                                var personelNumber = db.UserProfile.Where(y => branchIdList.Contains(y.BranchId.Value) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                                NikSmsWebServiceClient.SendSmsNik(item.text, personelNumber);
                               
                            }
                            if (!string.IsNullOrEmpty(item.userId))
                            {
                                var userId = item.userId.Split('-');
                                List<int> userIdList = userId.Select(int.Parse).ToList();
                                var personelNumber = db.UserProfile.Where(y => userIdList.Contains(y.Id) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                                NikSmsWebServiceClient.SendSmsNik(item.text, personelNumber);
                               
                            }
                            if (item.destinationNumber != null && item.destinationNumber != "" && !item.destinationNumber.Contains('-'))
                            {
                                NikSmsWebServiceClient.SendSmsNik(item.text, item.destinationNumber);
                               
                            }
                            if (item.destinationNumber != null && item.destinationNumber != "" && item.destinationNumber.Contains('-'))
                            {
                                Console.WriteLine(string.Format("Sending DaGgte is {0} SendingTime Is {1}", item.sendingDate.Date, item.time));
                                var numberList = item.destinationNumber.Split('-');
                                var destinationNumbers = new List<string>();
                                destinationNumbers.AddRange(numberList);
                                NikSmsWebServiceClient.SendSmsNik(item.text, destinationNumbers);
                                Console.WriteLine("Message Sent");
                              
                            }
                            entity.Sent = true;
                            db.SaveChanges();
                        }

                        if (item.sendingTimeMethod == 2 && item.dayOfMonth == DayOfMonth)
                        {
                            var newSms = new Sms()
                            {
                                Text = item.text,
                                DayOfMonth = item.dayOfMonth,
                                DayOfWeek = item.dayOfWeek,
                                SendingTimeMethod = item.sendingTimeMethod,
                                Sent = false,
                                DestinationNumber = item.destinationNumber,
                                UserId = item.userId,
                                BranchId = item.branchId,
                                SendingDate = DateTime.Now,
                                TimeTotalMinutes = (int)TimeSpan.Parse(item.time).TotalMinutes,
                                Time = item.time,
                                CreateUserId = 1,
                                ModifyUserId = 1,
                            };
                            if (!string.IsNullOrEmpty(item.branchId))
                            {
                                var branchId = item.branchId.Split('-');
                                List<int> branchIdList = branchId.Select(int.Parse).ToList();
                                var personelNumber = db.UserProfile.Where(y => branchIdList.Contains(y.BranchId.Value) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                                NikSmsWebServiceClient.SendSmsNik(item.text, personelNumber);

                            }
                            if (!string.IsNullOrEmpty(item.userId))
                            {
                                var userId = item.userId.Split('-');
                                List<int> userIdList = userId.Select(int.Parse).ToList();
                                var personelNumber = db.UserProfile.Where(y => userIdList.Contains(y.Id) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                                NikSmsWebServiceClient.SendSmsNik(item.text, personelNumber);

                            }
                            if (item.destinationNumber != null && item.destinationNumber != "" && !item.destinationNumber.Contains('-'))
                            {
                                NikSmsWebServiceClient.SendSmsNik(item.text, item.destinationNumber);

                            }
                            if (item.destinationNumber != null && item.destinationNumber != "" && item.destinationNumber.Contains('-'))
                            {
                                Console.WriteLine(string.Format("Sending DaGgte is {0} SendingTime Is {1}", item.sendingDate.Date, item.time));
                                var numberList = item.destinationNumber.Split('-');
                                var destinationNumbers = new List<string>();
                                destinationNumbers.AddRange(numberList);
                                NikSmsWebServiceClient.SendSmsNik(item.text, destinationNumbers);
                                Console.WriteLine("Message Sent");

                            }
                            entity.Sent = true;
                            db.Sms.Add(newSms);
                            db.SaveChanges();

                        }
                        if (item.sendingTimeMethod == 3 && todayDate == item.sendingDate)
                        {

                            if (!string.IsNullOrEmpty(item.branchId))
                            {
                                var branchId = item.branchId.Split('-');
                                List<int> branchIdList = branchId.Select(int.Parse).ToList();
                                var personelNumber = db.UserProfile.Where(y => branchIdList.Contains(y.BranchId.Value) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                                NikSmsWebServiceClient.SendSmsNik(item.text, personelNumber);

                            }
                            if (!string.IsNullOrEmpty(item.userId))
                            {
                                var userId = item.userId.Split('-');
                                List<int> userIdList = userId.Select(int.Parse).ToList();
                                var personelNumber = db.UserProfile.Where(y => userIdList.Contains(y.Id) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                                NikSmsWebServiceClient.SendSmsNik(item.text, personelNumber);

                            }
                            if (item.destinationNumber != null && item.destinationNumber != "" && !item.destinationNumber.Contains('-'))
                            {
                                NikSmsWebServiceClient.SendSmsNik(item.text, item.destinationNumber);

                            }
                            if (item.destinationNumber != null && item.destinationNumber != "" && item.destinationNumber.Contains('-'))
                            {
                                Console.WriteLine(string.Format("Sending DaGgte is {0} SendingTime Is {1}", item.sendingDate.Date, item.time));
                                var numberList = item.destinationNumber.Split('-');
                                var destinationNumbers = new List<string>();
                                destinationNumbers.AddRange(numberList);
                                NikSmsWebServiceClient.SendSmsNik(item.text, destinationNumbers);
                                Console.WriteLine("Message Sent");

                            }
                            entity.Sent = true;
                            db.SaveChanges();
                        }

                    }
                    Console.WriteLine("Task Complete");
                }
                else
                {
                    Console.WriteLine("No Data");
                }


               

            }
        }
    }
}
