using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Candoumbe.MiscUtilities.Comparers;
using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

/// <summary>
/// Represents a linked list data structure for managing string nodes.
/// </summary>
/// <remarks>
/// This implementation is specifically designed to not allow appending empty string values.
/// <para>The implementation provides four properties that can be used to fine-tune the performance of the linked list:</para>
/// <list type="bullet">
///   <item>
///     <term><see cref="ChunkSize"/></term>
///     <description>The size of the chunks used to compact the list.</description>
///   </item>
///   <item>
///     <term><see cref="MaxSmallSegmentLength"/></term>
///     <description>The length threshold under which a segment is considered small.</description>
///   </item>
///   <item>
///     <term><see cref="MaxNodeCountBeforeCompact"/></term>
///     <description>The maximum number of nodes to keep before compacting the list.</description>
///   </item>
///   <item>
///     <term><see cref="ReplaceCloneThreshold"/></term>
///     <description>The number of characters beyond which replace operations uses a clone/copy strategy.</description>
///   </item>
/// </list>
/// </remarks>
public sealed class StringSegmentLinkedList : IEnumerable<ReadOnlyMemory<char>>, IEquatable<StringSegmentLinkedList>
{
    private StringSegmentNode _head;
    private StringSegmentNode _tail;

    /// <summary>
    /// Size of the chunks used to compact the list.
    /// </summary>
    /// <remarks>
    /// This value is used to determine the size of the buffer used to <see cref="Compact">compact</see> the list.
    /// </remarks>
    public int ChunkSize { get; set; } = 4_096;

    /// <summary>
    /// Length of the smallest segment that will be kept in the list.
    /// </summary>
    public int MaxSmallSegmentLength { get; set; } = 64;

    /// <summary>
    /// Number of nodes that will be kept in the list before compacting it.
    /// </summary>
    public int MaxNodeCountBeforeCompact { get; set; } = 10_000;

    /// <summary>
    /// Number of characters threshold beyond which a "replace" operation adopts a different strategy.
    /// For example, cloning/copying a block instead of replacing character by character.
    /// </summary>
    public int ReplaceCloneThreshold { get; set; } = 1_000_000;


    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/> that is empty.
    /// </summary>
    public StringSegmentLinkedList()
    {
        _head = null;
        _tail = null;
        Count = 0;
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">The value of the head</param>
    public StringSegmentLinkedList(ReadOnlySpan<char> head)
    {
        if (head.IsEmpty)
        {
            _head = _tail = null;
            Count = 0;
        }
        else
        {
            _head = _tail = new StringSegmentNode(head);
            Count = 1;
        }
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">The value of the head</param>
    public StringSegmentLinkedList(string head)
    {
        if (string.IsNullOrEmpty(head))
        {
            _head = _tail = null;
            Count = 0;
        }
        else
        {
            _head = _tail = new StringSegmentNode(head);
            Count = 1;
        }
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">The value of the head</param>
    public StringSegmentLinkedList(ReadOnlyMemory<char> head)
    {
        if (head.IsEmpty)
        {
            _head = _tail = null;
            Count = 0;
        }
        else
        {
            _head = _tail = new StringSegmentNode(head);
            Count = 1;
        }
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">The head StringSegment</param>
    public StringSegmentLinkedList(StringSegment head)
    {
        if (!head.HasValue || head.Length == 0)
        {
            _head = _tail = null;
            Count = 0;
        }
        else
        {
            ReadOnlyMemory<char> mem = head.Buffer?.AsMemory(head.Offset, head.Length) ?? ReadOnlyMemory<char>.Empty;
            _head = _tail = new StringSegmentNode(mem);
            Count = 1;
        }
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">the value of the head</param>
    /// <param name="others">Additional elements to append to the list.</param>
    public StringSegmentLinkedList(string head, params string[] others)
        : this(head)
    {
        if (others is { Length: > 0 })
        {
            foreach (string s in others)
            {
                Append(s);
            }
        }
    }

    /// <summary>
    /// Appends <paramref name="value"/> to the end of the list.
    /// </summary>
    /// <param name="value">The value to append</param>
    /// <returns>The current instance with <paramref name="value"/> at the tail.</returns>
    /// <remarks>
    /// The current instance remains untouched if <paramref name="value"/> is empty.
    /// <para>
    /// Note: this method will increase the output of <see cref="Count"/> by 1 when <paramref name="value"/> is not empty.
    /// </para>
    /// </remarks>
    public StringSegmentLinkedList Append(ReadOnlySpan<char> value)
    {
        if (!value.IsEmpty)
        {
            StringSegmentNode newNode = new(value);
            AppendInternal(newNode);
        }

        return this;
    }

    /// <summary>
    /// Appends <paramref name="value"/> to the end of the list.
    /// </summary>
    /// <param name="value">The value to append</param>
    /// <returns>The current instance with <paramref name="value"/> at the tail.</returns>
    /// <remarks>
    /// The current instance remains untouched if <paramref name="value"/> is empty.
    /// <para>
    /// Note: this method will increase the output of <see cref="Count"/> by 1 when <paramref name="value"/> is not empty.
    /// </para>
    /// </remarks>
    public StringSegmentLinkedList Append(StringSegment value)
    {
        if (value is { HasValue: true, Length: > 0 })
        {
            ReadOnlyMemory<char> mem = value.Buffer?.AsMemory(value.Offset, value.Length) ?? ReadOnlyMemory<char>.Empty;
            if (!mem.IsEmpty)
            {
                AppendInternal(new StringSegmentNode(mem));
            }
        }

        return this;
    }

    /// <summary>
    /// Appends <paramref name="value"/> to the end of the list.
    /// </summary>
    /// <param name="value">The value to append</param>
    /// <returns>The current instance with <paramref name="value"/> at the tail.</returns>
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    public StringSegmentLinkedList Append(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            AppendInternal(new StringSegmentNode(value));
        }

        return this;
    }

    /// <summary>
    /// Appends <paramref name="value"/> to the end of the list.
    /// </summary>
    /// <param name="value">The value to append</param>
    /// <returns>The current instance with <paramref name="value"/> at the tail.</returns>
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    public StringSegmentLinkedList Append(ReadOnlyMemory<char> value)
    {
        if (!value.IsEmpty)
        {
            AppendInternal(new StringSegmentNode(value));
        }

        return this;
    }

    /// <summary>
    /// Appends <paramref name="value"/> to the end of the list.
    /// </summary>
    /// <param name="value">The value to append</param>
    /// <returns>The current instance with <paramref name="value"/> at the tail.</returns>
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    public StringSegmentLinkedList Append(char[] value) => value is { Length: > 0 }
                                                               ? Append((ReadOnlyMemory<char>)value)
                                                               : this;

    private void AppendInternal(StringSegmentNode newNode)
    {
        if (_head is null)
        {
            _head = _tail = newNode;
        }
        else
        {
            _tail.Next = newNode;
            _tail = newNode;
        }

        Count++;
    }

    /// <summary>
    /// Inserts a new link containing <paramref name="value"/> at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">0-based index where <paramref name="value"/> will be inserted</param>
    /// <param name="value">The value of the node to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is &lt; 0. or &gt; <see cref="Count"/>.</exception>
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    public void InsertAt(int index, ReadOnlySpan<char> value)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        if (!value.IsEmpty)
        {
            StringSegmentNode newNode = new(value);
            InsertAtInternal(index, newNode);
        }
    }

    /// <summary>
    /// Inserts a new link containing <paramref name="value"/> at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">0-based index where <paramref name="value"/> will be inserted</param>
    /// <param name="value">The value of the node to insert.</param>
    /// <remarks>
    /// The current instance remains untouched if <paramref name="value"/> is empty.
    /// <para>
    /// Note: this method will increase the output of <see cref="Count"/> by 1 when <paramref name="value"/> is not empty.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is &lt; 0 or &gt; <see cref="Count"/>.</exception>
    public void InsertAt(int index, StringSegment value)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        if (value is { HasValue: true, Length: > 0 })
        {
            ReadOnlyMemory<char> mem = value.Buffer?.AsMemory(value.Offset, value.Length) ?? ReadOnlyMemory<char>.Empty;
            if (!mem.IsEmpty)
            {
                InsertAtInternal(index, new StringSegmentNode(mem));
            }
        }
    }

    /// <summary>
    /// Inserts a new node containing <paramref name="value"/> at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">0-based index where <paramref name="value"/> will be inserted</param>
    /// <param name="value">The value of the node to insert.</param>
    /// <remarks>
    /// The current instance remains untouched if <paramref name="value"/> is empty.
    /// <para>
    /// Note: this method will increase the output of <see cref="Count"/> by 1 when <paramref name="value"/> is not empty.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is &lt; 0 or &gt; <see cref="Count"/>.</exception>
    public void InsertAt(int index, string value)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        if (!string.IsNullOrEmpty(value))
        {
            InsertAtInternal(index, new StringSegmentNode(value));
        }
    }

    /// <summary>
    /// Inserts a new link containing <paramref name="value"/> at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">0-based index where <paramref name="value"/> will be inserted</param>
    /// <param name="value">The value of the node to insert.</param>
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is &lt; 0 or &gt; <see cref="Count"/>.</exception>
    public void InsertAt(int index, ReadOnlyMemory<char> value)
    {
        if (index is < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        if (!value.IsEmpty)
        {
            InsertAtInternal(index, new StringSegmentNode(value));
        }
    }

    private void InsertAtInternal(int index, StringSegmentNode newNode)
    {
        if (index == 0)
        {
            newNode.Next = _head;
            _head = newNode;
            if (_tail is null)
            {
                _tail = newNode;
            }
        }
        else if (index == Count)
        {
            // insert at end
            if (_tail is null)
            {
                _head = _tail = newNode;
            }
            else
            {
                _tail.Next = newNode;
                _tail = newNode;
            }
        }
        else
        {
            StringSegmentNode previous = _head;
            for (int i = 1; i < index; i++)
            {
                previous = previous!.Next;
            }

            newNode.Next = previous!.Next;
            previous.Next = newNode;
        }

        Count++;
    }

    /// <summary>
    /// Inserts <paramref name="other"/> at the given <paramref name="index"/>
    /// </summary>
    /// <param name="index">0-based index where the new node will be inserted</param>
    /// <param name="other">The value of the node to inserts</param>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is &lt; 0 or &gt; <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentNullException">if <paramref name="other"/> is <see langword="null"/> .</exception>
    public StringSegmentLinkedList InsertAt(int index, StringSegmentLinkedList other)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        switch (index)
        {
            case > 0:
                InsertAtInternal(index, other._head);
                break;
            default:
                other.AppendInternal(_head);
                _head = other._head;
                break;
        }

        return this;
    }

    /// <summary>
    /// Gets the number of nodes.
    /// </summary>
    /// <remarks>
    /// The value obtained must be discarded after any call to :
    /// <list type="bullet">
    ///
    /// </list>
    /// <see cref="InsertAt(int, string)"/>, <see cref="InsertAt(int, string)"/>
    /// </remarks>
    public int Count { get; private set; }

    /// <summary>
    /// Compacts the list into larger chunks to reduce node count and improve locality.
    /// </summary>
    /// <remarks>
    /// Calling this method will have no impact on the value returned by <see cref="GetTotalLength"/>.
    /// But it will, in certain cases, reduce the number of nodes in the list, which, in turn, could make subsequent operations faster by reducing the <see cref="Count">internal node count</see>.
    /// </remarks>
    public StringSegmentLinkedList Compact()
    {
        if (_head is null)
        {
            return this;
        }

        ArrayPool<char> pool = ArrayPool<char>.Shared;
        char[] buffer = pool.Rent(ChunkSize);
        try
        {
            int pos = 0;
            StringSegmentLinkedList compacted = new();

            StringSegmentNode node = _head;
            while (node is not null)
            {
                ReadOnlySpan<char> span = node.Value.Span;
                int remaining = span.Length;
                int offset = 0;

                while (remaining > 0)
                {
                    int toCopy = Math.Min(remaining, ChunkSize - pos);
                    span.Slice(offset, toCopy).CopyTo(buffer.AsSpan(pos));
                    pos += toCopy;
                    offset += toCopy;
                    remaining -= toCopy;
                    if (pos == ChunkSize)
                    {
                        Flush();
                    }
                }

                node = node.Next;
            }

            Flush();

            _head = compacted._head;
            _tail = compacted._tail;
            Count = compacted.Count;
            return this;

            void Flush()
            {
                if (pos is not 0)
                {
                    string chunk = new string(buffer, 0, pos);
                    compacted.Append(chunk);
                    pos = 0;
                }
            }
        }
        finally
        {
            pool.Return(buffer);
        }
    }

    /// <summary>
    /// Computes the total length of the resulting string value
    /// </summary>
    /// <returns></returns>
    public int GetTotalLength()
    {
        int totalLength = 0;
        StringSegmentNode current = _head;
        while (current is not null)
        {
            totalLength += current.Value.Length;
            current = current.Next;
        }

        return totalLength;
    }

    /// <summary>
    /// Computes the <see cref="string"/> value.
    /// </summary>
    /// <returns>The computed <see cref="string"/>.</returns>
    public string ToStringValue()
    {
        int total = GetTotalLength();
        string result = string.Empty;

        if (total is not 0)
        {
#if NETSTANDARD2_0
            System.Text.StringBuilder sb = new (total);
            StringSegmentNode current = _head;
            do
            {
                ReadOnlySpan<char> span = current.Value.Span;
                sb.Append(span.ToString());
                current = current.Next;
            } while (current is not null);

#else
            result = string.Create(total, _head, static (dest, head) =>
            {
                int offset = 0;
                StringSegmentNode current = head;
                while (current is not null)
                {
                    ReadOnlySpan<char> span = current.Value.Span;
                    span.CopyTo(dest[offset..]);
                    offset += span.Length;
                    current = current.Next;
                }
            });
#endif
        }

        return result;
    }

    /// <inheritdoc />
    public IEnumerator<ReadOnlyMemory<char>> GetEnumerator()
    {
        StringSegmentNode current = _head;
        while (current is not null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Replaces one character <paramref name="oldChar"/> by <paramref name="newChar"/>.
    /// </summary>
    /// <param name="oldChar"><see langword="character"/> to replace.</param>
    /// <param name="newChar"><see langword="character"/> that will replace <paramref name="oldChar"/>.</param>
    /// <returns>The current list where all characters were replaced.</returns>
    /// <remarks>
    /// The returned <see cref="StringSegmentLinkedList"/>'s <see cref="Count">node count</see> may differ from the original instance's.
    /// </remarks>
    public StringSegmentLinkedList Replace(char oldChar, char newChar) => Replace(oldChar, [newChar]);

    /// <summary>
    /// Replaces one character <paramref name="oldChar"/> by <paramref name="newChar"/>.
    /// </summary>
    /// <param name="oldChar"><see langword="character"/> to replace.</param>
    /// <param name="newChar"><see langword="character"/> that will replace <paramref name="oldChar"/>.</param>
    /// <returns>The current list where all characters were replaced.</returns>
    /// <remarks>
    /// The returned <see cref="StringSegmentLinkedList"/>'s <see cref="Count">node count</see> may differ from the original instance's.
    /// </remarks>
    public StringSegmentLinkedList Replace(Func<char, bool> oldChar, char newChar) => Replace(oldChar, [newChar]);

    /// <summary>
    /// Replaces all <paramref name="oldChar"/> by <paramref name="replacement"/>.
    /// </summary>
    /// <param name="oldChar"><see langword="character"/> to replace.</param>
    /// <param name="replacement"><see langword="string"/> that will replace <paramref name="oldChar"/>.</param>
    /// <returns>The current list where all characters were replaced.</returns>
    /// <remarks>
    /// The returned <see cref="StringSegmentLinkedList"/>'s <see cref="Count">node count</see> may differ from the original instance's.
    /// </remarks>
    public StringSegmentLinkedList Replace(char oldChar, ReadOnlySpan<char> replacement)
        => Replace(chr => chr == oldChar, replacement);

    /// <summary>
    /// Replaces all <see langword="char"/>s that matches <paramref name="predicate"/>
    /// with the specified <paramref name="replacement"/>.
    /// </summary>
    /// <param name="predicate">The predicate used to test which <see langword="char"/> to replace</param>
    /// <param name="replacement">The value that will be used to replace <see langword="char"/>s that matches <paramref name="predicate"/>.</param>
    /// <returns>
    /// The <see cref="StringSegmentLinkedList"/> where each <see langword="char"/> that matches <paramref name="predicate"/>
    /// has been replaced with <paramref name="replacement"/>.
    /// </returns>
    /// <remarks>
    /// The returned <see cref="StringSegmentLinkedList"/>'s <see cref="Count">node count</see> may differ from the original instance's.
    /// </remarks>
    public StringSegmentLinkedList Replace(Func<char, bool> predicate, ReadOnlySpan<char> replacement)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        StringSegmentLinkedList replacementList = new();
        StringSegmentNode current = _head;
        bool anyReplacement = false;

        while (current is not null)
        {
            int indexOfOldChar = current.Value.FirstOccurrence(predicate);
            IEnumerable<int> occurrences = current.Value.Occurrences(predicate);

            if (indexOfOldChar >= 0)
            {
                anyReplacement = true;

                if (indexOfOldChar > 0)
                {
                    replacementList = replacementList.Append(current.Value[..indexOfOldChar]);
                }

                replacementList = replacementList.Append(replacement);

                int index = indexOfOldChar + 1;
                foreach (int occurrence in occurrences.Skip(1))
                {
                    if (index < occurrence)
                    {
                        replacementList = replacementList.Append(current.Value.Slice(index, occurrence - index));
                    }

                    replacementList = replacementList.Append(replacement);
                    index = occurrence + 1;
                }

                if (index < current.Value.Length)
                {
                    replacementList = replacementList.Append(current.Value[index..]);
                }
            }
            else
            {
                replacementList = replacementList.Append(current.Value);
            }

            current = current.Next;
        }

        return anyReplacement ? replacementList : this;
    }

    /// <summary>
    /// Replaces all <see langword="char"/>s that matches <paramref name="predicate"/>
    /// with a value from <paramref name="replacements"/>.
    /// </summary>
    /// <param name="predicate">The predicate used to test which <see langword="char"/> to replace</param>
    /// <param name="replacements">The value that will be used to replace <see langword="char"/>s that matches <paramref name="predicate"/>.</param>
    /// <returns>
    /// The <see cref="StringSegmentLinkedList"/> where each <see langword="char"/> that matches <paramref name="predicate"/>
    /// has been replaced with a replacement value from <paramref name="replacements"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var originalList = new StringSegmentLinkedList(/* initialization */);
    /// var replacements = new Dictionary&lt;char, ReadOnlyMemory&lt;char&gt;&gt;
    /// {
    ///     { 'a', "alpha".AsMemory() },
    ///     { 'b', "beta".AsMemory() }
    /// };
    /// var list = linkedList.Replace(c => c == 'a' || c == 'b', replacements);
    /// // `list` will have 'a' replaced with "alpha" and 'b' with "beta"
    /// </code>
    /// </example>
    public StringSegmentLinkedList Replace(Func<char, bool> predicate, IReadOnlyDictionary<char, ReadOnlyMemory<char>> replacements)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (replacements is null)
        {
            throw new ArgumentNullException(nameof(replacements));
        }

        StringSegmentLinkedList replacementList = new();
        StringSegmentNode current = _head;
        bool anyReplacement = false;

        while (current is not null)
        {
            int indexOfOldChar = current.Value.FirstOccurrence(predicate);
            IEnumerable<int> occurrences = current.Value.Occurrences(predicate);

            if (indexOfOldChar >= 0)
            {
                anyReplacement = true;

                if (!replacements.TryGetValue(current.Value.Span[indexOfOldChar], out ReadOnlyMemory<char> replacement))
                {
                    replacement = ReadOnlyMemory<char>.Empty;
                }

                if (indexOfOldChar > 0)
                {
                    replacementList = replacementList.Append(current.Value[..indexOfOldChar]);
                }

                replacementList = replacementList.Append(replacement);

                int index = indexOfOldChar;
                foreach (int occurrence in occurrences.Skip(1))
                {
                    if (!replacements.TryGetValue(current.Value.Span[occurrence], out replacement))
                    {
                        replacement = ReadOnlyMemory<char>.Empty;
                    }

                    if (index < occurrence)
                    {
                        ReadOnlyMemory<char> valueToKeep = current.Value[(index + 1) .. occurrence];
                        replacementList = replacementList.Append(valueToKeep)
                            .Append(replacement);
                    }
                    else
                    {
                        replacementList = replacementList.Append(replacement);
                    }

                    index = occurrence;
                }

                if (index < current.Value.Length)
                {
                    replacementList = replacementList.Append(current.Value[(index + 1)..]);
                }
            }
            else
            {
                replacementList = replacementList.Append(current.Value);
            }

            current = current.Next;
        }

        return anyReplacement ? replacementList : this;
    }

    /// <summary>
    /// Replaces all <paramref name="oldValue"/> by <paramref name="newValue"/>.
    /// </summary>
    /// <param name="oldValue"><see langword="string"/> to replace.</param>
    /// <param name="newValue"><see langword="string"/> that will replace <paramref name="oldValue"/>.</param>
    /// <returns>The current list where all characters were replaced.</returns>
    /// <remarks>
    /// This method does its best to never allocate.
    /// Also, beware that the returned <see cref="StringSegmentLinkedList"/> may have more <see cref="StringSegmentNode">nodes</see> than
    /// the initial instance had.
    /// </remarks>
    public StringSegmentLinkedList Replace(in ReadOnlySpan<char> oldValue, in ReadOnlySpan<char> newValue)
    {
        StringSegmentLinkedList replacementList = new();
        StringSegmentNode current = _head;
        int oldStringLength = oldValue.Length;
        bool anyReplacement = false;

        while (current is not null)
        {
            ReadOnlySpan<char> subject = current.Value.Span;
            int index = subject.IndexOf(oldValue.Length > 0 ? oldValue[0] : '\0');

            if (oldStringLength == 0)
            {
                // Nothing to replace
                replacementList = replacementList.Append(current.Value);
            }
            else if (index is -1)
            {
                replacementList = replacementList.Append(current.Value);
            }
            else
            {
                int previousIndex = 0;
                do
                {
                    if (index > 0)
                    {
                        replacementList = replacementList.Append(subject[..index]);
                    }

                    int offset = 0;
                    bool mismatchFound;
                    int startIndex = index;
                    do
                    {
                        mismatchFound = subject[index] != oldValue[offset];
                        index++;
                        offset++;
                    } while (offset < oldStringLength && index < subject.Length && !mismatchFound);

                    if (mismatchFound)
                    {
                        // rewind and copy from previousIndex+1 to index
                        replacementList = replacementList.Append(subject[(previousIndex + 1)..index]);
                    }
                    else
                    {
                        replacementList = replacementList.Append(newValue);
                        anyReplacement = true;
                    }

                    subject = subject[index..];
                    previousIndex = startIndex;
                    index = subject.IndexOf(oldValue[0]);
                } while (index != -1 && subject.Length > 0);

                if (!subject.IsEmpty)
                {
                    replacementList = replacementList.Append(subject);
                }
            }

            current = current.Next;
        }

        if (anyReplacement)
        {
            _head = replacementList._head;
            _tail = replacementList._tail;
            Count = replacementList.Count;
        }

        return anyReplacement
                   ? replacementList
                   : this; // no-op when none replaced
    }

    /// <summary>
    /// Remove nodes by a predicate
    /// </summary>
    /// <param name="predicate">filter</param>
    /// <returns>A new <see cref="StringSegmentLinkedList"/> where all candidates <paramref name="predicate"/> were removed.</returns>
    public StringSegmentLinkedList RemoveBy(Func<ReadOnlyMemory<char>, bool> predicate)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        StringSegmentNode current = _head;
        StringSegmentNode previous = null;
        int newCount = 0;

        while (current is not null)
        {
            if (predicate(current.Value))
            {
                // remove current
                if (previous is not null)
                {
                    previous.Next = current.Next;
                }
                else
                {
                    _head = current.Next;
                }

                if (current.Next is null)
                {
                    _tail = previous;
                }
            }
            else
            {
                // keep current
                previous = current;
                newCount++;
            }

            current = current.Next;
        }

        Count = newCount;
        if (Count is 0)
        {
            _head = _tail = null;
        }

        return this;
    }

    /// <summary>
    /// Builds a new <see cref="StringSegmentLinkedList"/> which is
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">if <paramref name="other"/> is <see langword="null"/></exception>
    public StringSegmentLinkedList Append(StringSegmentLinkedList other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        StringSegmentLinkedList result = new();
        StringSegmentNode current = _head;
        while (current is not null)
        {
            result = result.Append(current.Value);
            current = current.Next;
        }

        current = other._head;
        while (current is not null)
        {
            result = result.Append(current.Value);
            current = current.Next;
        }

        return result;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is StringSegmentLinkedList other && Equals(other, CharComparer.InvariantCultureIgnoreCase);

    /// <inheritdoc />
#if NETSTANDARD2_0
    public override int GetHashCode()
    {
        int hashCode = 17;

        using IEnumerator<ReadOnlyMemory<char>> enumerator = GetEnumerator();

        while (enumerator.MoveNext())
        {
            hashCode = hashCode * 31 + enumerator.Current.GetHashCode();
        }

        return hashCode;
    }
#else
    public override int GetHashCode()
    {
        HashCode hashCode = new();

        using IEnumerator<ReadOnlyMemory<char>> enumerator = GetEnumerator();

        while (enumerator.MoveNext())
        {
            hashCode.Add(enumerator.Current.GetHashCode());
        }

        return hashCode.ToHashCode();
    }
#endif

    /// <summary>
    /// Determines whether the current <see cref="StringSegmentLinkedList"/> is equal to another <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="other">The <see cref="StringSegmentLinkedList"/> to compare with the current instance.</param>
    /// <param name="comparer">An optional <see cref="IEqualityComparer{T}"/> to compare elements. If null, the default equality comparer is used.</param>
    /// <returns>
    /// <c>true</c> if the current <see cref="StringSegmentLinkedList"/> is equal to the <paramref name="other"/> instance; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method compares the elements of the two linked lists. If the lengths of the lists are different,
    /// it checks if one list starts with the other. The comparison can be customized using the provided <paramref name="comparer"/>.
    /// </remarks>
    public bool Equals(StringSegmentLinkedList other, IEqualityComparer<char> comparer)
    {
        bool equals = false;

        if (other is not null)
        {
            using IEnumerator<ReadOnlyMemory<char>> currentEnumerator = GetEnumerator();
            using IEnumerator<ReadOnlyMemory<char>> otherEnumerator = other.GetEnumerator();

            bool currentHasNext = currentEnumerator.MoveNext();
            bool otherHasNext = otherEnumerator.MoveNext();

            if (!currentHasNext && !otherHasNext)
            {
                equals = true;
            }
            else
            {
                bool mismatchFound = false;
                Func<char, char, bool> predicateComparer = comparer switch
                {
                    null => (x, y) => Equals(x, y),
                    _    => comparer.Equals,
                };

                bool hasReachedCurrentEnd = false;
                bool hasReachedOtherEnd = false;

                do
                {
                    ReadOnlyMemory<char> current = currentEnumerator.Current;
                    ReadOnlyMemory<char> otherCurrent = otherEnumerator.Current;
                    if (current.Length == otherCurrent.Length)
                    {
                        mismatchFound = !(current.StartsWith(otherCurrent, comparer) || otherCurrent.StartsWith(current, comparer));
                    }
                    else if (current.Length < otherCurrent.Length && otherCurrent.StartsWith(current, comparer))
                    {
                        if (!currentEnumerator.MoveNext())
                        {
                            mismatchFound = true;
                        }
                        else
                        {
                            int index = current.Length;
                            current = currentEnumerator.Current;
                            int j = 0;
                            do
                            {
                                while (j < current.Length && index < otherCurrent.Length && !mismatchFound)
                                {
                                    char currentChar = current.Span[j];
                                    char otherChar = otherCurrent.Span[index];
                                    mismatchFound = !predicateComparer(currentChar, otherChar);
                                    j++;
                                    index++;

                                    // No mismatch found, but we read current to the end => grab the next node (if any) and starts again.
                                    if (!mismatchFound && j == current.Length && currentEnumerator.MoveNext())
                                    {
                                        current = currentEnumerator.Current;
                                        j = 0;
                                    }

                                    // No mismatch found, but we read otherCurrent to the end => grab the next node (if any) and starts again.
                                    if (!mismatchFound && index == otherCurrent.Length && otherEnumerator.MoveNext())
                                    {
                                        otherCurrent = otherEnumerator.Current;
                                        index = 0;
                                    }
                                }
                            } while (j < current.Length && index < otherCurrent.Length && !mismatchFound);
                        }
                    }
                    else if (otherCurrent.Length < current.Length && current.StartsWith(otherCurrent, comparer))
                    {
                        if (!otherEnumerator.MoveNext())
                        {
                            mismatchFound = true;
                        }
                        else
                        {
                            int index = otherCurrent.Length;
                            otherCurrent = otherEnumerator.Current;
                            int j = 0;
                            do
                            {
                                while (j < otherCurrent.Length && index < current.Length && !mismatchFound)
                                {
                                    char currentChar = current.Span[index];
                                    char otherChar = otherCurrent.Span[j];
                                    mismatchFound = !predicateComparer(currentChar, otherChar);
                                    j++;
                                    index++;

                                    // No mismatch found, but we read current to the end => grab the next node (if any) and starts again.
                                    if (!mismatchFound && j == otherCurrent.Length && otherEnumerator.MoveNext())
                                    {
                                        otherCurrent = otherEnumerator.Current;
                                        j = 0;
                                    }

                                    // No mismatch found, but we read otherCurrent to the end => grab the next node (if any) and starts again.
                                    if (!mismatchFound && index == current.Length && currentEnumerator.MoveNext())
                                    {
                                        current = currentEnumerator.Current;
                                        index = 0;
                                    }
                                }
                            } while (j < otherCurrent.Length && index < current.Length && !mismatchFound);
                        }
                    }
                    else
                    {
                        mismatchFound = true;
                    }

                    if (!mismatchFound)
                    {
                        hasReachedCurrentEnd = !currentEnumerator.MoveNext();
                        hasReachedOtherEnd = !otherEnumerator.MoveNext();
                        equals = hasReachedCurrentEnd && hasReachedOtherEnd;
                    }

                    //equals = !mismatchFound && hasReachedCurrentEnd && hasReachedOtherEnd;
                } while (!mismatchFound && (!hasReachedCurrentEnd || !hasReachedOtherEnd));
            }
        }

        return equals;
    }

    /// <summary>
    /// Checks if the current instance contains <paramref name="search"/>.
    /// </summary>
    /// <param name="search">The value to search in the current instance.</param>
    /// <returns><see langword="true"/> if the current instance contains the specified <paramref name="search"/> and <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// This method uses <see cref="CharComparer.InvariantCultureIgnoreCase"/> comparer.
    /// </remarks>
    public bool Contains(ReadOnlySpan<char> search) => Contains(search, CharComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Checks if the current instance contains <paramref name="search"/>.
    /// </summary>
    /// <param name="search">The value to search in the current instance.</param>
    /// <param name="comparer">The comparer to use when comparing each <see cref="char"/> from <paramref name="search"/>.</param>
    /// <returns><see langword="true"/> if the current instance contains the specified <paramref name="search"/> and <see langword="false"/> otherwise.</returns>
    public bool Contains(ReadOnlySpan<char> search, IEqualityComparer<char> comparer)
    {
        bool found = false;

        if (search.IsEmpty)
        {
            found = true;
        }
        else
        {
            StringSegmentNode currentNode = _head;
            int searchIndex = 0;
            while (currentNode is not null && !found)
            {
                ReadOnlySpan<char> valueSpan = currentNode.Value.Span;

                for (int i = 0; i < valueSpan.Length && !found; i++)
                {
                    if (searchIndex < search.Length && comparer.Equals(valueSpan[i], search[searchIndex]))
                    {
                        searchIndex++;
                    }
                    else
                    {
                        searchIndex = 0;
                        if (valueSpan[i] == search[searchIndex])
                        {
                            searchIndex++;
                        }
                    }

                    found = searchIndex == search.Length;
                }

                currentNode = currentNode.Next;
            }
        }

        return found;
    }

    ///<inheritdoc/>
    public bool Equals(StringSegmentLinkedList other) => other switch
    {
        null => false,
        _    => ReferenceEquals(this, other) || Equals(other, null)
    };

    /// <summary>
    /// Checks if the current instance starts with <paramref name="search"/>.
    /// </summary>
    /// <param name="search">The value to search in the current instance.</param>
    /// <param name="comparer">The comparer to use when comparing each <see langword="char"/> from <paramref name="search"/>.</param>
    /// <returns><see langword="true"/> if the current instance starts the specified <paramref name="search"/> and <see langword="false"/> otherwise.</returns>
    public bool StartsWith(ReadOnlySpan<char> search, IEqualityComparer<char> comparer = null)
    {
        IEqualityComparer<char> localComparer = comparer ?? EqualityComparer<char>.Default;

        bool startsWith;

        if (search.IsEmpty)
        {
            startsWith = true;
        }
        else
        {
            StringSegmentNode current = _head;
            int i = 0;
            bool mismatchFound = false;
            if (search.Length <= current.Value.Length)
            {
                while (i < search.Length && !mismatchFound)
                {
                    mismatchFound = !localComparer.Equals(search[i], current.Value.Span[i]);
                    i++;
                }

                startsWith = !mismatchFound;
            }
            else
            {
                int offset = 0;

                do
                {
                    i = 0;
                    while (i < current.Value.Length && !mismatchFound)
                    {
                        char currentChar = current.Value.Span[i];
                        int searchedCharacterIndex = i + offset;
                        if (searchedCharacterIndex < search.Length)
                        {
                            char searchChar = search[i + (offset)];
                            mismatchFound = !localComparer.Equals(searchChar, currentChar);
                        }

                        i++;
                    }

                    if (!mismatchFound)
                    {
                        offset += current.Value.Length;
                    }

                    current = current.Next;
                } while (offset <= search.Length && !mismatchFound && current is not null);

                bool hasLoopedThroughAllSearchCharacters = offset >= search.Length;
                startsWith = !mismatchFound && hasLoopedThroughAllSearchCharacters;
            }
        }

        return startsWith;
    }

    /// <summary>
    /// Checks if the current instance ends with <paramref name="search"/>.
    /// </summary>
    /// <param name="search">The value to search in the current instance.</param>
    /// <param name="comparer">The comparer to use when comparing each <see cref="char"/> from <paramref name="search"/>.</param>
    /// <returns><see langword="true"/> if the current instance ends with the specified <paramref name="search"/> and <see langword="false"/> otherwise.</returns>
    public bool EndsWith(ReadOnlySpan<char> search, IEqualityComparer<char> comparer = null)
{
    IEqualityComparer<char> localComparer = comparer ?? EqualityComparer<char>.Default;

    bool result = false;

    if (!search.IsEmpty)
    {
        if (_head is not null)
        {
            int totalLength = GetTotalLength();
            if (search.Length <= totalLength)
            {
                int skip = totalLength - search.Length;
                StringSegmentNode current = _head;

                // Skip nodes until we reach the position where the search should start
                while (current is not null && skip >= current.Value.Length)
                {
                    skip -= current.Value.Length;
                    current = current.Next;
                }

                if (current is not null)
                {
                    int indexInNode = skip;
                    int searchIndex = 0;
                    bool mismatchFound = false;

                    // Compare characters from the current position to the end
                    while (current is not null && searchIndex < search.Length && !mismatchFound)
                    {
                        ReadOnlySpan<char> span = current.Value.Span;
                        for (int i = indexInNode; i < span.Length && searchIndex < search.Length && !mismatchFound; i++)
                        {
                            if (!localComparer.Equals(span[i], search[searchIndex]))
                            {
                                mismatchFound = true;
                            }
                            else
                            {
                                searchIndex++;
                            }
                        }

                        current = current.Next;
                        indexInNode = 0;
                    }

                    result = searchIndex == search.Length && !mismatchFound;
                }
            }
        }
    }
    else
    {
        result = true;
    }

    return result;
}
}