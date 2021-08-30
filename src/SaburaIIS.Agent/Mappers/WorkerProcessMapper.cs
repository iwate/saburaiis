using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Mappers
{
    public class WorkerProcessMapper : IMapper<WorkerProcess, POCO.WorkerProcess>
    {
        public void Map(WorkerProcess src, POCO.WorkerProcess dst)
        {
            dst.ApplicationDomains = src.ApplicationDomains.Select(domain => new POCO.ApplicationDomain
            {
                Id = domain.Id,
                Idle = domain.Idle,
                PhysicalPath = domain.PhysicalPath,
                VirtualPath = domain.VirtualPath,
                WorkerProcess = domain.WorkerProcess == src ? null : new POCO.WorkerProcess
                {
                    AppPoolName = domain.WorkerProcess.AppPoolName,
                    ProcessGuid = domain.WorkerProcess.ProcessGuid,
                    ProcessId = domain.WorkerProcess.ProcessId,
                    State = (POCO.WorkerProcessState)(int)domain.WorkerProcess.State
                }
            }).ToList();
            dst.AppPoolName = src.AppPoolName;
            dst.ProcessGuid = src.ProcessGuid;
            dst.ProcessId = src.ProcessId;
            dst.State = (POCO.WorkerProcessState)(int)src.State;
        }

        public POCO.WorkerProcess Map(WorkerProcess src)
        {
            var dst = new POCO.WorkerProcess();
            Map(src, dst);
            return dst;
        }

        public IEnumerable<POCO.WorkerProcess> Map(IEnumerable<WorkerProcess> sources)
        {
            return sources.Select(Map);
        }
    }
}
