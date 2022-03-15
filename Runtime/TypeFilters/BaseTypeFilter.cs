using System;
using System.Collections.Generic;

namespace TypeCodebase
{
    public abstract class BaseTypeFilter : ITypeFilter
    {
        protected abstract int FilterId { get; }
        private int? _cachedHashCode = null;

        public abstract IEnumerable<Type> Filter(IEnumerable<Type> types);
        protected abstract int BuildHashCode();

        public override int GetHashCode()
        {
            if (!_cachedHashCode.HasValue)
            {
                _cachedHashCode = BuildHashCode();
            }
            return _cachedHashCode.Value;
        }
    }
}
