using System;
using System.Collections.Generic;
using System.Reflection;

namespace TypeCodebase
{
    /// <summary>
    /// Query all types from an assembly
    /// </summary>
    public class GetAllTypesFromAssemblyQuery : BaseTypeCodebaseQuery
    {
        protected override int QueryTypeId => 0;

        private Assembly _assembly;

        public GetAllTypesFromAssemblyQuery(Assembly assembly)
            => _assembly = assembly;

        protected override Type[] CacheResults()
            => _assembly.GetTypes();

        protected override int BuildHashCode()
            => HashCode.Combine(QueryTypeId, _assembly);
    }
}
