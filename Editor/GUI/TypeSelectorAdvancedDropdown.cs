using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace TypeCodebase
{
    public class TypeSelectorAdvancedDropdown : AdvancedDropdown
    {
        public class Settings
        {
            public Type ConstraintType { get; set; }
            public ETypeUsageFlag UsageFlags { get; set; }
        }

        private const int AllocatedTimeForOperationsPerFrame = 1000;
        private static FieldInfo _getWindowInstanceFI = null;

        private Settings _customSettings = null;
        private EditorCoroutine _currentBuildRootCoroutine = null;
        private Dictionary<Assembly, ITypeCodebaseQuery> _queries;

        public event Action<Type> OnTypeSelected;
        private EditorWindow _currentWindow;

        public TypeSelectorAdvancedDropdown(AdvancedDropdownState state, Settings customSettings = null)
            : base(state)
        {
            minimumSize = new Vector2(200, 300);
            _customSettings = customSettings;
            _queries = new Dictionary<Assembly, ITypeCodebaseQuery>();
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Type");

            if (_currentBuildRootCoroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(_currentBuildRootCoroutine);
            }

            _currentBuildRootCoroutine = EditorCoroutineUtility.StartCoroutine(BuildRootAsync(root), this);
            return root;
        }

        private IEnumerator BuildRootAsync(AdvancedDropdownItem root)
        {
            _queries.Clear();
            _currentWindow = GetCurrentWindow();

            TimeoutYielder yielder = new TimeoutYielder(AllocatedTimeForOperationsPerFrame);

            foreach (var assemblyRef in AssemblyCodebase.Assemblies)
            {
                BuildTypesForAssembly(root, assemblyRef, yielder);
                if (yielder.ShouldYield())
                {
                    _currentWindow.Repaint();
                    yield return null;
                }
            }

            root.AddSeparator();
            root.AddChild(new AdvancedDropdownItem("Undefined") { id = 0 });

            _currentWindow.Repaint();

            _currentBuildRootCoroutine = null;
        }

        private void BuildTypesForAssembly(AdvancedDropdownItem root, Assembly assembly, TimeoutYielder yielder)
        {
            AssemblyCodebase.Handle assemblyHandle = AssemblyCodebase.GetAssemblyHandle(assembly);
            var assemblyNode = new AdvancedDropdownItem(assemblyHandle.ShortName) { id = assembly.GetHashCode() };

            ITypeCodebaseQuery query;
            if (_customSettings != null)
            {
                query = GetConstraintTypesEnumeratorFromAssembly(assemblyHandle.Assembly, _customSettings);
            }
            else
            {
                query = new GetAllTypesFromAssemblyQuery(assembly);
            }

            foreach (var type in TypeQueryManager.Query(query))
            {
                assemblyNode.AddChild(new AdvancedDropdownItem(type.Name) { id = type.GetHashCode() });
            }

            // Check if there is at least one child
            if (assemblyNode.children.Any())
            {
                root.AddChild(assemblyNode);
                _queries.Add(assembly, query);
            }
        }

        private ITypeCodebaseQuery GetConstraintTypesEnumeratorFromAssembly(Assembly assembly, Settings settings = null)
        {
            if (settings == null)
            {
                return new GetAllTypesFromAssemblyQuery(assembly);
            }

            return new FilterQuery(new GetAllTypesFromAssemblyQuery(assembly),
                new IsAssignableFromFilter(settings.ConstraintType),
                new TypeUsageFilter(settings.UsageFlags));
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            Type selectedType = FindItemById(item.id);
            OnTypeSelected.Invoke(selectedType);
        }

        private Type FindItemById(int id)
        {
            if (id == 0)
            {
                return null;
            }

            foreach (var kv in _queries)
            {
                foreach (var type in TypeQueryManager.Query(kv.Value))
                {
                    if (type.GetHashCode() == id)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        private EditorWindow GetCurrentWindow()
        {
            if (_getWindowInstanceFI == null)
            {
                _getWindowInstanceFI = typeof(TypeSelectorAdvancedDropdown).GetField("m_WindowInstance", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            return (EditorWindow)_getWindowInstanceFI.GetValue(this);
        }

        public void ShowAsDropdown()
        {
            if (Event.current != null)
            {
                Show(new Rect(Event.current.mousePosition, Vector2.zero));
            }
        }
    }
}
