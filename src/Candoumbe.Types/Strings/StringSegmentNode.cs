using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

public record StringSegmentNode
{
    public StringSegment Value { get; }
    
    public StringSegmentNode? Next { get; set; }

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
