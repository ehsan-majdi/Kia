using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Crm.App.Model
{
    public class SettingsViewModel
    {
        public long goldPrice { get; set; }
        public List<DiscountSettingViewModel> discountList { get; set; }
    }
}
