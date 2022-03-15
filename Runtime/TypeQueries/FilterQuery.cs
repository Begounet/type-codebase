using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TypeCodebase
{
    /// <summary>
    /// Apply a combination of filters ("Filter Query") on a query source to get only what you want.
    /// You can see it as a `System.Linq.Where` function.
    /// </summary>
    public class FilterQuery : BaseTypeCodebaseQuery
    {
        private ITypeCodebaseQuery _source;
        private ITypeFilter[] _filters;

        protected override int QueryTypeId => 1;

        public FilterQuery(ITypeCodebaseQuery source, ITypeFilter filter, params ITypeFilter[] filters)
        {
            _source = source;
            _filters = new ITypeFilter[1 + filters.Length];
            _filters[0] = filter;
            for (int i = 0; i < filters.Length; ++i)
            {
                _filters[1 + i] = filters[i];
            }
        }

        protected override Type[] CacheResults()
        {
            IEnumerable<Type> results = TypeQueryManager.Query(_source);
            for (int i = 0; i < _filters.Length; ++i)
            {
                results = _filters[i].Filter(results);
            }
            return results.ToArray();
        }

        protected override int BuildHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(QueryTypeId);
            hash.Add(_source.GetHashCode());
            for (int i = 0; i < _filters.Length; ++i)
            {
                hash.Add(_filters[i]);
            }
            return hash.ToHashCode();
        }
    }
}
