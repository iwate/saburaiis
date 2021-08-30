using Microsoft.Web.Administration;
using System.Linq;

namespace SaburaIIS.Agent.Transformers
{
    public class ApplicationPoolCollectionTransformer : ITransformer
    {
        public void Transform(object obj, IDelta delta)
        {
            var colleciton = (ApplicationPoolCollection)obj;

            if (delta.Method == DeltaMethod.Add)
            {
                var pool = colleciton.Add((string?)delta.Key);
                Transformer.Transform(pool, delta);
            }
            else if (delta.Method == DeltaMethod.Remove)
            {
                var pool = colleciton.First(item => item.Name == (string?)delta.Key);
                colleciton.Remove(pool);
            }
            else if (delta.Method == DeltaMethod.Update)
            {
                var pool = colleciton.First(item => item.Name == (string?)delta.Key);
                Transformer.Transform(pool, delta);
            }
        }
    }
}
