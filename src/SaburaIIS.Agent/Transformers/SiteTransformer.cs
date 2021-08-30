using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaburaIIS.Agent.Transformers
{
    public class SiteTransformer : DefaultTransformer
    {
        protected override string[] IgnoreProps { get; } = new[] { nameof(POCO.Site.Id), nameof(POCO.Site.State) };
    }
}
