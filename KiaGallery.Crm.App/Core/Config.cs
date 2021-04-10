using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Crm.App.Core
{
    public class Config
    {
        public const string BaseUrl = "http://localhost:49762/api/";

        public const string LoginUrl = BaseUrl + "user/applogin";
        public const string GetCustomerUrl = BaseUrl + "customer/getCustomer";
        public const string SaveCustomerUrl = BaseUrl + "customer/save";
        public const string GetSettingsUrl = BaseUrl + "invoice/getSettings";
        public const string GetBarcodeUrl = BaseUrl + "invoice/getBarcode";
        public const string SaveInvoiceUrl = BaseUrl + "invoice/saveInvoice";
    }
}
