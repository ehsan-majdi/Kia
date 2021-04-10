using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// سنگ های محصول
    /// </summary>
    public class ProductStone
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف محصول
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        [Column("OrderNo")]
        public int Order { get; set; }
        /// <summary>
        /// ردیف سنگ پیش فرض
        /// </summary>
        public int? DefaultStoneId { get; set; }
        /// <summary>
        /// ردیف سنگ
        /// </summary>
        public int StoneId { get; set; }
        /// <summary>
        /// شکل سنگ
        /// </summary>
        public StoneShape StoneShape { get; set; }
        /// <summary>
        /// ردیف ابعاد شکل سنگ
        /// </summary>
        public int? ShapeSizeId { get; set; }

        /// <summary>
        /// ابعاد شکل سنگ
        /// </summary>
        public virtual ShapeSize ShapeSize { get; set; }
        /// <summary>
        /// محصول
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// سنگ پیش فرض
        /// </summary>
        public virtual Stone DefaultStone { get; set; }
        /// <summary>
        /// سنگ
        /// </summary>
        public virtual Stone Stone { get; set; }
    }
}
