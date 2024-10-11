#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

public static class StringSegmentLinkedListBuilder
{
    public static StringSegmentLinkedList Create(ReadOnlySpan<StringSegment> segments)
        => segments.Length == 0 
            ? new StringSegmentLinkedList() 
            : new StringSegmentLinkedList(segments[0], [.. segments[1..]] );
}
#endif