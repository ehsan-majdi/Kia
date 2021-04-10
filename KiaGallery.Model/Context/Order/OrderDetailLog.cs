﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// سوابق محصول سفارش داده شده
    /// </summary>
    [Table(name: "OrderDetailLog", Schema = "order")]
    public class OrderDetailLog
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف محصول سفارش داده شده
        /// </summary>
        public int OrderDetailId { get; set; }
        /// <summary>
        /// وضعیت محصول سفارش داده شده
        /// </summary>
        public OrderDetailStatus OrderDetailStatus { get; set; }
        /// <summary>
        /// ردیف علت ثبت شده برای وضعیت محصول سفارش داده شده
        /// </summary>
        public int? OrderDetailLogReasonId { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(45)]
        public string Ip { get; set; }

        /// <summary>
        /// محصول سفارش داده شده
        /// </summary>
        public virtual OrderDetail OrderDetail { get; set; }
        /// <summary>
        /// علت ثبت شده برای وضعیت محصول سفارش داده شده
        /// </summary>
        public virtual OrderDetailLogReason OrderDetailLogReason { get; set; }
        /// <summary>
        /// کاربر ایحاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
    }
}
