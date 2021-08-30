using SaburaIIS.Agent.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SaburaIIS.Agent
{

    public interface IDelta
    {
        object? Key { get; }
        DeltaMethod Method { get; }
        IDictionary<string, (object? newValue, object? oldValue)> ValueProperties { get; }
        IDictionary<string, IDelta> NestProperties { get; }
        IDictionary<string, IEnumerable<IDelta>> NestCollectionProperties { get; }
        bool HasDiff { get; }
    }
    public enum DeltaMethod
    {
        Add,
        Remove,
        Update
    }

    public class Delta : IDelta
    {
        public object? Key { get; set; }

        public DeltaMethod Method { get; set; }

        public IDictionary<string, (object? newValue, object? oldValue)> ValueProperties { get; } = new Dictionary<string, (object?, object?)>();

        public IDictionary<string, IDelta> NestProperties { get; } = new Dictionary<string, IDelta>();

        public IDictionary<string, IEnumerable<IDelta>> NestCollectionProperties { get; } = new Dictionary<string, IEnumerable<IDelta>>();

        public bool HasDiff => 
            ValueProperties.Any() || 
            NestProperties.Any(nest => nest.Value.HasDiff) || 
            NestCollectionProperties.Any(item => item.Value.Any(nest => nest.HasDiff));

        private static readonly object[] _comparers = new object[]
        {
            new ApplicationComparer(),
            new ApplicationPoolComparer(),
            new BindingComparer(),
            new CustomLogFieldComparer(),
            new ScheduleComparer(),
            new SiteComparer(),
            new VirtualDirectoryComparer(),
            new WorkerProcessComparer(),
        };

        private static IEqualityComparer<T> GetComparer<T>()
        {
            foreach (var it in _comparers)
                if (it is IEqualityComparer<T> comparer)
                    return comparer;

            throw new NotSupportedException(typeof(T).FullName);
        }

        private static readonly IDictionary<Type, Func<object, object>> _keySelectors =
            _comparers.ToDictionary(
                comparer =>comparer.GetType().GetInterface("IEqualityComparer`1")!.GenericTypeArguments.First(), 
                comparer => ((IKeySelector)comparer).KeySelector);

        private static Func<T, object>? GetKeySelector<T>()
        {
            var type = typeof(T);
            if (_keySelectors.ContainsKey(type) && _keySelectors[type] is Func<T, object> selector)
                return selector;

            return null;
        }
        public static IDelta Create<T>(T local, T remote)
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var keySekector = GetKeySelector<T>();


            var delta = new Delta
            {
                Method = local == null && remote != null ? DeltaMethod.Add
                        : local != null && remote == null ? DeltaMethod.Remove
                        : DeltaMethod.Update,
                Key = keySekector?.Invoke(local ?? remote)
            };

            foreach (var prop in properties)
            {
                var propType = prop.PropertyType;
                var localValue = local != null ? prop.GetValue(local) : null;
                var remoteValue = remote != null ? prop.GetValue(remote) : null;

                if (propType.IsValueType || propType.IsEnum || propType.FullName == "System.String")
                {
                    if ((localValue != null || remoteValue != null) && (localValue?.Equals(remoteValue) ?? remote?.Equals(localValue)) == false)
                        delta.ValueProperties.Add(prop.Name, (remoteValue, localValue));
                }
                else if (propType.IsClass)
                {
                    if (localValue != null || remoteValue != null)
                    {
                        var nest = CreateDeltaMethod.MakeGenericMethod(propType).Invoke(null, new[] { localValue, remoteValue }) as IDelta;
                        if (nest is not null)
                            delta.NestProperties.Add(prop.Name, nest);
                    }
                }
                else if (propType.FullName?.StartsWith("System.Collections.Generic.ICollection`1") == true)
                {
                    if (localValue != null || remoteValue != null)
                    {
                        var nest = CreateDeltaCollectionMethod.MakeGenericMethod(propType.GenericTypeArguments).Invoke(null, new[] { localValue, remoteValue }) as IEnumerable<IDelta>;
                        if (nest is not null)
                            delta.NestCollectionProperties.Add(prop.Name, nest);
                    }
                }
            }

            return delta;
        }

        public static IEnumerable<IDelta> CreateCollection<T>(IEnumerable<T> locals, IEnumerable<T> remotes)
        {
            var comparer = GetComparer<T>();
            var keySekector = GetKeySelector<T>();

            if (keySekector == null)
                throw new NotSupportedException();

            locals = locals ?? new List<T>();
            remotes = remotes ?? new List<T>();
            var additions = remotes.Except(locals, comparer).ToList();
            var removes = locals.Except(remotes, comparer).ToList();
            var updations = locals.Join(remotes, keySekector, keySekector, (local, remote) => (local, remote)).ToList();

            var type = typeof(T);
            foreach (var (local, remote) in updations)
            {
                var delta = CreateDeltaMethod.MakeGenericMethod(type).Invoke(null, new object?[] { local, remote }) as IDelta;

                if (delta is not null && delta.HasDiff)
                    yield return delta;
            }

            foreach (var addition in additions)
            {
                var delta = CreateDeltaMethod.MakeGenericMethod(type).Invoke(null, new object?[] { null, addition }) as IDelta;

                if (delta is not null && delta.HasDiff)
                    yield return delta;
            }

            foreach (var remove in removes)
            {
                var delta = CreateDeltaMethod.MakeGenericMethod(type).Invoke(null, new object?[] { remove, null }) as IDelta;

                if (delta is not null && delta.HasDiff)
                    yield return delta;
            }
        }

        private readonly static MethodInfo CreateDeltaMethod = typeof(Delta).GetMethod(nameof(Create))!;
        private readonly static MethodInfo CreateDeltaCollectionMethod = typeof(Delta).GetMethod(nameof(CreateCollection))!;
    }
}