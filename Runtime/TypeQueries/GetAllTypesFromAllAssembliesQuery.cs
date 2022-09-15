using System;
using System.Collections.Generic;
using UnityEngine;

namespace TypeCodebase
{
    public class GetAllTypesFromAllAssembliesQuery : BaseTypeCodebaseQuery
    {
        protected override int QueryTypeId => 2;

        protected override Type[] CacheResults()
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in AssemblyCodebase.Assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    types.Add(type);
                }
            }
            return types.ToArray();
        }

        protected override int BuildHashCode()
            => HashCode.Combine(QueryTypeId);
    }
}
