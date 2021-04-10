using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class SmsSearchViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        public int phoneNumber { get; set; }
        /// <summary>
        /// شماره کارت بانکی
        /// </summary>
        public int? creditCard { get; set; }

    }
    public class SendSmsViewModel
    {
        public string phoneNumber { get; set; }
        public string firstName { get; set; }
        public string bank { get; set; }
        public string text { get; set; }
        public string iban { get; set; }
        public string accountNumber { get; set; }
        public string cardNumber { get; set; }

    }
}