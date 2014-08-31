using System.Web;
using System.Web.Optimization;

namespace AreaOfInterest
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/application").Include(
                
                "~/Js/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/leaflet").Include(
                "~/Scripts/d3/d3.js",
                "~/Scripts/evispa-timo-jsclipper/clipper.js",
                "~/Scripts/graham_scan/graham_scan.js",
                "~/Scripts/hammerjs/hammer.js",
                "~/Scripts/concavehull/concavehull.js",            
                "~/Scripts/leaflet-{version}.js",
                "~/Scripts/leaflet.*"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/leaflet.css",
                      "~/Content/site.css"));
        }
    }
}
