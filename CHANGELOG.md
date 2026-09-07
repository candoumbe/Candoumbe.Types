# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### 🧹 Housekeeping
- Removed a generated dotnet runfile-discovery cache from version control.
- Removed the tracked devcontainer environment file.
- Simplified devcontainer implementation to use official .NET 10.0 base image
- Replaced custom Dockerfile and docker-compose setup with simpler configuration
- Removed unnecessary devcontainer documentation files
- Streamlined VS Code extensions to only essential tools for .NET development
- Added post-create and post-start scripts for container initialization

## [0.5.0] / 2025-11-27
### 🚀 New features
- Added `net10.0` TFM support ([#346](https://github.com/candoumbe/candoumbe.types/issues/346))
- Added `netstandard2.0` TFM support ([#355](https://github.com/candoumbe/candoumbe.types/issues/355))
- Added `StringSegmentLinkedList.EndsWith(ReadOnlySpan<char>,IEqualityComparer<char>)` method ([#287](https://github.com/candoumbe/candoumbe.types/issues/287))
- Optimized memory usage of `StringSegmentLinkedList`
  - Added new constructors for `StringSegmentNode` and `StringSegmentLinkedList`
  - Improved `Append` and `InsertAt` methods to handle different types of inputs
  - Added `Compact` method to reduce node count and improve locality

### 🧹 Housekeeping
- Removed unnecessary imports and simplify code
- Added performance tests for `StringSegmentLinkedList`

## [0.4.2] / 2025-11-11
### 🐛 Fixes
- Add missing `Parse` and `TryParse` methods for `NonNegativeInteger` and `PositiveInteger` types.
- Add mising `PositiveInteger.ToString()` to better display positive integer values in logs.
- Updated all dependencies to their latest versions.

## [0.4.1] / 2025-08-12
### 🐛 Fixes
- Revert usage of weak references to optimize `StringSegmentLinkedList` memory consumption.

### 🧹 Housekeeping
- Fix versioning for hotfix branches.

## [0.4.0] / 2025-08-08
### 🚀 New features
- Added `StringSegmentLinkedList.StartsWith(ReadOnlySpan<char>,IEqualityComparer<char>)` method ([#287](https://github.com/candoumbe/Candoumbe.Types/issues/287))
- Added `net9.0` support

### 🧹 Housekeeping
- Optimize `StringSegmentLinkedList` memory consumption by using weak references when possible.
- updated cache key for Stryker config file.

## [0.3.1] / 2025-07-27
### 🐛 Fixes
- `StringSegmentLinkedList.GetHashCode` throws `NotSupportedException` ([#301](https://github.com/candoumbe/candoumbe.types/issues/301))

### 🧹 Housekeeping

- Reinitialized `parameters.local.json` content

## [0.3.0] / 2025-07-13
### 💥 Breaking changes

- Moved all types from `Candoumbe.Types` namespace from `Candoumbe.Types` NuGet package into `Candoumbe.Types.Core` NuGet package
- Moved all types from `Candoumbe.Types.Numerics` namespace from `Candoumbe.Types` NuGet package into `Candoumbe.Types.Numerics` NuGet package
- Moved all types from `Candoumbe.Types.Calendar` namespace from `Candoumbe.Types` NuGet package into `Candoumbe.Types.Calendar` NuGet package
- Moved all types from `Candoumbe.Types.Strings` namespace from `Candoumbe.Types` NuGet package into `Candoumbe.Types.Strings` NuGet package
- Replaced `StringSegmentLinkedList.IsEquivalentTo(StringSegmentLinkedList)` with `StringSegmentLinkedList.Equals(StringSegmentLinkedList)`

### 🚀 New features
- Added `NonNegativeLong` type ([#43](https://github.com/candoumbe/Candoumbe.Types/issues/43))
- Added `PositiveLong` type ([#43](https://github.com/candoumbe/Candoumbe.Types/issues/43))
- Added `StringSegmentLinkedList.Equals(StringSegmentLinkedList, IEqualituComparer<char>)`
- Added `StringSegmentLinkedList.Contains(ReadOnlySpan<char>)` ([#284](https://github.com/candoumbe/Candoumbe.Types/issues/284))
- Added `StringSegmentLinkedList.Contains(ReadOnlySpan<char>, IEqualityComparer<char>)` ([#284](https://github.com/candoumbe/Candoumbe.Types/issues/284))

### 🧹 Housekeeping
- Moved to central package management

## [0.2.1] / 2025-06-17
### 🚨 Breaking changes
- Renamed `StringSegmentLinkedList.Equals(StringSegmentLinkedList, IEqualityComparer<char>)` to `StringSegmentLinkedList.IsEquivalentTo(StringSegmentLinkedList, IEqualityComparer<char>)`

### 🐛 Fixes
- Fixed `ArgumentOutOfRangeException` thrown by `StringSegmentLinkedList.Replace(Func<char, bool>, IReadOnlyDictionary<char, ReadOnlyMemory<char>>)` 
when there are more than 1 match in a node.
- Fixed incorrect behavior of `StringSegmentLinkedList.IsEquivalentTo`([#283](https://github.com/candoumbe/candoumbe.types/issues/283))

### 🧹 Housekeeping
- Added documentation.
- Updated GitVersion configuration to better handle hotfix versioning

### 🧹 Housekeeping
- Move to central package management
- Add `DotNet.ReproducibleBuilds` package to `core.props`
- Update [`Candoumbe.Pipelines`](https://nuget.org/packages/pipelines) to `0.13.2`
- Update `README.md` with new badges for nightly and main branches, and mutation testing
- Improve `StringSegmentLinkedList.Replace` when replacing a `string` by a `string`  method to reduce memory allocations
    - Optimize character replacement logic
    - Avoid unnecessary allocations by using `ReadOnlyMemory<char>` and `ReadOnlySpan<char>`
    - Refactor code for better readability and performance

### 🧹 Housekeeping
- Add `DotNet.ReproducibleBuilds` package to `core.props`
- Update [`Candoumbe.Pipelines`](https://nuget.org/packages/pipelines) to `0.13.2`
- Update `README.md` with new badges for nightly and main branches, and mutation testing
- Improve `StringSegmentLinkedList.Replace` when replacing a `string` by a `string`  method to reduce memory allocations
    - Optimize character replacement logic
    - Avoid unnecessary allocations by using `ReadOnlyMemory<char>` and `ReadOnlySpan<char>`
    - Refactor code for better readability and performance

### 🛠️ Technical
- Updated GitHub workflows to download required SDKs when running CI. 

## [0.2.0] / 2024-12-03
### 🚀 New features
- Added `NonNegativeInteger` type ([#43](https://github.com/candoumbe/Candoumbe.Types/issues/43))
- Added `PositiveInteger` type ([#43](https://github.com/candoumbe/Candoumbe.Types/issues/43))
- Added `net8.0` support
- Added [`StringSegmentLinkedList`](./src/Candoumbe.Types/Strings/StringSegmentLinkedList.cs) type
- Added collection expression support for `MultiTimeOnlyRange`, `MultiDateTimeRange` and `MultiDateOnlyRange` ([#205](https://github.com/candoumbe/candoumbe.types/issues/205))
- Added [`IRange`](./src/Candoumbe.Types/IRange.cs) interface
- Added [ICanRepresentEmpty](./src/Candoumbe.Types/ICanRepresentEmpty.cs) interface
- Added [ICanRepresentInfinite](./src/Candoumbe.Types/ICanRepresentInfinite.cs) interface

### 💥 Breaking changes
- Dropped `net6.0` support as it's no longer maintained by Microsoft ([#216](https://github.com/candoumbe/Candoumbe.Types/issues/216)).
- Dropped `net7.0` support as it's no longer maintained by Microsoft.
- Removed `Ranges` property from [`MultiDateTimeRange`](./src/Candoumbe.Types/Calendar/MultiDateTimeRange.cs), [`MultiTimeOnlyRange`](./src/Candoumbe.Types/Calendar/MultiTimeOnlyRange.cs) and [`MultiDateOnlyRange`](./src/Candoumbe.Types/Calendar/MultiDateOnlyRange.cs)
which is now redundant
- Added `NonNegativeLong` type ([#70](https://github.com/candoumbe/Candoumbe.Types/issues/70))
- Added `PositiveLong` type ([#70](https://github.com/candoumbe/Candoumbe.Types/issues/70))

### 🛠️ Technical
- Code refactoring to be able to add more numeric types ([#70](https://github.com/candoumbe/Candoumbe.Types/issues/70))

### 🧹 Housekeeping
- Fixed incorrect package and repository urls which caused report of mutation tests to not be sent.
- Bumped `Candoumbe.Pipelines` dependency to `0.11.0`
- Added benchmarks for `StringSegmentLinkedList` type
- Added `DotNet.ReproducibleBuilds` NuGet dependency when running on CI server

## [0.1.0] / 2023-01-29
- Initial release

[Unreleased]: https://github.com/candoumbe/Candoumbe.Types/compare/0.5.0...HEAD
[0.5.0]: https://github.com/candoumbe/Candoumbe.Types/compare/0.4.2...0.5.0
[0.4.2]: https://github.com/candoumbe/Candoumbe.Types/compare/0.4.1...0.4.2
[0.4.1]: https://github.com/candoumbe/Candoumbe.Types/compare/0.4.0...0.4.1
[0.4.0]: https://github.com/candoumbe/Candoumbe.Types/compare/0.3.1...0.4.0
[0.3.1]: https://github.com/candoumbe/Candoumbe.Types/compare/0.3.0...0.3.1
[0.3.0]: https://github.com/candoumbe/Candoumbe.Types/compare/0.2.1...0.3.0
[0.2.1]: https://github.com/candoumbe/Candoumbe.Types/compare/0.2.0...0.2.1
[0.2.0]: https://github.com/candoumbe/Candoumbe.Types/compare/0.1.0...0.2.0
[0.1.0]: https://github.com/candoumbe/Candoumbe.Types/tree/0.1.0