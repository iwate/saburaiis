using System.Collections.Generic;

namespace SaburaIIS.Agent.Mappers
{
    public interface IMapper { }
    public interface IMapper<TSrc, TDst> : IMapper
    {
        void Map(TSrc src, TDst dst);
        TDst Map(TSrc src);
        IEnumerable<TDst> Map(IEnumerable<TSrc> sources);
    }
    public interface IMapperRegistry
    {
        IMapper<TSrc, TDst>? GetMapper<TSrc, TDst>();
        void Map<TSrc, TDst>(TSrc src, TDst dst);
        TDst Map<TSrc, TDst>(TSrc src) where TDst : new();
        IEnumerable<TDst> Map<TSrc, TDst>(IEnumerable<TSrc> sources) where TDst : new();
    }
}
