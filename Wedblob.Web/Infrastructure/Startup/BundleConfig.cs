using Forloop.HtmlHelpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Wedblob.Web.Infrastructure.Startup
{
    public class StringArrayEqualityComparer : IEqualityComparer<string[]>
    {
        public static StringArrayEqualityComparer Instance = new StringArrayEqualityComparer();

        private StringArrayEqualityComparer() { }

        public bool Equals(string[] x, string[] y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            if (x.Length != y.Length)
                return false;

            for(var i=0; i < x.Length; i++)
            {
                if (!x[i].Equals(y[i]))
                    return false;
            }
            return true;
        }

        public int GetHashCode(string[] obj)
        {
            var hashCode = 0;
            if(obj != null)
            {
                for (var i = 0; i < obj.Length; i++)
                {
                    hashCode ^= obj[i].GetHashCode();
                }
            }
            return hashCode;
        }
    }

    public class BundleConfig : IStartupTask
    {
        public ConcurrentDictionary<string[], IHtmlString> ScriptToBundleCache = new ConcurrentDictionary<string[], IHtmlString>(StringArrayEqualityComparer.Instance);

        private class BundleInfo
        {
            public string Path { get; set; }
            public string[] ContentPaths { get; set; }
        }

        public int ExecutionOrder
        {
            get
            {
                return 1;
            }
        }

        public void Execute()
        {
            var bundleCollection = BundleTable.Bundles;

            //most of hte js files we use, we will just have one bundle with everything
            bundleCollection.Add(new ScriptBundle("~/Content/common-js")
               .Include("~/Content/lib/freewall/1.0.5/freewall.js")
               .Include("~/Content/lib/jqote2/0.9.8/jquery.jqote2.js")
               .Include("~/Content/lib/ladda/0.9.8/ladda.comb.min.js")
               .IncludeDirectory("~/Content/js", "*.js"));

            ScriptContext.ScriptPathResolver = ResolveToBundles;

        }


        private IHtmlString ResolveToBundles(string[] scriptsToResolve)
        {
            //we will find the most appropraite bundles to handle hte scripts we want
            return ScriptToBundleCache.GetOrAdd(scriptsToResolve, scripts =>
            {
                var bundleCollection = BundleTable.Bundles;
                var resolver = new BundleResolver(bundleCollection);

                var bundleInfos = bundleCollection.Select(x => new BundleInfo
                {
                    Path = x.Path,
                    ContentPaths = resolver.GetBundleContents(x.Path).Select(p => p.Replace("\\", "/")).ToArray()
                });

                var newScripts = new List<string>();
                var effectiveScripts = new HashSet<string>();
                foreach (var script in scripts)
                {
                    if (effectiveScripts.Contains(script))
                        continue;

                    double mostApplicableBundleApplicabilityScore = 0;
                    BundleInfo mostApplicableBundle = null;
                    foreach (var bundleInfo in bundleInfos)
                    {
                        var score = BundleApplicabilityScore(script, bundleInfo, scripts);
                        if (score > 0 && score > mostApplicableBundleApplicabilityScore)
                        {
                            mostApplicableBundleApplicabilityScore = score;
                            mostApplicableBundle = bundleInfo;
                        }
                    }

                    if (mostApplicableBundle == null)
                    {
                        newScripts.Add(script);
                        effectiveScripts.Add(script);
                    }
                    else
                    {
                        newScripts.Add(mostApplicableBundle.Path);
                        effectiveScripts.UnionWith(mostApplicableBundle.ContentPaths);
                    }

                }
                return System.Web.Optimization.Scripts.Render(newScripts.ToArray());
            });
            
        }

        private static double BundleApplicabilityScore(string script, BundleInfo bundle, string[] scripts)
        {
            //if bundle doesnt contain script its not applicable at all
            if (!bundle.ContentPaths.Contains(script))
                return 0;

            var scriptsCoveredByBundle = 1.0 * bundle.ContentPaths.Intersect(scripts).Count();

            return (scriptsCoveredByBundle / scripts.Length) * (scriptsCoveredByBundle / bundle.ContentPaths.Length);
        }
    }
}