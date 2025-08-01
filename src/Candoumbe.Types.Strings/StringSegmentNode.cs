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
    public StringSegmentNode Next
    {
        get
        {
            StringSegmentNode next = null;
            if (_next is null)
            {
                _next = new WeakReference<StringSegmentNode>(null);
            }
            else
            {
                _next.TryGetTarget(out next);
            }
            return next;
        }
        internal set
        {
            if (_next is null)
            {
                _next = new WeakReference<StringSegmentNode>(value);
            }
            else
            {
                _next.SetTarget(value);
            }
        }
    }

    private WeakReference<StringSegmentNode> _next;

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
        _next = null;
    }

    /// <summary>
    /// Checks if the current node is the last of the string.
    /// </summary>
    /// <returns><see langword="true"/> if the current node is the last of the string; otherwise, <see langword="false"/>.</returns>
    public bool HasNext() => _next is not null;

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is StringSegmentNode other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public bool Equals(StringSegmentNode other) => other?.Value.Span.SequenceEqual(Value.Span) is true;
}