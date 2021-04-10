using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.DailyReport.Common
{
    public class Services
    {
        public static KiaGallery.Common.Response CallService(ServiceType serviceType, NameValueCollection Params)
        {
            string Prefix = "";
            switch (serviceType)
            {
                case ServiceType.Login:
                    Prefix = "account";
                    break;
                case ServiceType.GetBaseData:
                    Prefix = "Home";
                    break;
                case ServiceType.Load:
                case ServiceType.Save:
                case ServiceType.SaveDraft:
                    Prefix = "DailyReportt";
                    break;
                default:
                    break;
            }

            WebClient req = new WebClient();
            using (var wb = new WebClient())
            {
                string URL = "http:" + $"//185.129.169.121:2007//api/dailyReportFinancial/{Prefix}/{serviceType.ToString()}";

                try
                {
                    byte[] responseByte = wb.UploadValues(URL, "POST", Params);
                    string ResString = System.Text.UTF8Encoding.UTF8.GetString(responseByte);
                    KiaGallery.Common.Response Response = Newtonsoft.Json.JsonConvert.DeserializeObject<KiaGallery.Common.Response>(ResString);

                    return Response;
                }
                catch (WebException wex)
                {
                    // "Unable to connect to the remote server"
                    return null;
                }
                catch (Exception ex)
                {
                    return null;

                }
            }
        }

        public enum ServiceType
        {
            Login,
            GetBaseData,
            Load,
            Save,
            SaveDraft
        }
    }
}
