using Microsoft.Web.Administration;
using System;
using System.Linq;

namespace SaburaIIS.Agent.Transformers
{
    public class ScheduleCollectionTransformer : ITransformer
    {
        public void Transform(object obj, IDelta delta)
        {
            var colleciton = (ScheduleCollection)obj;

            if (delta.Method == DeltaMethod.Add)
            {
                colleciton.Add((TimeSpan)delta.Key!);
            }
            else if (delta.Method == DeltaMethod.Remove)
            {
                var schedule = colleciton.First(item => item.Time == (TimeSpan)delta.Key!);
                colleciton.Remove(schedule);
            }
        }
    }
}
