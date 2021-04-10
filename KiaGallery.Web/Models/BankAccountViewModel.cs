using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class BankAccountViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// عنوان حساب
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public string bank { get; set; }
        /// <summary>
        /// شماره کارت
        /// </summary>
        public double cardNumber { get; set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public string accountNumber { get; set; }
        /// <summary>
        /// شبا
        /// </summary>
        public string iban { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// تلفن ثابت
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// ارگان
        /// </summary>
        public bool organ { get; set; }
        /// <summary>
        /// شرح در مورد نوع حساب
        /// </summary>
        public string explanation { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// آی پی
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? createDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime? modifyDate { get; set; }
        public List<BankAccountDetailViewModel> bankAccountDetailViewModelList { get; set; }



    }
    public class BankAccountDetailViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// عنوان حساب
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public string bankList { get; set; }
        /// <summary>
        /// شماره کارت
        /// </summary>
        public double? cardNumberList { get; set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public string accountNumberList { get; set; }
        /// <summary>
        /// شبا
        /// </summary>
        public string ibanList { get; set; }
        /// <summary>
        /// شرح در مورد نوع حساب
        /// </summary>
        public string explanationList { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }

    }
    public class BankAccountSearchViewModel
    {
        public bool organ { get; set; }
    }

}