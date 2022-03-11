using System;
using UnityEngine;

namespace TypeCodebase
{
    public interface ITypeCodebaseQuery : IEquatable<ITypeCodebaseQuery> 
    {
        public Type[] GetResults();
    }

}
