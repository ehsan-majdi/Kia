using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// جدول کمکی تعداد فاکتور شعب برای تخمین مشتریان وفادار
    /// </summary>
    public class BranchFactorViewModel
    {
        /// <summary>
        /// ردیف فاکتورها
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int branchId { get; set; }
        /// <summary>
        /// تعداد فاکتورها
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// تاریخ تعداد فاکتور 
        /// </summary>
        public string date { get; set; }

    }
    /// <summary>
    /// جدول کمکی برای مشاهده تعداد فاکتور ورودی هر شعب و مشاهده نام شعب
    /// </summary>
    public class BranchFactorSearchViewModel
    {
        /// <summary>
        /// ردیف فاکتور
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? branchId { get; set; }
        /// <summary>
        /// تعداد فاکتورها
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// تاریخ تعداد فاکتور 
        /// </summary>
        public DateTime? date { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// کل فاکتور مربوط به شعب
        /// </summary>
        public int factorCount { get; set; }
    }
    /// <summary>
    /// جدول کمکی برای نمایش موارد ذخیره شده اعم از تعداد فاکتور هر شعب به همراه نام شعبه
    /// </summary>
    public class GetdetailViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// تعداد فاکتور
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string persianDate { get; set; }
    }

}