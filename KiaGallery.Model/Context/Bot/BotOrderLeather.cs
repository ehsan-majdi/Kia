using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// چرم های محصول ربات سفارش
    /// </summary>
    [Table(name: "BotOrderLeather", Schema = "order")]
    public class BotOrderLeather
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف محصول سفارش داده شده
        /// </summary>
        public int OrderId { get; set; }
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
        public virtual BotOrder BotOrder { get; set; }
        /// <summary>
        /// چرم
        /// </summary>
        public virtual Leather Leather { get; set; }
    }
}
