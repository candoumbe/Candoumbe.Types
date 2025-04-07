using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Candoumbe.Types.Strings;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Fluent;
using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.UnitTests.Strings;

public class StringSegmentLinkedListSpecification : Machine<StringSegmentLinkedList, StringSegmentLinkedListState>
{
    private static readonly Faker _faker = new();

    private class StringSegmentLinkedListSetup : Setup<StringSegmentLinkedList, StringSegmentLinkedListState>
    {
        private readonly ReadOnlyMemory<char> _value;
        private readonly IReadOnlyList<string> _additionalValues;

        internal StringSegmentLinkedListSetup(ReadOnlySpan<char> value, params IReadOnlyList<string> additionalValues)
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
        IList<Gen<Operation<StringSegmentLinkedList, StringSegmentLinkedListState>>> operations = [];

        if (state.Value is not { Length: 0 })
        {
            operations.Add(
                Gen.Constant<Operation<StringSegmentLinkedList, StringSegmentLinkedListState>>(
                    new ReplaceCharWithChar(_faker.PickRandom(state.Value.ToCharArray()), _faker.Random.Char())));

            operations.Add(
                Gen.Constant<Operation<StringSegmentLinkedList, StringSegmentLinkedListState>>(
                    new ReplaceStringWithString($"{_faker.PickRandom(state.Value.ToCharArray())}", $"{_faker.Random.Word()}")));
        }

        return Gen.OneOf(operations);
    }

    /// <inheritdoc />
    public override Arbitrary<Setup<StringSegmentLinkedList, StringSegmentLinkedListState>> Setup
    {
        get
        {
            int count = _faker.Random.Int(min: 0, max: 2);

            return Gen.Constant<Setup<StringSegmentLinkedList, StringSegmentLinkedListState>>(new StringSegmentLinkedListSetup(_faker.Random.Word(), _faker.Random.WordsArray(count)))
                .ToArbitrary();
        }
    }
}