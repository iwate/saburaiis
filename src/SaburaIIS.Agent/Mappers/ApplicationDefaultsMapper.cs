using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Mappers
{
    public class ApplicationDefaultsMapper : IMapper<ApplicationDefaults, POCO.ApplicationDefaults>
    {
        public void Map(ApplicationDefaults src, POCO.ApplicationDefaults dst)
        {
            dst.ApplicationPoolName = src.ApplicationPoolName;
            dst.EnabledProtocols = src.EnabledProtocols;
        }

        public POCO.ApplicationDefaults Map(ApplicationDefaults src)
        {
            var dst = new POCO.ApplicationDefaults();
            Map(src, dst);
            return dst;
        }

        public IEnumerable<POCO.ApplicationDefaults> Map(IEnumerable<ApplicationDefaults> sources)
        {
            return sources.Select(Map);
        }
    }
}
