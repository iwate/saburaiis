using Microsoft.Web.Administration;
using System.Linq;

namespace SaburaIIS.Agent.Transformers
{
    public class VirtualDirectoryCollectionTransformer : ITransformer
    {
        public void Transform(object obj, IDelta delta)
        {
            var colleciton = (VirtualDirectoryCollection)obj;

            if (delta.Method == DeltaMethod.Add)
            {
                var path = (string?)delta.Key;
                if (path == "/")
                {
                    var vdir = colleciton.First(item => item.Path == path);
                    Transformer.Transform(vdir, delta);
                }
                else
                {
                    var physicalPath = (string?)delta.ValueProperties[nameof(POCO.VirtualDirectory.PhysicalPath)].newValue;
                    var vdir = colleciton.Add(path, physicalPath);
                    Transformer.Transform(vdir, delta);
                }
            }
            else if (delta.Method == DeltaMethod.Remove)
            {
                var vdir = colleciton.First(item => item.Path == (string?)delta.Key);
                colleciton.Remove(vdir);
            }
            else if (delta.Method == DeltaMethod.Update)
            {
                var vdir = colleciton.First(item => item.Path == (string?)delta.Key);
                Transformer.Transform(vdir, delta);
            }
        }
    }
}
