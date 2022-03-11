using System;
using UnityEngine;

namespace TypeCodebase
{
    [Flags]
    public enum ETypeUsageFlag
    {
        Class = 1 << 1,
        Struct = 1 << 2,
        Abstract = 1 << 3,
        Interface = 1 << 4,
        Generic = 1 << 5,
        ForbidUnityObject = 1 << 6,
    }
}