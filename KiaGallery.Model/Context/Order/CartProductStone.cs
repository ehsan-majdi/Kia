using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// سنگ سبد خرید
    /// </summary>
    [Table(name: "CartProductStone", Schema = "order")]
    public class CartProductStone
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
        /// ردیف سنگ
        /// </summary>
        public int? StoneId { get; set; }
        /// <summary>
        /// سبد خرید
        /// </summary>
        public virtual Cart Cart { get; set; }
        /// <summary>
        /// سنگ
        /// </summary>
        public virtual Stone Stone { get; set; }
    }
}
