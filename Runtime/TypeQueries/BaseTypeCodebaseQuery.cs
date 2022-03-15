using System;
using UnityEngine;

namespace TypeCodebase
{
    public abstract class BaseTypeCodebaseQuery : ITypeCodebaseQuery
    {
        protected abstract int QueryTypeId { get; }
        private Type[] _cachedTypes;
        private int? _cachedHashCode = null;

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

        public override int GetHashCode()
        {
            if (!_cachedHashCode.HasValue)
            {
                _cachedHashCode = BuildHashCode();
            }
            return _cachedHashCode.Value;
        }

        protected abstract Type[] CacheResults();
        protected abstract int BuildHashCode();

    }
}
