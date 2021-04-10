using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.DailyReport.Common.Model
{
    public sealed class User
    {
        public string firsName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string branch { get; set; }
        public object fileName { get; set; }
        public string token { get; set; }
    }
}
