using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// سنگ های جزئیات محصول سفارش
    /// </summary>
    [Table(name: "OrderDetailStone", Schema = "order")]
    public class OrderDetailStone
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
        /// ردیف سنگ
        /// </summary>
        public int? StoneId { get; set; }

        /// <summary>
        /// محصول سفارش داده شده
        /// </summary>
        public virtual OrderDetail OrderDetail { get; set; }
        /// <summary>
        /// سنگ
        /// </summary>
        public virtual Stone Stone { get; set; }
    }
}
