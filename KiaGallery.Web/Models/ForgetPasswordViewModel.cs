using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class ForgetPasswordViewModel
    {

        /// <summary>
        /// نام کاربری یا موبایل
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// متن کپچا
        /// </summary>
        public string captcha { get; set; }
        /// <summary>
        /// کد تایید
        /// </summary>
        public string confirmationCode { get; set; }
    }
}