#  Types <!-- omit in toc -->

[![GitHub Workflow Status (develop)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/integration.yml/badge.svg?branch=develop)](https://github.com/candoumbe/Candoumbe.Types/actions/workflows/integration.yml)
[![codecov](https://codecov.io/gh/candoumbe/candoumbe.types/branch/develop/graph/badge.svg?token=FHSC41A4X3)](https://codecov.io/gh/candoumbe/candoumbe.types)
[![GitHub raw issues](https://img.shields.io/github/issues-raw/candoumbe/candoumbe.types)](https://github.com/candoumbe/candoumbe.types/issues)
[![Nuget](https://img.shields.io/nuget/vpre/candoumbe.types)](https://nuget.org/packages/candoumbe.types)

Custom types that can be usefull when doing development.

## **Disclaimer**
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

Major version zero (0.y.z) is for initial development. **Anything MAY change at any time**.

The public API SHOULD NOT be considered stable.

## Ranges

`Candoumbe.Types.Ranges` namespace contains types describe intervals

## Numerics

`Candoumbe.Types.Numerics` namespace contains various numerics types

- [`PositiveInteger`](class-types-numerics-positive-int) : a numeric type that can only contains 
strictly positive integers. The `PositiveInteger.Value` is garantied to be between `1` and `int.MaxValue`


[class-types-numerics-positive-int]:./src/Candoumbe.Types/Numerics/PositiveInteger.cs