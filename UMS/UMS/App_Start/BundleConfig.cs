using System.Web;
using System.Web.Optimization;

namespace UMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                            "~/Scripts/jquery-{version}.js",
                            "~/Scripts/jquery-ui-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                            "~/Scripts/jquery.validate*"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryDataTables").Include(
                            "~/DataTables-1.10.15/media/js/jquery.dataTables.min.js",
                            "~/DataTables-1.10.15/media/js/dataTables.bootstrap.min.js",    
                            "~/Scripts/DataTables/dataTables.buttons.min.js",
                            "~/Scripts/jszip.js",
                            "~/Scripts/DataTables/buttons.html5.min.js"

                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr*"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                            "~/Scripts/bootstrap.js",
                            "~/Scripts/respond.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                            "~/Content/bootstrap.css",
                            "~/Content/site.css"
                        ));

            bundles.Add(new StyleBundle("~/bundles/DataTables").Include(
                            "~/DataTables-1.10.15/media/css/jquery.dataTables.css",
                            "~/DataTables-1.10.15/media/css/dataTables.bootstrap.css"

                        ));
        }
    }
}
