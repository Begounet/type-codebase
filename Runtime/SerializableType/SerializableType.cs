using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TypeCodebase
{
    [Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string _typeFullname;

        private Type _cachedType = null;
        public Type Type
        {
            get
            {
                if (_cachedType == null)
                {
                    TryLoadSerializedType();
                }
                return _cachedType;
            }
            set
            {
                _typeFullname = value?.AssemblyQualifiedName ?? null;
                _cachedType = value;
            }
        }
        public bool IsValidType => (Type != null);

        public SerializableType() { }
        public SerializableType(Type type) => Type = type;


        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            TryLoadSerializedType();
        }

        private void TryLoadSerializedType()
        {
            if (!string.IsNullOrWhiteSpace(_typeFullname))
            {
                _cachedType = Type.GetType(_typeFullname);
            }
        }
    }
}