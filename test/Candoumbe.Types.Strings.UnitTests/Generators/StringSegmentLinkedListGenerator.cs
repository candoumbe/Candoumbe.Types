using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;

namespace Candoumbe.Types.Strings.UnitTests.Generators
{
    internal static class StringSegmentLinkedListGenerator
    {
        public static Arbitrary<StringSegmentLinkedList> GenerateStringSegmentLinkedLists()
            => ArbMap.Default.ArbFor<NonEmptyString>()
                .Generator
                .Select(value => (value: value.Item, chunkSize: Random.Shared.Next(1, value.Item.Length)))
                .Select(tuple =>
                        {
                            IReadOnlyList<char[]> chunks = [.. tuple.value.Chunk(tuple.chunkSize)];

                            StringSegmentLinkedList list = [];

                            return chunks.Aggregate(list, (current, chunk) => current.Append(chunk));
                        })
                .ToArbitrary();
    }
}