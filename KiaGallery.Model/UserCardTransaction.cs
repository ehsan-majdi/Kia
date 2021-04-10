using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model
{
    public class UserCardTransaction
    {
        public int Id { get; set; }
        public int UserInfoId { get; set; }
        public string TrnDate { get; set; }
        public string totalTrnCount { get; set; }
        public double PointPercent { get; set; }
        public double discountPercent { get; set; }
        public string IsLoyal { get; set; }
        public string LocalBalance { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public virtual UserInfo UserInfo { get; set; }
    }
}
