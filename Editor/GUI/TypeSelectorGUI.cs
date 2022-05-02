using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace TypeCodebase
{
    public static class TypeSelectorGUI
    {
        private static int _workControlID = -2;
        private static Type _selectedType = null;

        private static readonly char[] FullTypenameSplitChars = new char[] { ' ', '.' };

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;

        public static Rect Draw(Rect position, SerializedProperty property, TypeSelectorAdvancedDropdown.Settings options)
        {
            options.ConstraintType = TryLoadTypeFromManagedReference(property);
            Type currentType = !string.IsNullOrEmpty(property.managedReferenceFullTypename) ? TryLoadTypeFromManagedReferenceTypename(property.managedReferenceFullTypename) : null;
            position = Draw(position, currentType, options, out bool hasSelectedType, out Type selectedType);
            if (hasSelectedType)
            {
                property.managedReferenceValue = (selectedType != null ? Activator.CreateInstance(selectedType) : null);
            }
            return position;
        }

        public static Rect Draw(Rect position, Type currentType, TypeSelectorAdvancedDropdown.Settings options, out bool hasSelectedType, out Type selectedType)
        {
            int hotControl = GUIUtility.GetControlID(FocusType.Passive);
            if (hotControl == _workControlID)
            {
                _workControlID = -2;
                hasSelectedType = true;
                selectedType = _selectedType;
            }
            else
            {
                hasSelectedType = false;
                selectedType = null;
            }

            string displayName = GetShortTypename(currentType?.Name ?? options.ConstraintType?.Name ?? "<unknown>");
            if (GUI.Button(position, displayName))
            {
                var dropdown = new TypeSelectorAdvancedDropdown(new AdvancedDropdownState(), options);
                dropdown.OnTypeSelected += (t) =>
                {
                    _workControlID = hotControl;
                    _selectedType = t;
                };
                dropdown.Show(position);
            }
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return position;
        }

        public static string GetShortTypename(string fullTypename)
        {
            int idx = fullTypename.LastIndexOfAny(FullTypenameSplitChars);
            if (idx >= 0)
            {
                fullTypename = fullTypename.Substring(idx + 1);
            }
            return fullTypename;
        }
        
        public static string ConvertManagedReferenceTypenameToStandardTypename(string managedReferenceFieldTypename)
        {
            if (string.IsNullOrEmpty(managedReferenceFieldTypename))
            {
                return null;
            }

            string[] chunks = managedReferenceFieldTypename.Split(' '); // [0] = Assembly name, [1] = Full typename
            return $"{chunks[1]}, {chunks[0]}";
        }

        public static Type TryLoadTypeFromManagedReferenceTypename(string managedReferenceFieldTypename)
        {
            string typename = ConvertManagedReferenceTypenameToStandardTypename(managedReferenceFieldTypename);
            if (typename == null)
            {
                return null;
            }
            return Type.GetType(typename);
        }

        public static Type TryLoadTypeFromManagedReference(SerializedProperty property)
        {
            Assert.AreEqual(SerializedPropertyType.ManagedReference, property.propertyType);
            return TryLoadTypeFromManagedReferenceTypename(property.managedReferenceFieldTypename);
        }
    }
}