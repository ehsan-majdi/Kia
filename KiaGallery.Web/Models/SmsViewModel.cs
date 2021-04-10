using KiaGallery.Model;
using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    public class GetSmsTextViewModel
    {
        public string title { get; set; }
        public string text { get; set; }
        public int order { get; set; }
    }
    public class SmsCategoryViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// پیام آزاد
        /// </summary>
        public string freeMessage { get; set; }
        /// <summary>
        /// رنگ
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// تشریحی
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// نوع پیام
        /// </summary>
        public SmsCategoryType categoryType { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// لیست پیام های ایجاد شده
        /// </summary>
        public List<CreateMessageViewModel> createMessageList { get; set; }

    }
    /// <summary>
    /// لیست پیام های ایجاد شده
    /// </summary>
    public class CreateMessageViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف دسته بندی
        /// </summary>
        public int smsCategoryId { get; set; }
        /// <summary>
        /// اسم 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// جزئیات تشریحی اسم
        /// </summary>
        public string detailName { get; set; }
        /// <summary>
        /// مقدار پیش فرض
        /// </summary>
        public bool defaultSelected { get; set; }
    }
    public class SmsTextViewModel
    {
        public int? id { get; set; }
        public int smsCategoryId { get; set; }
        public string title { get; set; }
        [AllowHtml]
        public string text { get; set; }
        public string urlKey { get; set; }
        public int order { get; set; }
        public string categoryTitle { get; set; }
        public bool active { get; set; }

    }
    public class SendMessageViewModel
    {

        public string phoneNumber { get; set; }
        public string text { get; set; }
    }

    public class SmsOption
    {
        public int? id { get; set; }
        public string dayOfWeek { get; set; }
        public string dayOfMonth { get; set; }
        public int sendingTimeMethod { get; set; }
        public string branchId { get; set; }
        public string userId { get; set; }
        public string phoneNumber { get; set; }
        public string text { get; set; }
        public string persianDate { get; set; }
        public string time { get; set; }
    }
    public class SearchSmsViewModel
    {
        public int id { get; set; }
        public string branchId { get; set; }
        public string userId { get; set; }
        public string phoneNumber { get; set; }
        public string text { get; set; }
        public int sendingTimeMethod { get; set; }
        public string dayOfWeek { get; set; }
        public string dayOfMonth { get; set; }
        public DateTime date { get; set; }
        public string persianDate { get; set; }
        public string time { get; set; }
        public int page { get; set; }
        public int count { get; set; }
        public bool sent { get; set; }
        public int type { get; set; }
    }
}