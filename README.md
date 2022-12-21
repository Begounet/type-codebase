# Type Codebase

This package aims to provide an unified way to treat heavy queries on Assemblies and Types.

## Why?

Fetching assemblies and types on heavy project can quickly become really heavy and make your soft/tools slow. By coupling a query and cache system, you can execute the same query multiple times at different place during your development without caring your own cache system.

## Usage

### Assembly Codebase

- Ensure to cache all assemblies in an unique type array so there is no more allocations during `AppDomain.CurrentDomain.GetAssemblies()` calls.
- Cache assembly full name
- Cache assembly short name version (generally really slow) (`Assembly.GetName().Name`)

### Type Query

Use `TypeQueryManager.Query(myQuery)` to execute types query. All queries are cached. Queries comparisons are based on what they do. So if you make a new instance of query doing the exact same thing of another query, you will get the cache result.

Queries comparison are based on the Hash Code, that are generated according to the parameter of the query.

#### "Type Query" types

- `GetAllTypesFromAssemblyQuery`: Query all types from an assembly
- `GetAllTypesFromAllAssembliesQuery`: Query all types from all assemblies
- `FilterQuery`: Apply a combination of filters ("Filter Query") on a query source to get only what you want. You can see it as a `System.Linq.Where` function.

#### "Filter Query" types

- `IsAssignableFromFilter`: Only types that inherit from the type used in the constructor will pass.

- `TypeUsageFilter`: Only the types that match the `ETypeUsageFlag` will pass. Supports:

  - Class

  - Struct

  - Abstract

  - Interface

  - Generic

  - ForbidUnityObject (So type that inherit from UnityObject cannot be selected)

  - MustBeUnityObject (So only type that inherit from UnityObject can be selected)

    

  ##### Examples

  - Return all types from an assembly:

    `TypeQueryManager.Query(new GetAllTypesFromAssemblyQuery(assembly))`

  - Return all types from an assembly that are class or structs and are assignable from `MyStruct`:
  
    ```csharp
    TypeQueryManager.Query(
        new FilterQuery(new GetAllTypesFromAssemblyQuery(assembly),
                        new IsAssignableFromFilter(typeof(MyClass)),
                        new TypeUsageFilter(ETypeUsageFlag.Class | ETypeUsageFlag.Struct))
    );
    ```

---

## TypeSelectorGUI

It is a button for EditorGUI to select a type, according to a list of constraint. Types queries are based on the type codebase and generation is async (but not the search system!).

![](Documentation~/Resources/TypeSelectorGUI.gif)

## Usage

```csharp
TypeSelectorGUI.Draw(position, property, new TypeSelectorAdvancedDropdown.Settings()
{
    ConstraintType = TypeSelectorGUI.TryLoadTypeFromManagedReference(property),
    UsageFlags = ETypeUsageFlag.Class | ETypeUsageFlag.ForbidUnityObject
})
```

`ConstraintType` is the base class of the types that can be selected. If your property is a `SerializeReference` with a base class type, you can use `TypeSelectorGUI.TryLoadTypeFromManagedReference(property)` to assign this type to the constraint type.

For advanced purposes, you can also use directly `TypeSelectorAdvancedDropdown` to use the dropdown anywhere you want. Subscribe to `OnTypeSelected` to get the result but be careful that this is not called during the same frame that the draw! So you may have to defer the treatment you will do with the result.

---

## SerializableType

You can use `SerializableType` to define a type that will be serialized (you still have to add the classes to `link.xml` on AOT platforms).

```csharp
[SerializedField]
private SerializableType _type;
```

You can also add the attribute `SerializableTypeConstraint` to constraint the type selection.

```csharp
[SerializedField, SerializableTypeConstraint(typeof(IMyInterfaceType))]
private SerializableType _type;
```

*Only types inheriting from `IMyInterfaceType` will be selectable.*

