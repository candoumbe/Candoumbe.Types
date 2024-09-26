using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

public record StringSegmentNode
{
    /// <summary>
    /// Value of the current node
    /// </summary>
    public StringSegment Value { get; }
    
    /// <summary>
    /// Pointer to the next node (if any).
    /// </summary>
    public StringSegmentNode Next { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the StringSegmentNode class with the specified value.
    /// </summary>
    /// <param name="value">The StringSegment value to be stored in the node.</param>
    public StringSegmentNode(StringSegment value)
    {
        Value = value;
        Next = null;
    }
}
