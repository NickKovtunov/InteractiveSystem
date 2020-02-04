using System.Web;
using System.Web.Optimization;

namespace interactiveSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryscroll").Include(
                      "~/Scripts/scroll/jquery-1.11.0.min.js",
                        "~/Scripts/scroll/jquery-ui-1.10.4.min.js",
                        "~/Scripts/scroll/jquery.mousewheel.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/styleScript.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/reset.css",
                      "~/Content/site.css",
                      "~/Content/historyStyle.css"));

            bundles.Add(new StyleBundle("~/Content/datepicker").Include(
                      "~/Content/datepicker.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                      "~/Scripts/datepicker.min.js"));
        }
    }
}
