using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class LoyalityCardViewModel
    {
        public int id { get; set; }
        public int? branchId { get; set; }
        public int count { get; set; }
        public string code { get; set; }
        public string branchName { get; set; }
        public string customerName { get; set; }
        public List<int> idList { get; set; }
        public LoyalityCardStatus cardStatus { get; set; }
        public string cardStatusTitle { get; set; }
        public LoyalityCardType cardType { get; set; }
        public string cardTypeTitle { get; set; }
        public DateTime createDate { get; set; }
        public string persianCreateDate { get; set; }
        public string createUser { get; set; }
    }
    public class LoyalityCardSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public LoyalityCardStatus? status { get; set; }
        public LoyalityCardType? type { get; set; }
    }

    public class LoyalityCardReserveViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public string birthDate { get; set; }
        /// <summary>
        /// تاریخ ازدواج
        /// </summary>
        public string mariageDate { get; set; }
        /// <summary>
        /// تاریخ 
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// آدرس محل
        /// </summary>
        public string addressLocation { get; set; }
        public string CustomerLocationId { get; set; }
        public string persianBirthDate { get; set; }
        public string persianMariageDate { get; set; }



    }
}