#  Datafilters <!-- omit in toc -->

[![GitHub Workflow Status (main)](https://img.shields.io/github/workflow/status/candoumbe/types/delivery/main?label=main)](https://github.com/candoumbe/types/actions/workflows/delivery.yml)
[![GitHub Workflow Status (develop)](https://img.shields.io/github/workflow/status/candoumbe/types/integration/develop?label=develop)](https://github.com/candoumbe/types/actions/workflows/delivery.yml)
[![codecov](https://codecov.io/gh/candoumbe/DataFilters/branch/develop/graph/badge.svg?token=FHSC41A4X3)](https://codecov.io/gh/candoumbe/types)
[![GitHub raw issues](https://img.shields.io/github/issues-raw/candoumbe/types)](https://github.com/candoumbe/types/issues)
[![Nuget](https://img.shields.io/nuget/vpre/candoumbe.types)](https://nuget.org/packages/candoumbe.types)

Custom types that can be usefull when doing development.

## **Disclaimer**
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

Major version zero (0.y.z) is for initial development. **Anything MAY change at any time**.

The public API SHOULD NOT be considered stable. 



| Package                                                                                                                                                             | Description                                                                                                                                                                         |
| ------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [![Nuget](https://img.shields.io/nuget/v/Datafilters?label=Datafilters&color=blue)](https://www.nuget.org/packages/DataFilters)                                     | provides core functionalities of parsing strings and converting to [IFilter][class-ifilter] instances.                                                                              |
| [![Nuget](https://img.shields.io/nuget/v/DataFilters.Expressions?label=Datafilters.Expressions&color=blue)](https://www.nuget.org/packages/DataFilters.Expressions) | adds `ToExpression<T>()` extension method on top of [IFilter][class-ifilter] instance to convert it to an equivalent `System.Linq.Expressions.Expression<Func<T, bool>>` instance.  |
| [![Nuget](https://img.shields.io/nuget/v/Datafilters.Queries?label=DataFilters.Queries&color=blue)](https://www.nuget.org/packages/DataFilters.Queries)             | adds `ToWhere<T>()` extension method on top of [IFilter][class-ifilter] instance to convert it to an equivalent [`IWhereClause`](https://dev.azure.com/candoumbe/Queries) instance. |


[class-multi-filter]: /src/DataFilters/MultiFilter.cs
[class-ifilter]: /src/DataFilters/IFilter.cs
[class-filter]: /src/DataFilters/Filter.cs
[datafilters-expressions]: https://www.nuget.org/packages/DataFilters.Expressions
[datafilters-queries]: https://www.nuget.org/packages/DataFilters.Queries