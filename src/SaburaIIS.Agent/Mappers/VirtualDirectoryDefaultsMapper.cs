using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Mappers
{
    public class VirtualDirectoryDefaultsMapper : IMapper<VirtualDirectoryDefaults, POCO.VirtualDirectoryDefaults>
    {
        public void Map(VirtualDirectoryDefaults src, POCO.VirtualDirectoryDefaults dst)
        {
            dst.LogonMethod = (POCO.AuthenticationLogonMethod)(int)src.LogonMethod;
            dst.UserName = src.UserName;
            dst.Password = src.Password;
        }

        public POCO.VirtualDirectoryDefaults Map(VirtualDirectoryDefaults src)
        {
            var dst = new POCO.VirtualDirectoryDefaults();
            Map(src, dst);
            return dst;
        }

        public IEnumerable<POCO.VirtualDirectoryDefaults> Map(IEnumerable<VirtualDirectoryDefaults> sources)
        {
            return sources.Select(Map);
        }
    }
}
