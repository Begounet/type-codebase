using System;
using UnityEngine;

namespace TypeCodebase
{
    public abstract class BaseTypeCodebaseQuery : ITypeCodebaseQuery
    {
        protected abstract int QueryTypeId { get; }
        private Type[] _cachedTypes;

        public Type[] GetResults()
        {
            if (_cachedTypes == null)
            {
                _cachedTypes = CacheResults();
            }
            return _cachedTypes;
        }

        public bool Equals(ITypeCodebaseQuery other)
            => GetHashCode() == other.GetHashCode();

        protected abstract Type[] CacheResults();
    }
}
