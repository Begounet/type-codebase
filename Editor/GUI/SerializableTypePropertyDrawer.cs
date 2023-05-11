using System;
using TypeCodebase;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace TypeCodebase
{
    [CustomPropertyDrawer(typeof(SerializableType), useForChildren: true)]
    public class SerializableTypePropertyDrawer : PropertyDrawer
    {
        private TypeSelectorAdvancedDropdown.Settings _typeSettings;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeTypeSearchDropdownIFN(property);
            Rect valueRect = EditorGUI.PrefixLabel(position, label);
            Type currentType = SerializableTypeHelper.LoadType(property);

            TypeSelectorGUI.Draw(valueRect, currentType, _typeSettings, out bool hasNewTypeSelected, out Type newTypeSelected);

            CheckNewTypeSelected(property, hasNewTypeSelected, newTypeSelected);
        }

        private void CheckNewTypeSelected(SerializedProperty property, bool hasNewTypeSelected, Type newTypeSelected)
        {
            if (!hasNewTypeSelected)
            {
                return;
            }

            var typeFullNameSP = property.FindPropertyRelative(SerializableTypeHelper.SerializedTypeFullNameSPName);
            typeFullNameSP.stringValue = newTypeSelected?.AssemblyQualifiedName ?? string.Empty;
        }

        private void InitializeTypeSearchDropdownIFN(SerializedProperty property)
        {
            if (_typeSettings == null)
            {
                SerializableTypeConstraintAttribute attr = GetConstraintType();
                if (attr != null)
                {
                    _typeSettings = new TypeSelectorAdvancedDropdown.Settings()
                    {
                        ConstraintType = attr.ConstraintType,
                        UsageFlags = attr.FilterFlags
                    };
                }
                else
                {
                    _typeSettings = new TypeSelectorAdvancedDropdown.Settings()
                    {
                        UsageFlags = ETypeUsageFlag.Class
                        | ETypeUsageFlag.Abstract
                        | ETypeUsageFlag.Interface
                        | ETypeUsageFlag.Struct
                    };
                }
            }
        }

        private SerializableTypeConstraintAttribute GetConstraintType()
        {
            object[] constraints = fieldInfo.GetCustomAttributes(typeof(SerializableTypeConstraintAttribute), inherit: true);
            if (constraints.Length == 0)
            {
                return null;
            }

            return (constraints[0] as SerializableTypeConstraintAttribute);
        }
    }
}
