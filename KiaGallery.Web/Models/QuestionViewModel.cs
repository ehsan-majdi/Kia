using KiaGallery.Model;
using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// سوالات سیستم مدیریت ارتباط با مشتری 
    /// </summary>
    public class QuestionViewModel
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
        /// ردیف دسته بندی سوالات
        /// </summary>
        /// 
        public int categoryQuestionId { get; set; }
        /// <summary>
        /// نوع پرسش
        /// </summary>
        public CrmQuestionType crmQuestionType { get; set; }
        /// <summary>
        /// نوع خرید
        /// </summary>
        public BuyType buyType { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset buyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline buyTypeOnline { get; set; }

        /// <summary>
        /// عنوان دسته بندی
        /// </summary>
        public string categoryQuestionTitle { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        ///بله / خیر 
        /// </summary>
        public bool? defaultYesNo { get; set; }
        /// <summary>
        /// تشریحی
        /// </summary>
        public string defaultDescriptive { get; set; }
        /// <summary>
        /// لیست مقادیر 
        /// </summary>
        public List<CrmQuestionValueViewModel> crmQuestionValueViewModelList { get; set; }
        /// <summary>
        /// دسته بندی سوالات
        /// </summary>
        public virtual CategoryQuestion categoryQuestion { get; set; }
    }
    /// <summary>
    /// مقدار سوالات سیستم مدیریت ارتباط با مشتری
    /// </summary>
    public class CrmQuestionValueViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف سوالات
        /// </summary>
        public int crmQuestionId { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// مقدار پیش فرض
        /// </summary>
        public bool defaultSelected { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }

    }
    /// <summary>
    /// جستجوی سوالات
    /// </summary>
    public class CrmQuestionSearchViewModel
    {
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? branchId { get; set; }
        /// <summary>
        /// نوع خرید
        /// </summary>
        public BuyType buyType { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset buyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline buyTypeOnline { get; set; }
    }

    /// <summary>
    /// توع خرید
    /// </summary>
    public class CrmBuyTypeViewModel
    {
        public int id { get; set; }
        /// <summary>
        /// نوع خرید
        /// </summary>
        public BuyType buyType { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset buyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline buyTypeOnline { get; set; }
    }

    /// <summary>
    /// دسته بندی سوالات
    /// </summary>
    public class CategoryQuestionViewModel
    {
        /// <summary>
        /// ردیف دسته بندی
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ترتیب دسته بندی
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// عنوان دسته بندی
        /// </summary>
        public string title { get; set; }
    }
    /// <summary>
    /// لیست دسته بندی سوالات
    /// </summary>
    public class CategorySearchViewModel
    {
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
    }
    /// <summary>
    /// سوالات سیستم مدیریت ارتباط با مشتری 
    /// </summary>
    public class CrmCustomerViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// نام کامل
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// شماره تماس
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public int factorNumber { get; set; }
        /// <summary>
        /// نوع خرید مشتری
        /// </summary>
        public BuyType buyType { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset buyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline buyTypeOnline { get; set; }
        /// <summary>
        /// تاریخ 
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// امتیاز
        /// </summary>
        public int score { get; set; }
        /// <summary>
        /// لیست مقادیر سوالات
        /// </summary>
        public virtual List<CrmCustomerAnswerViewModel> answerList { get; set; }

    }

    /// <summary>
    /// جواب سوالات سیستم مدیریت ارتباط با مشتری
    /// </summary>
    public class CrmCustomerAnswerViewModel
    {
        /// <summary>
        /// ردیف سوال
        /// </summary>
        public int crmQuestionId { get; set; }
        /// <summary>
        /// نوع پرسش
        /// </summary>
        public CrmQuestionType crmQuestionType { get; set; }
        /// <summary>
        /// نوع خرید
        /// </summary>
        public BuyType buyType { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset buyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline buyTypeOnline { get; set; }
        /// <summary>
        /// جواب بله خیر
        /// </summary>
        public bool? yesNoAnswer { get; set; }
        /// <summary>
        /// ردیف مقدار سوال
        /// </summary>
        public string crmQuestionValueId { get; set; }
        /// <summary>
        /// جواب تشریحی
        /// </summary>
        public string descriptiveAnswer { get; set; }

    }
    /// <summary>
    /// لیست ارائه مشخصات مشتریان ثبت شده توسط شعب کیاگالری
    /// </summary>
    public class CrmCustomerSearchReportViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// شماره صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// نام مشتری
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// شماره تماس مشتری
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// نام مشتری
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// شروع تاریخ جاری میلادی
        /// </summary>
        public DateTime fromDateToAc { get; set; }
        /// <summary>
        /// شروع تاریخ جاری به شمسی
        /// </summary>
        public string fromDate { get; set; }
        /// <summary>
        /// پایان تاریخ انتخاب شده به میلادی
        /// </summary>
        public DateTime toDateToAc { get; set; }
        /// <summary>
        /// پایان تاریخ انتخاب شده به شمسی
        /// </summary>
        public string toDate { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset buyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline buyTypeOnline { get; set; }
    }
    /// <summary>
    /// گزارش اکسل مشتریان ثبت شده توسط شعب
    /// </summary>
    public class CrmCustomerReportViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// لیست مشتریان
        /// </summary>
        public List<string> customerName { get; set; }
        /// <summary>
        /// لیست شعب
        /// </summary>
        public List<string> branchName { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset buyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline buyTypeOnline { get; set; }

    }
}