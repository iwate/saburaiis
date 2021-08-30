using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaburaIIS.Agent.Mappers
{
    public class SiteMapper : IMapper<Site, POCO.Site>
    {
        private readonly IMapperRegistry _mapper;
        public SiteMapper(IMapperRegistry mapper)
        {
            _mapper = mapper;
        }
        public void Map(Site src, POCO.Site dst)
        {
            dst.Id = src.Id;
            //dst.ApplicationDefaults = _mapper.Map<ApplicationDefaults, POCO.ApplicationDefaults>(src.ApplicationDefaults)
            dst.Applications = src.Applications.Select(app => new POCO.Application
            {
                ApplicationPoolName = app.ApplicationPoolName,
                EnabledProtocols = app.EnabledProtocols,
                Path = app.Path,
                VirtualDirectories = app.VirtualDirectories.Select(vdir => new POCO.VirtualDirectory
                {
                    LogonMethod = (POCO.AuthenticationLogonMethod)(int)vdir.LogonMethod,
                    Password = vdir.Password,
                    Path = vdir.Path,
                    PhysicalPath = vdir.PhysicalPath,
                    UserName = vdir.UserName
                }).ToList(),
                //VirtualDirectoryDefaults = _mapper.Map<VirtualDirectoryDefaults, POCO.VirtualDirectoryDefaults>(app.VirtualDirectoryDefaults)
            }).ToList();
            dst.Bindings = src.Bindings.Select(binding => new POCO.Binding
            {
                BindingInformation = binding.BindingInformation,
                CertificateHash = binding.CertificateHash,
                CertificateStoreName = binding.CertificateStoreName,
                Host = binding.Host,
                IsIPPortHostBinding = binding.IsIPPortHostBinding,
                Protocol = binding.Protocol,
                SslFlags = (POCO.SslFlags)(int)binding.SslFlags,
                UseDsMapper = binding.UseDsMapper,
            }).ToList();
            dst.HSTS = new POCO.SiteHSTS
            {
                Enabled = src.HSTS.Enabled,
                IncludeSubDomains = src.HSTS.IncludeSubDomains,
                MaxAge = src.HSTS.MaxAge,
                Preload = src.HSTS.Preload,
                RedirectHttpToHttps = src.HSTS.RedirectHttpToHttps,
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
            dst.TraceFailedRequestsLogging = new POCO.SiteTraceFailedRequestsLogging
            {
                Directory = src.TraceFailedRequestsLogging.Directory,
                Enabled = src.TraceFailedRequestsLogging.Enabled,
                MaxLogFiles = src.TraceFailedRequestsLogging.MaxLogFiles,
            };
            dst.Name = src.Name;
            //dst.VirtualDirectoryDefaults = new POCO.VirtualDirectoryDefaults
            //{
            //    LogonMethod = (POCO.AuthenticationLogonMethod)(int)src.VirtualDirectoryDefaults.LogonMethod,
            //    UserName = src.VirtualDirectoryDefaults.UserName,
            //    Password = src.VirtualDirectoryDefaults.Password
            //};
            dst.ServerAutoStart = src.ServerAutoStart;
            dst.State = (POCO.ObjectState)(int)src.State;
        }

        public POCO.Site Map(Site src)
        {
            var dst = new POCO.Site();
            Map(src, dst);
            return dst;
        }

        public IEnumerable<POCO.Site> Map(IEnumerable<Site> sources)
        {
            return sources.Select(Map);
        }
    }
}
