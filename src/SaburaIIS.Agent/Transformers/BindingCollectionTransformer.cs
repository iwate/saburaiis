using Microsoft.Web.Administration;
using System.Linq;

namespace SaburaIIS.Agent.Transformers
{
    public class BindingCollectionTransformer : ITransformer
    {
        public void Transform(object obj, IDelta delta)
        {
            var colleciton = (BindingCollection)obj;

            if (delta.Method == DeltaMethod.Add)
            {
                var app = colleciton.Add((string?)delta.Key, string.Empty);
                Transformer.Transform(app, delta);
            }
            else if (delta.Method == DeltaMethod.Remove)
            {
                var app = colleciton.First(item => item.BindingInformation == (string?)delta.Key);
                colleciton.Remove(app);
            }
            else if (delta.Method == DeltaMethod.Update)
            {
                var app = colleciton.First(item => item.BindingInformation == (string?)delta.Key);
                Transformer.Transform(app, delta);
            }
        }
    }
}
