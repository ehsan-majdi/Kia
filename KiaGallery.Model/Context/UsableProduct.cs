using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// محصولات مصرفی
    /// </summary>
    public class UsableProduct
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public UsableProduct()
        {
            UsableProductFileList = new List<UsableProductFile>();
            UsableProductCartList = new List<UsableProductCart>();
            OrderUsableProductDetailList = new List<OrderUsableProductDetail>();
        }
        /// <summary>
        ///  ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف دسته بندی
        /// </summary>
        public int CategoryUsableProductId { get; set; }
        /// <summary>
        /// ردیف دسته بندی چاپخانه
        /// </summary>
        public int PrintingHouseId { get; set; }
        /// <summary>
        /// نام پک محصول
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// واحد
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// کد پک محصول
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ترتیب پک محصول
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// قیمت محصول
        /// </summary>
        public long Price { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public UsableProductStatus UsableProductStatus { get; set; }
        /// <summary>
        /// وضعیت موجودی محصول
        /// </summary>
        public bool Available { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// دریافت عنوان نوع شعبه
        /// </summary>
        public BranchType BranchType { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده 
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// ایجاد دسته بندی محصول
        /// </summary>
        public virtual CategoryUsableProduct CategoryUsableProduct { get; set; }
        /// <summary>
        /// ایجاد نوع چاپخانه
        /// </summary>
        public virtual PrintingHouse PrintingHouse { get; set; }
        /// <summary>
        /// جزئیات سفارش محصول مصرفی
        /// </summary>
        public virtual List<OrderUsableProductDetail> OrderUsableProductDetailList { get; set; }
        /// <summary>
        /// لیست سبد خرید
        /// </summary>
        public virtual List<UsableProductCart> UsableProductCartList { get; set; }

        /// <summary>
        /// لیست فایل های محصول
        /// </summary>
        public virtual List<UsableProductFile> UsableProductFileList { get; set; }
    }
}
