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
                var protocol = (string?)delta.ValueProperties.FirstOrDefault(vp => vp.Key == "Protocol").Value.newValue;
                Binding binding;
                if (protocol == "http")
                {
                    binding = colleciton.Add((string?)delta.Key, "http");
                }
                else
                {
                    var thumbprint = (byte[]?)delta.ValueProperties.FirstOrDefault(vp => vp.Key == "CertificateHash").Value.newValue;
                    var storeName = (string?)delta.ValueProperties.FirstOrDefault(vp => vp.Key == "CertificateStoreName").Value.newValue;
                    binding = colleciton.Add((string?)delta.Key, thumbprint, storeName);
                }

                if (binding != null)
                    Transformer.Transform(binding, delta);
            }
            else if (delta.Method == DeltaMethod.Remove)
            {
                var binding = colleciton.First(item => item.BindingInformation == (string?)delta.Key);
                colleciton.Remove(binding);
            }
            else if (delta.Method == DeltaMethod.Update)
            {
                var binding = colleciton.First(item => item.BindingInformation == (string?)delta.Key);
                Transformer.Transform(binding, delta);
            }
        }
    }
}
