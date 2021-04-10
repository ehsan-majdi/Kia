using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.InternalOrder
{
    /// <summary>
    /// سفارش مشتری با هر سنگ
    /// </summary>
    public class InternalOrderDetailStone
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف جزئیات سفارش
        /// </summary>
        public int InternalOrderDetailId { get; set; }
        /// <summary>
        /// ردیف سنگ
        /// </summary>
        public int StoneId { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// سفارشات داخلی مشتری
        /// </summary>
        public virtual InternalOrderDetail InternalOrderDetail { get; set; }
        /// <summary>
        ///  سنگ 
        /// </summary>
        public virtual Stone Stone { get; set; }
    }
}
