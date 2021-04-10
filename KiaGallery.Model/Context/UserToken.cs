using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// توکن کاربر
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// توکن اعتبار سنجی
        /// </summary>
        [MaxLength(1024)]
        public string AuthoritarianToken { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// تاریخ انقضاء
        /// </summary>
        public DateTime? ExpiredDateTime { get; set; }
        /// <summary>
        /// آی پی ایجاد کننده
        /// </summary>
        [MaxLength(48)]
        public string CreatedIp { get; set; }

        /// <summary>
        /// کاربر
        /// </summary>
        public virtual User User { get; set; }
    }
}
