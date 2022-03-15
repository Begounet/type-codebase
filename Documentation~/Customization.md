# Customization

This document aims to explain how you can create additional type queries and filters.

## Implementing new Type Query

- Inherit from `BaseTypeCodebaseQuery`
- Implement `CacheResults` where you execute the operation. It will be called only when necessary.
- Set a [unique id](UniqueIds.md) for `QueryTypeId`.
- Override `BuildHashCode` and generate a new Hash Code by combining the query type id and parameters of your query to ensure its uniqueness. It is really important because it is what defines if the query has already been executed and results can get from the cache.

## Implementing new Type Filter

- Inherit from `ITypeFilter`
- Implement `Filter` and return a filter for types passed as parameters.
- Override `BuildHashCode` and generate a new Hash Code by combining a filter id and parameters of your query to ensure its uniqueness. It is really important because it is what defines if the query has already been executed and results can get from the cache.