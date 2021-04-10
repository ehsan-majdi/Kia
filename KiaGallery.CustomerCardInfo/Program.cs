using KiaGallery.Common;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KiaGallery.CustomerCardInfo
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (var db = new KiaGalleryMainEntities())
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Program Started");
                var model = new UserInfoRequestViewModel();
                model.adminusername = "1091147";
                model.adminpassword = Sha1Hash("880866252");
                Console.WriteLine("Sending Parameters");
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/userinfo");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                request.Timeout = 999999999;
                Console.WriteLine("Waiting For Response");
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<UserInfoResponseViewModel>(responsed.Content);
                var counter = 0;
                if (data.lstUser != null && data.lstUser.Count > 0)
                {
                    foreach (var item in data.lstUser)
                    {
                        var entity = db.UserInfoes.Where(x => x.CarriorCode == item.cariCode).SingleOrDefault();
                        Console.WriteLine(string.Format("{0} {1}", counter++, " check " + entity.CarriorCode));
                        if (entity != null)
                        {
                            if (entity.FullName != "بي نام")
                            {

                                entity.FullName = item.fullname;
                                entity.BirthDate = DateUtility.GetDateTime(item.birthDate);
                                entity.NationalCode = item.ntCode;
                                entity.Email = item.email;
                                entity.Mobile = item.mobile;
                                entity.Address = item.homeAddr;
                                entity.CreateUserId = 1;
                                entity.ModifyUserId = 1;
                                entity.CreateDate = DateTime.Now;
                                entity.ModifyDate = DateTime.Now;
                                entity.DiscountPercent = item.discountPercent;
                                entity.PointPercent = item.pointPercent;
                                Console.WriteLine(string.Format("{0} {1}", counter++, " edit  " + entity.CarriorCode));
                            }
                        }
                        else
                        {

                            var UserInfoe = new UserInfoes()
                            {
                                FullName = item.fullname,
                                CarriorCode = item.cariCode,
                                Active = true,
                                CardNumber = item.cardNo,
                                BirthDate = DateUtility.GetDateTime(item.birthDate),
                                NationalCode = item.ntCode,
                                Email = item.email,
                                Mobile = item.mobile,
                                Address = item.homeAddr,
                                CreateUserId = 1,
                                ModifyUserId = 1,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                DiscountPercent = item.discountPercent,
                                PointPercent = item.pointPercent,
                            };
                            db.UserInfoes.Add(UserInfoe);
                            Console.WriteLine(string.Format("{0} {1}", counter++, " new "));
                        }
                    }
                }
                Console.WriteLine("saving in database");
                db.SaveChanges();
                Console.WriteLine("save success");
            }
        }
        public static string Sha1Hash(string password)
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hashedPassword;
            }
        }
        public static CardReportResponseViewModel ChangeCardReport(string cariorcard)
        {
            var model = new
            {
                adminusername = "1091147",
                adminpassword = Sha1Hash("880866252"),
                dcCode = "0",
                merchantName = "0",
                mobile = cariorcard,
                pageSize = "99999999",
            };

            var param = JsonConvert.SerializeObject(model);
            var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/accountreport");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/json", param, ParameterType.RequestBody);
            IRestResponse responsed = client.Execute(request);
            var data = JsonConvert.DeserializeObject<CardReportResponseViewModel>(responsed.Content);
            if (data.lstTransaction != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

    }

}
public class CardReportResponseViewModel
{
    public double pointPercent { get; set; }
    public List<CardReportListResponseViewModel> lstTransaction { get; set; }
}
public class CardReportListResponseViewModel
{
    public double debitAmount { get; set; }
    public double creditAmount { get; set; }
    public string jdate { get; set; }
}
public class UserInfoRequestViewModel
{
    public string adminusername { get; set; }
    public string adminpassword { get; set; }
    public string cariorcard { get; set; }

}
public class UserInfoResponseViewModel
{
    public List<UserInfoResponseListViewModel> lstUser { get; set; }
}
public class UserInfoResponseListViewModel
{
    public string cariCode { get; set; }
    public string cardNo { get; set; }
    public string ntCode { get; set; }
    public string birthDate { get; set; }
    public string fullname { get; set; }
    public double discountPercent { get; set; }
    public double pointPercent { get; set; }
    public string email { get; set; }
    public string mobile { get; set; }
    public string homeAddr { get; set; }
}
