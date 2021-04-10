
namespace KiaGallery.Model.Context
{
    /// <summary>
    /// خرج کار محصول
    /// </summary>
    public class ProductOuterWerk
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
        /// نوع خرج کار
        /// </summary>
        public OuterWerkType OuterWerkType { get; set; }
        /// <summary>
        /// محصول
        /// </summary>
        public virtual Product Product { get; set; }
    }
}
