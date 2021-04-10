using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// چرم سبد خرید
    /// </summary>
    [Table(name: "CartProductLeather", Schema = "order")]
    public class CartProductLeather
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سبد خرید
        /// </summary>
        public int CartId { get; set; }
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
        /// محصول سبد خرید
        /// </summary>
        public virtual Cart Cart { get; set; }
        /// <summary>
        /// چرم
        /// </summary>
        public virtual Leather Leather { get; set; }
    }
}
