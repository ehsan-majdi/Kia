using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Areas.Bot.Models
{
    public class BotSettingsViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// کلید
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public string valueFa { get; set; }
    }
}