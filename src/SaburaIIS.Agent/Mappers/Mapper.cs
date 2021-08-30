using System;
using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Mappers
{
    public class Mapper : IMapperRegistry
    {
        private readonly IMapper[] _mappers;

        public Mapper(bool configuableOnly = true)
        {
            var registries = new List<IMapper>() {
                new SiteMapper(this),
                new ApplicationPoolMapper(this),
                //new ApplicationDefaultsMapper(),
                //new ApplicationPoolDefaultsMapper(),
                //new SiteDefaultsMapper(),
                //new VirtualDirectoryDefaultsMapper(),
            };

            if (!configuableOnly)
            {
                registries.Add(new WorkerProcessMapper());
            }

            _mappers = registries.ToArray();
        }

        public IMapper<TSrc, TDst>? GetMapper<TSrc, TDst>()
        {
            foreach (var it in _mappers)
                if (it is IMapper<TSrc, TDst> mapper)
                    return mapper;

            return null;
        }
        public void Map<TSrc, TDst>(TSrc src, TDst dst)
        {
            var mapper = GetMapper<TSrc, TDst>();

            mapper?.Map(src, dst);
        }

        public TDst Map<TSrc, TDst>(TSrc src) where TDst : new()
        {
            var dst = new TDst();
            Map(src, dst);
            return dst;
        }

        public IEnumerable<TDst> Map<TSrc, TDst>(IEnumerable<TSrc> sources) where TDst : new()
        {
            return sources.Select(src => Map<TSrc, TDst>(src));
        }
    }
}
