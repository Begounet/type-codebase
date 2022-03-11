using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeCodebase
{
    /// <summary>
    /// Only the types that match the <see cref="ETypeUsageFlag"/> will pass.
    /// </summary>
    public class TypeUsageFilter : ITypeFilter
    {
        private ETypeUsageFlag _usage;

        public TypeUsageFilter(ETypeUsageFlag usage)
        {
            _usage = usage;
        }

        public IEnumerable<Type> Filter(IEnumerable<Type> types)
            => types.Where((t)
                => (!t.IsAbstract || _usage.HasFlag(ETypeUsageFlag.Abstract))
                && (!t.IsInterface || _usage.HasFlag(ETypeUsageFlag.Interface))
                && (!t.IsClass || _usage.HasFlag(ETypeUsageFlag.Class))
                && (!t.IsValueType || _usage.HasFlag(ETypeUsageFlag.Struct))
                && (!t.IsGenericType || _usage.HasFlag(ETypeUsageFlag.Generic))
                && (!_usage.HasFlag(ETypeUsageFlag.ForbidUnityObject) || !typeof(UnityEngine.Object).IsAssignableFrom(t))
            );

        public override int GetHashCode()
        {
            const int filterId = 0;
            return HashCode.Combine(filterId, _usage);
        }
    }
}
