using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// سنگ های جزئیات محصول سفارش
    /// </summary>
    [Table(name: "BotOrderStone", Schema = "order")]
    public class BotOrderStone
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
        /// ردیف سنگ
        /// </summary>
        public int? StoneId { get; set; }

        /// <summary>
        /// محصول ربات سفارش داده شده
        /// </summary>
        public virtual BotOrder BotOrder { get; set; }
        /// <summary>
        /// سنگ
        /// </summary>
        public virtual Stone Stone { get; set; }
    }
}
