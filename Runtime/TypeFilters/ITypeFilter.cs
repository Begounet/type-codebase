using System;
using System.Collections.Generic;

namespace TypeCodebase
{
    public interface ITypeFilter
    {
        IEnumerable<Type> Filter(IEnumerable<Type> types);
    }
}
