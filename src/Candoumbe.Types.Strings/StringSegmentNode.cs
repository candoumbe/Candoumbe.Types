using System;
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
    /// Builds a new instance of <see cref="StringSegmentNode"/> initialized with the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to store in the node.</param>
    public StringSegmentNode(ReadOnlyMemory<char> value)
    {
        Value = value;
        Next = null;
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentNode"/> initialized with the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    public StringSegmentNode(string value)
    {
        Value = value?.AsMemory() ?? ReadOnlyMemory<char>.Empty;
        Next = null;
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentNode"/> initialized with the specified <paramref name="span"/>.
    /// </summary>
    /// <param name="span"></param>
    public StringSegmentNode(ReadOnlySpan<char> span)
    {
        Value = span.ToArray();
        Next = null;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is StringSegmentNode other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public bool Equals(StringSegmentNode other) => other?.Value.Span.SequenceEqual(Value.Span) is true;
}