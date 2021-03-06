using Microsoft.Web.Administration;
using System.Linq;

namespace SaburaIIS.Agent.Transformers
{
    public class SiteCollectionTransformer : ITransformer
    {
        public void Transform(object obj, IDelta delta)
        {
            var colleciton = (SiteCollection)obj;

            if (delta.Method == DeltaMethod.Add)
            {
                var app = colleciton.Add((string?)delta.Key, string.Empty, 80);
                Transformer.Transform(app, delta);
            }
            else if (delta.Method == DeltaMethod.Remove)
            {
                var app = colleciton.First(item => item.Name == (string?)delta.Key);
                colleciton.Remove(app);
            }
            else if (delta.Method == DeltaMethod.Update)
            {
                var app = colleciton.First(item => item.Name == (string?)delta.Key);
                Transformer.Transform(app, delta);
            }
        }
    }
}
