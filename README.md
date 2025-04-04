#  Types <!-- omit in toc -->

[![GitHub Workflow Status (develop)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/integration.yml/badge.svg?branch=develop)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/integration.yml)
[![GitHub Workflow Status (nightly)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/nightly.yml/badge.svg?branch=develop)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/nightly.yml)
[![GitHub Workflow Status (main)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/delivery.yml/badge.svg?branch=main)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/delivery.yml)
[![codecov](https://codecov.io/gh/candoumbe/candoumbe.types/branch/develop/graph/badge.svg?token=FHSC41A4X3)](https://codecov.io/gh/candoumbe/candoumbe.types)
[![GitHub raw issues](https://img.shields.io/github/issues-raw/candoumbe/candoumbe.types)](https://github.com/candoumbe/candoumbe.types/issues)
[![Nuget](https://img.shields.io/nuget/vpre/candoumbe.types)](https://nuget.org/packages/candoumbe.types)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fcandoumbe%2FCandoumbe.Types%2Fdevelop)](https://dashboard.stryker-mutator.io/reports/github.com/candoumbe/Candoumbe.Types/develop)

Various custom types that can be useful when doing development.

## **Disclaimer**
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

Major version zero (0.y.z) is for initial development. **Anything MAY change at any time**.

The public API SHOULD NOT be considered stable.


## How it all started

This project was born out of frustration encountered while developing a matching algorithm in a professional context.
The goal of that project was to enable the effective matching of one or more temporary workers based on :
 - the assignment dates
 - the declared availability of collaborators.
 - their qualification ...

With the team back then, it took several weeks to get this "planning matcher" to work properly and,
at the time, I was not very happy with the result (specifically with the planning matching part of the algorithm).

That's how I started working on types from [Candoumbe.Types.Calendar](./src/Candoumbe.Types/Calendar) namespace.

## Features
### Calendar

The `Calendar` namespace contains various types related to calendar operations, such as:
- `DateTimeRange`: a datetime range represents a interval between two `DateTime`s
- `DateOnlyRange`: an interval between two `DateOnly`s
- `TimeOnlyRange`: an interval between two `TimeOnly`s

### Numerics
The numerics namespace contains various types useful when you want to work with very specific numeric values such as:
- `NonNegativeInteger`: a type that can only hold non-negative integers
- `PositiveInteger`: a type that can only hold integers values greater than zero.


## Contributing

Contributions are welcome! Feel free to submit issues or pull requests to improve the project.
License

This project is licensed under the MIT License.
See the [LICENSE](./LICENSE) file for more details.