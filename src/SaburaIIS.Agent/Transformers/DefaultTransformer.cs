using System.Linq;

namespace SaburaIIS.Agent.Transformers
{
    public class DefaultTransformer : ITransformer
    {
        protected virtual string[] IgnoreProps { get; } = new string[] { };
        public virtual void Transform(object obj, IDelta delta)
        {
            var type = obj.GetType();

            foreach (var vp in delta.ValueProperties)
            {
                var prop = type.GetProperty(vp.Key);
                if (prop != null && (prop.SetMethod != null || prop.CanWrite) && !IgnoreProps.Contains(prop.Name))
                    prop.SetValue(obj, vp.Value.newValue);
            }

            foreach (var np in delta.NestProperties)
            {
                var prop = type.GetProperty(np.Key);
                if (prop != null)
                {
                    var nest = prop.GetValue(obj);
                    if (nest != null)
                        Transformer.Transform(nest, np.Value);
                }

            }

            foreach (var cp in delta.NestCollectionProperties)
            {
                var prop = type.GetProperty(cp.Key);
                if (prop != null)
                {
                    var nest = prop.GetValue(obj);
                    if (nest != null)
                        foreach (var d in cp.Value)
                            Transformer.Transform(nest, d);
                }
            }
        }
    }
}
