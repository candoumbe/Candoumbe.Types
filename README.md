#  Types <!-- omit in toc -->

[![GitHub Workflow Status (develop)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/integration.yml/badge.svg?branch=develop)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/integration.yml)
[![codecov](https://codecov.io/gh/candoumbe/candoumbe.types/branch/develop/graph/badge.svg?token=FHSC41A4X3)](https://codecov.io/gh/candoumbe/candoumbe.types)
[![GitHub raw issues](https://img.shields.io/github/issues-raw/candoumbe/candoumbe.types)](https://github.com/candoumbe/candoumbe.types/issues)
[![Nuget](https://img.shields.io/nuget/vpre/candoumbe.types)](https://nuget.org/packages/candoumbe.types)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2FCandoumbe.Types)](https://dashboard.stryker-mutator.io/reports/Candoumbe.Types)

Various custom types that can be usefull when doing development.

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

That's how I started working on [Candoumbe.Types.Calendar](./src/Candoumbe.Types/Calendar) types namespace.



## Contributing

Contributions are welcome! Feel free to submit issues or pull requests to improve the project.
License

This project is licensed under the MIT License.
See the [LICENSE](./LICENSE) file for more details.