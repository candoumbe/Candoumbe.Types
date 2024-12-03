# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

### 🚨 Breaking changes
- Dropped `net6.0` support as it's no longer maintained by Microsoft ([#216](https://github.com/candoumbe/Candoumbe.Types/issues/216)).
- Dropped `net7.0` support as it's no longer maintained by Microsoft.
- Removed `Ranges` property from [`MultiDateTimeRange`](./src/Candoumbe.Types/Calendar/MultiDateTimeRange.cs), [`MultiTimeOnlyRange`](./src/Candoumbe.Types/Calendar/MultiTimeOnlyRange.cs) and [`MultiDateOnlyRange`](./src/Candoumbe.Types/Calendar/MultiDateOnlyRange.cs)
which is now redundant

### 🧹 Housekeeping
- Fixed incorrect package and repository urls which caused report of mutation tests to not be sent.
- Bumped `Candoumbe.Pipelines` dependency to `0.11.0`
- Added benchmarks for `StringSegmentLinkedList` type
- Added `DotNet.ReproducibleBuilds` NuGet dependency when running on CI server

## [0.1.0] / 2023-01-29
- Initial release

[Unreleased]: https://github.com/candoumbe/Candoumbe.Types/compare/0.2.0...HEAD
[0.2.0]: https://github.com/candoumbe/Candoumbe.Types/compare/0.1.0...0.2.0
[0.1.0]: https://github.com/candoumbe/Candoumbe.Types/tree/0.1.0
