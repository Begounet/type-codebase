using System;
using System.Reflection;

namespace TypeCodebase
{
    public static class AssemblyCodebase
    {
        public struct Handle
        {
            internal int Index;

            public ref readonly Assembly Assembly => ref _assemblies[Index];
            public ref readonly string ShortName => ref GetShortName(this);
            public ref readonly string FullName => ref GetFullName(this);
        }

        private static Assembly[] _assemblies;
        private static Handle[] _handles;
        private static string[] _shortNames;
        private static string[] _fullNames;


        public static Assembly[] Assemblies
        {
            get
            {
                if (_assemblies == null)
                {
                    CacheAssemblies();
                }
                return _assemblies;
            }
        }

        public static ref readonly Handle GetAssemblyHandle(Assembly assembly)
        {
            for (int i = 0; i < _assemblies.Length;++i)
            {
                if (_assemblies[i] == assembly)
                {
                    return ref _handles[i];
                }
            }
            throw new ArgumentException($"Assembly {assembly} is not registered.");
        }

        public static ref readonly string GetShortName(Handle handle)
        {
            if (_shortNames[handle.Index] == null)
            { 
                _shortNames[handle.Index] = handle.Assembly.GetName().Name;
            }
            return ref _shortNames[handle.Index];
        }

        public static ref readonly string GetFullName(Handle handle)
        {
            if (_fullNames[handle.Index] == null)
            {
                _fullNames[handle.Index] = _assemblies[handle.Index].FullName;
            }
            return ref _fullNames[handle.Index];
        }

        private static void CacheAssemblies()
        {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies();

            int assemblyCount = _assemblies.Length;
            _handles = new Handle[assemblyCount];
            _shortNames = new string[assemblyCount];
            _fullNames = new string[assemblyCount];

            for (int i = 0; i < _handles.Length; ++i)
            {
                _handles[i].Index = i;
            }
        }        
    }
}
