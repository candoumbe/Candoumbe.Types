// "Copyright (c) Cyrille NDOUMBE.
// Licenced under GNU General Public Licence, version 3.0"

using FsCheck;
using FsCheck.Fluent;

using System;

namespace Candoumbe.Types.Numerics.UnitTests.Generators;

/// <summary>
/// Utility class for generating custom <see cref="Arbitrary{T}"/>
/// </summary>
internal static class ValueGenerators
{
    public static Arbitrary<Array> Arrays() => ArbMap.Default.ArbFor<int>().Generator.ArrayOf()
                                                  .Select(Array (numbers) => numbers)
                                                  .ToArbitrary();

    public static Arbitrary<PositiveInteger> PositiveIntegers() => ArbMap.Default.ArbFor<PositiveInt>()
                                                                         .Generator
                                                                         .Select(value => PositiveInteger.From(value.Item))
        .ToArbitrary();

    public static Arbitrary<NonNegativeInteger> NonNegativeIntegers() => ArbMap.Default.ArbFor<NonNegativeInt>()
                                                                         .Generator
                                                                         .Select(value => NonNegativeInteger.From(value.Item))
        .ToArbitrary();
}
