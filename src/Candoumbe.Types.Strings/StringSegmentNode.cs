using System;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif
using System.Diagnostics.CodeAnalysis;

namespace Candoumbe.Types.Strings;

/// <summary>
/// Represents a node in a <see cref="StringSegmentLinkedList"/>
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class StringSegmentNode : IEquatable<StringSegmentNode>
{
    /// <summary>
    /// Value of the current node
    /// </summary>
    public ReadOnlyMemory<char> Value { get; }

    /// <summary>
    /// Pointer to the next node (if any).
    /// </summary>
    public StringSegmentNode Next { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the StringSegmentNode class with the specified value.
    /// </summary>
    /// <param name="value">The StringSegment value to be stored in the node.</param>
    public StringSegmentNode(ReadOnlySpan<char> value)
    {
#if NET8_0_OR_GREATER
        Value = value.ToImmutableArray().AsMemory();
#else
        Value = value.ToArray();
#endif
        Next = null;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is StringSegmentNode other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public bool Equals(StringSegmentNode other) => other?.Value.Span.SequenceEqual(Value.Span) is true;
}