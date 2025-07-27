using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Fluent;

namespace Candoumbe.Types.Strings.UnitTests;

public class StringSegmentLinkedListSpecification : Machine<StringSegmentLinkedList, StringSegmentLinkedListState>
{
    private static readonly Faker _faker = new();

    private class StringSegmentLinkedListSetup : Setup<StringSegmentLinkedList, StringSegmentLinkedListState>
    {
        private readonly ReadOnlyMemory<char> _value;
        private readonly IReadOnlyList<string> _additionalValues;

        internal StringSegmentLinkedListSetup(string value, params IReadOnlyList<string> additionalValues)
        {
            _value = value.ToArray();
            _additionalValues = additionalValues;
        }

        /// <inheritdoc />
        public override StringSegmentLinkedList Actual() => new(_value.Span, [.. _additionalValues ]);

        /// <inheritdoc />
        public override StringSegmentLinkedListState Model() => new ([ _value.ToString(), ..  _additionalValues ], string.Concat([ _value, ..  _additionalValues ]));
    }

    /// <inheritdoc />
    public override Gen<Operation<StringSegmentLinkedList, StringSegmentLinkedListState>> Next(StringSegmentLinkedListState state)
    {
        List<Gen<Operation<StringSegmentLinkedList, StringSegmentLinkedListState>>> operations =
        [
            Gen.Constant<Operation<StringSegmentLinkedList, StringSegmentLinkedListState>>(
                new ReplaceCharWithChar(_faker.PickRandom(state.Value.ToCharArray()), _faker.Random.Char())),

            Gen.Constant<Operation<StringSegmentLinkedList, StringSegmentLinkedListState>>(
                new ReplaceStringWithString($"{_faker.PickRandom(state.Value.ToCharArray())}",
                    $"{_faker.Random.Word()}"))

        ];

        return Gen.OneOf(operations);
    }

    /// <inheritdoc />
    public override Arbitrary<Setup<StringSegmentLinkedList, StringSegmentLinkedListState>> Setup
    {
        get
        {
            return ArbMap.Default
                .ArbFor<NonEmptyString>().Generator
                .Zip(ArbMap.Default.ArbFor<NonNull<List<string>>>().Generator)
                .Select(tuple => (head: tuple.Item1.Item, additionalValues: tuple.Item2.Item ))
                .Select(Setup<StringSegmentLinkedList, StringSegmentLinkedListState> (tuple) => new StringSegmentLinkedListSetup(tuple.head, tuple.additionalValues))
                .ToArbitrary();
        }
    }
}