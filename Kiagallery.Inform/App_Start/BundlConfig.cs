using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Kiagallery.Inform.App_Start
{
    /// <summary>
    /// پیکره بندی باندل های استفاده شده در سایت
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// ثبت باندل های استفاده شده در برنامه
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/styles/core")
                .Include("~/Styles/uikit-rtl.css", new CssRewriteUrlTransform())
                .Include("~/Styles/uikit-override.css", new CssRewriteUrlTransform())
                .Include("~/Fonts/fontawsome/all.css", new CssRewriteUrlTransform())
                .Include("~/Fonts/sans/fontiran.css", new CssRewriteUrlTransform())
                .Include("~/Styles/general.css", new CssRewriteUrlTransform())
            );
            bundles.Add(new StyleBundle("~/styles/site")
                .Include("~/Styles/site.css", new CssRewriteUrlTransform())
            );
            bundles.Add(new ScriptBundle("~/scripts/site").Include(
              "~/Scripts/jquery-3.3.1.js",
               "~/Scripts/uikit.js",
               "~/Scripts/jsrender.js",
               "~/Scripts/template.js",
               "~/Scripts/jquery.general.js",
               "~/Scripts/app/jquery.validate.js",
               "~/Scripts/app/jquery.list.js",
               "~/Scripts/app/jquery.form.js",
               "~/Scripts/app/jquery.select.js",
               "~/Scripts/number-only.js",
               "~/Scripts/number-text.js"
            ));
        }
    }
}