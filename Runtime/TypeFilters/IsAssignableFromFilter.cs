using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeCodebase
{
    /// <summary>
    /// Only types that inherit from the type used in the constructor will pass.
    /// TypesSource.Where((t) => baseType == null || baseType.IsAssignableFrom(t));
    /// </summary>
    public class IsAssignableFromFilter : BaseTypeFilter
    {
        protected override int FilterId => 1;
        private Type _baseType;

        public IsAssignableFromFilter(Type baseType)
        {
            _baseType = baseType;
        }

        public override IEnumerable<Type> Filter(IEnumerable<Type> types)
            => types.Where((t) => _baseType == null || _baseType.IsAssignableFrom(t));

        protected override int BuildHashCode()
            => HashCode.Combine(FilterId, _baseType);
    }
}
