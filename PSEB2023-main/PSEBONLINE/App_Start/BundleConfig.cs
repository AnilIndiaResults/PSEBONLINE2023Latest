using System.Web;
using System.Web.Optimization;

namespace PSEBONLINE
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
           "~/Scripts/jquery.dataTables.min.js",
                     "~/Scripts/dataTables.bootstrap4.min.js",
                "~/Scripts/jquery-{version}.js",
            "~/Scripts/common.js",
            "~/Scripts/punjabi.js",
            "~/Scripts/keyboard.js",
            "~/Scripts/jquery-1.12.4.js",
            "~/Scripts/jquery-ui.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            // Boorstrap dropdown select
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select").Include(
                                  "~/Scripts/bootstrap-select.js",
                                  "~/Scripts/script-bootstrap-select.js"));

#endregion

            #region Styles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/keyboard.css",
                      "~/Content/style.css",
                      "~/Content/kdcs.css",
                      "~/Content/site.css"));


            // Bootstrap dropdown select
            bundles.Add(new StyleBundle("~/Content/Bootstrap-Select/css").Include(
                                 "~/Content/style/bootstrap-select.css",
                                 "~/Content/style/bootstrap-select.min.css"));
            #endregion
        }
    }
}
