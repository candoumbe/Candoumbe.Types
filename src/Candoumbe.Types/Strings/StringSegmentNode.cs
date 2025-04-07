using System;
using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

/// <summary>
/// Represents a node in a <see cref="StringSegmentLinkedList"/>
/// </summary>
internal class StringSegmentNode : IEquatable<StringSegmentNode>
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
        Value = value.ToArray();
        Next = null;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is StringSegmentNode other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Value, Next);

    /// <inheritdoc />
    public virtual bool Equals(StringSegmentNode other) => other?.Value.Span.SequenceEqual(Value.Span) is true
                                                           && Equals(Next, other.Next);
}
