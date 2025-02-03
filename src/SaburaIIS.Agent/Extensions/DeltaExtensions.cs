using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Extensions
{
    public static class DeltaExtensions
    {
        public static IEnumerable<(string? newValue, string? oldValue)> GetPhysicalPathChanges(this IEnumerable<IDelta> dSites)
            => dSites.SelectMany(dsite =>
                    dsite.NestCollectionProperties[nameof(POCO.Site.Applications)].SelectMany(dapp =>
                        dapp.NestCollectionProperties[nameof(POCO.Application.VirtualDirectories)]
                            .Where(dvdir => dvdir.ValueProperties.ContainsKey(nameof(POCO.VirtualDirectory.PhysicalPath)))
                            .Select(dvdir => dvdir.ValueProperties[nameof(POCO.VirtualDirectory.PhysicalPath)])))
                            .Select(changes => ((string?)changes.newValue, (string?)changes.oldValue));

        public static IEnumerable<string> GetApplicationPoolNames(this IEnumerable<IDelta> dSites, IEnumerable<POCO.Site> sites)
        {
            var apppools = new List<string>();
            foreach (var dsite in dSites)
            {
                var site = sites.FirstOrDefault(s => s.Name == (string?)dsite.Key);
                if (site == null)
                {
                    continue;
                }

                foreach (var dapp in dsite.NestCollectionProperties["Applications"])
                {
                    if (dapp.Method == DeltaMethod.Update && dapp.HasDiff)
                    {
                        var app = site.Applications.FirstOrDefault(a => a.Path == (string?)dapp.Key);
                        if (app == null)
                        {
                            continue; 
                        }
                        apppools.Add(app.ApplicationPoolName);
                    }
                }
            }
            return apppools;
        }
    }
}
