using KiaGallery.Model;

namespace KiaGallery.Web.Models
{
    public class BranchViewModel
    {
        public int? id { get; set; }
        public int? provinceId { get; set; }
        public int cityId { get; set; }
        public int order { get; set; }
        public string alias { get; set; }
        public int workingHour { get; set; }
        public string ownerName { get; set; }
        public string ownerFatherName { get; set; }
        public string ownerNationalityCode { get; set; }
        public string ownerNationalityNo { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
        public string phone { get; set; }
        public string mobileNumber { get; set; }
        public string address { get; set; }
        public string englishAddress { get; set; }
        public string color { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public bool active { get; set; }
        public BranchType branchType { get; set; }
        public int orderCount { get; set; }
        /// <summary>
        /// بدهی حساب طلا
        /// </summary>
        //public float goldDebt { get; set; }
        ///// <summary>
        ///// بدهی حساب ریال
        ///// </summary>
        //public long rialDebt { get; set; }
        ///// <summary>
        ///// اعتبار حساب طلا
        ///// </summary>
        //public float goldCredit { get; set; }
    }

    public class BranchSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
    }

    public class BranchFinancial
    {
        public int id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// بدهی حساب طلا
        /// </summary>
        public float goldDebt { get; set; }
        /// <summary>
        /// بدهی حساب ریال
        /// </summary>
        public long rialDebt { get; set; }
        /// <summary>
        /// اعتبار حساب طلا
        /// </summary>
        public float goldCredit { get; set; }
    }

    public class BranchModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }

   public class BranchApiViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? provinceId { get; set; }
        public int cityId { get; set; }
        public int order { get; set; }
        public string alias { get; set; }
        public int workingHour { get; set; }
        public string englishName { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string englishAddress { get; set; }
        public string color { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public bool active { get; set; }
        public BranchType branchType { get; set; }
        public string branchTypeTitle { get; set; }
    }
}