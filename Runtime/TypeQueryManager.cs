using System;
using System.Collections.Generic;
using System.Reflection;

namespace TypeCodebase
{
    public static class TypeQueryManager
    {
        private static HashSet<ITypeCodebaseQuery> _queries = new HashSet<ITypeCodebaseQuery>();

        public static Type[] Query(ITypeCodebaseQuery query)
        {
            if (_queries.TryGetValue(query, out ITypeCodebaseQuery cachedQuery))
            {
                return cachedQuery.GetResults();
            }
            _queries.Add(query);
            return query.GetResults();
        }

        public static Type[] QueryAllTypes(Assembly assembly)
            => Query(new GetAllTypesFromAssemblyQuery(assembly));
    }
}
