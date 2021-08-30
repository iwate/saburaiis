using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Transformers
{
    public class Transformer
    {
        private static readonly ITransformer _defaultTransformer = new DefaultTransformer();
        private static readonly IDictionary<Type, ITransformer> _transformers = new Dictionary<Type, ITransformer>
        {
            [typeof(ApplicationPoolCollection)] = new ApplicationPoolCollectionTransformer(),
            [typeof(ApplicationCollection)] = new ApplicationCollectionTransformer(),
            [typeof(ScheduleCollection)] = new ScheduleCollectionTransformer(),
            [typeof(SiteCollection)] = new SiteCollectionTransformer(),
            [typeof(BindingCollection)] = new BindingCollectionTransformer(),
            [typeof(VirtualDirectoryCollection)] = new VirtualDirectoryCollectionTransformer(),
            [typeof(WorkerProcessCollection)] = new NoneTransformer(),
            [typeof(Site)] = new SiteTransformer(),
        };
        private static ITransformer GetTransformer(Type type)
        {
            if (_transformers.ContainsKey(type))
                return _transformers[type];

            return _defaultTransformer;
        }

        public static void Transform(object obj, IDelta delta)
        {
            var transformer = GetTransformer(obj.GetType());

            transformer.Transform(obj, delta);
        }
    }

    public class CustomLogFieldCollectionTransformer : ITransformer
    {
        public void Transform(object obj, IDelta delta)
        {
            var colleciton = (CustomLogFieldCollection)obj;

            if (delta.Method == DeltaMethod.Add)
            {
                var app = colleciton.Add((string?)delta.Key, string.Empty, CustomLogFieldSourceType.RequestHeader);
                Transformer.Transform(app, delta);
            }
            else if (delta.Method == DeltaMethod.Remove)
            {
                var app = colleciton.First(item => item.LogFieldName == (string?)delta.Key);
                colleciton.Remove(app);
            }
            else if (delta.Method == DeltaMethod.Update)
            {
                var app = colleciton.First(item => item.LogFieldName == (string?)delta.Key);
                Transformer.Transform(app, delta);
            }
        }
    }
}
