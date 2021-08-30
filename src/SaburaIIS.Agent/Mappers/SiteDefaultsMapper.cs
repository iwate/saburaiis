using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaburaIIS.Agent.Mappers
{
    public class SiteDefaultsMapper : IMapper<SiteDefaults, POCO.SiteDefaults>
    {
        public void Map(SiteDefaults src, POCO.SiteDefaults dst)
        {
            dst.HSTS = new POCO.SiteHSTS
            {
                Enabled = src.HSTS.Enabled,
                IncludeSubDomains = src.HSTS.IncludeSubDomains,
                MaxAge = src.HSTS.MaxAge,
                Preload = src.HSTS.Preload,
                RedirectHttpToHttps = src.HSTS.RedirectHttpToHttps
            };
            dst.Limits = new POCO.SiteLimits
            {
                ConnectionTimeout = src.Limits.ConnectionTimeout,
                MaxBandwidth = src.Limits.MaxBandwidth,
                MaxConnections = src.Limits.MaxConnections,
                MaxUrlSegments = src.Limits.MaxUrlSegments
            };
            dst.LogFile = new POCO.SiteLogFile
            {
                CustomLogFields = src.LogFile.CustomLogFields.Select(custom => new POCO.CustomLogField
                {
                    LogFieldName = custom.LogFieldName,
                    SourceName = custom.SourceName,
                    SourceType = (POCO.CustomLogFieldSourceType)(int)custom.SourceType
                }).ToList(),
                CustomLogPluginClsid = src.LogFile.CustomLogPluginClsid,
                Directory = src.LogFile.Directory,
                Enabled = src.LogFile.Enabled,
                LocalTimeRollover = src.LogFile.LocalTimeRollover,
                LogExtFileFlags = (POCO.LogExtFileFlags)(int)src.LogFile.LogExtFileFlags,
                LogFormat = (POCO.LogFormat)(int)src.LogFile.LogFormat,
                LogTargetW3C = (POCO.LogTargetW3C)(int)src.LogFile.LogTargetW3C,
                Period = (POCO.LoggingRolloverPeriod)(int)src.LogFile.Period,
                TruncateSize = src.LogFile.TruncateSize,
            };
            dst.ServerAutoStart = src.ServerAutoStart;
            dst.TraceFailedRequestsLogging = new POCO.SiteTraceFailedRequestsLogging
            {
                Directory = src.TraceFailedRequestsLogging.Directory,
                Enabled = src.TraceFailedRequestsLogging.Enabled,
                MaxLogFiles = src.TraceFailedRequestsLogging.MaxLogFiles,
            };
        }

        public POCO.SiteDefaults Map(SiteDefaults src)
        {
            var dst = new POCO.SiteDefaults();
            Map(src, dst);
            return dst;
        }

        public IEnumerable<POCO.SiteDefaults> Map(IEnumerable<SiteDefaults> sources)
        {
            return sources.Select(Map);
        }
    }
}
