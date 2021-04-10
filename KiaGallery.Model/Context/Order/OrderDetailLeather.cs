using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// چرم های جزئیات محصول سفارش
    /// </summary>
    [Table(name: "OrderDetailLeather", Schema = "order")]
    public class OrderDetailLeather
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
        /// ترتیب
        /// </summary>
        [Column("OrderNo")]
        public int Order { get; set; }
        /// <summary>
        /// ردیف چرم
        /// </summary>
        public int? LeatherId { get; set; }

        /// <summary>
        /// محصول سفارش داده شده
        /// </summary>
        public virtual OrderDetail OrderDetail { get; set; }
        /// <summary>
        /// چرم
        /// </summary>
        public virtual Leather Leather { get; set; }
    }
}
