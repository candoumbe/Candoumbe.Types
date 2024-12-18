using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Candoumbe.MiscUtilities.Comparers;
using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

/// <summary>
/// Represents a linked list data structure for managing <see cref="StringSegment"/> nodes.
/// </summary>
/// <remarks>
/// This implementation is specifically designed to not allow appending <see cref="StringSegment.Empty"/> values.
/// </remarks>
public class StringSegmentLinkedList : IEnumerable<ReadOnlyMemory<char>>
{
    private StringSegmentNode _head;
    private StringSegmentNode _tail;

    private static readonly StringSegmentNode EmptyNode = new(ReadOnlyMemory<char>.Empty);

    private readonly IDictionary<string, string> _replacements;

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/> that is empty.
    /// </summary>
    public StringSegmentLinkedList()
    {
        _head = EmptyNode;
        Count = 0;
        _replacements = new Dictionary<string, string>();
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">the value of the head</param>
    /// <param name="others">Additional <see cref="StringSegment"/>s to append to the list</param>
    public StringSegmentLinkedList(StringSegment head, params StringSegment[] others) : this()
    {
        _head = new StringSegmentNode(head);
        Count = 1;

        foreach (StringSegment next in others)
        {
            Append(next);
        }

        _replacements = new Dictionary<string, string>();
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">the value of the head</param>
    /// <param name="others">Additional <see cref="ReadOnlyMemory{T}"/>s to append to the list</param>
    internal StringSegmentLinkedList(ReadOnlyMemory<char> head, params ReadOnlyMemory<char>[] others)
    {
        _head = new StringSegmentNode(head);
        Count = 1;

        foreach (ReadOnlyMemory<char> next in others)
        {
            Append(next);
        }

        _replacements = new Dictionary<string, string>();
    }

    /// <summary>
    /// Appends <paramref name="value"/> to the end of the list.
    /// </summary>
    /// <param name="value"></param>
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    public StringSegmentLinkedList Append(ReadOnlyMemory<char> value)
    {
        if (value is { Length: > 0 })
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
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    public StringSegmentLinkedList Append(StringSegment value) => Append((ReadOnlyMemory<char>)value);

    private void AppendInternal(StringSegmentNode newNode)
    {
        if (_tail is null)
        {
            _tail = newNode;
            if (_head == EmptyNode)
            {
                _head = newNode;
            }
            else
            {
                _head.Next = _tail;
            }
        }
        else
        {
            _head.Next ??= _tail;
            _tail.Next = newNode;
            _tail = newNode;
        }

        Count++;
    }

    /// <summary>
    /// Inserts a new link containing <paramref name="value"/> at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">0-based index where the new node will be inserted</param>
    /// <param name="value">The value of the node to inserts</param>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is &lt; 0. or &gt; <see cref="Count"/>.</exception>
    public void InsertAt(int index, StringSegment value)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        if (value is { Length: 0 })
        {
            return;
        }

        StringSegmentNode newNode = new(value);
        InsertAtInternal(index, newNode);
    }

    private void InsertAtInternal(int index, StringSegmentNode newNode)
    {
        if (index == 0)
        {
            newNode.Next = _head;
            _head = newNode;
        }
        else
        {
            StringSegmentNode current = _head;
            StringSegmentNode previous = null;

            for (int i = 0; i < index; i++)
            {
                previous = current;
                current = current!.Next;
            }

            previous ??= newNode;
            previous.Next = newNode;
            newNode.Next ??= _tail;
        }

        Count++;
    }

    /// <summary>
    /// Inserts <paramref name="other"/> at the given <paramref name="index"/>
    /// </summary>
    /// <param name="index">0-based index where the new node will be inserted</param>
    /// <param name="other">The value of the node to inserts</param>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is &lt; 0. or &gt; <see cref="Count"/>.</exception>
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
            case 0:
                other.AppendInternal(_head);
                _head = other._head;
                break;
        }

        return this;
    }

    /// <summary>
    /// Gets the number of nodes in the current linked list
    /// </summary>
    public int Count { get; private set; }

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
    /// <returns></returns>
    public string ToStringValue()
    {
        StringBuilder sb = new();
        StringSegmentNode current = _head;
        while (current is not null)
        {
            ReadOnlyMemory<char> segment = current.Value;
            sb.Append(segment.Span);
            current = current.Next;
        }

        if (_replacements is not null)
        {
            foreach (( string key, string value ) in _replacements)
            {
                sb = sb.Replace(key, value);
            }
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    public IEnumerator<ReadOnlyMemory<char>> GetEnumerator()
    {
        if (_head == EmptyNode)
        {
            yield break;
        }

        yield return _head.Value;

        StringSegmentNode current = _head.Next;
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
    /// This method does its best to never allocated.
    /// Also, beware that the returned <see cref="StringSegmentLinkedList"/> may have more <see cref="StringSegmentNode">nodes</see> than
    /// the current instance.
    /// </remarks>
    public StringSegmentLinkedList Replace(char oldChar, char newChar) => Replace(oldChar, [newChar]);

    /// <summary>
    /// Replaces all <paramref name="oldChar"/> by <paramref name="replacement"/>.
    /// </summary>
    /// <param name="oldChar"><see langword="character"/> to replace.</param>
    /// <param name="replacement"><see langword="string"/> that will replace <paramref name="oldChar"/>.</param>
    /// <returns>The current list where all characters were replaced.</returns>
    /// <remarks>
    /// This method does its best to never allocated.
    /// Also, beware that the returned <see cref="StringSegmentLinkedList"/> may have more <see cref="StringSegmentNode">nodes</see> than
    /// the current instance.
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
    /// has been replaced with <paramref name="predicate"/>.
    /// </returns>
    public StringSegmentLinkedList Replace(Func<char, bool> predicate, ReadOnlySpan<char> replacement)
    {
        StringSegmentLinkedList replacementList = [];
        StringSegmentNode current = _head;
        ReadOnlyMemory<char> replacementMemory = replacement.ToArray();

        while (current is not null)
        {
            int indexOfOldChar = -1;
            IEnumerable<int> occurrences = [];
            Parallel.Invoke(() => indexOfOldChar = current.Value.FirstOccurrence(predicate),
                            () => occurrences = current.Value.Occurrences(predicate));

            if (indexOfOldChar >= 0)
            {
                ReadOnlyMemory<char> valueToKeep = current.Value[..indexOfOldChar];
                replacementList.Append(valueToKeep).Append(replacementMemory);

                int index = indexOfOldChar + 1;
                foreach (int occurrence in occurrences.Skip(1))
                {
                    replacementList = replacementList.Append(replacementMemory);
                    if (index < occurrence)
                    {
                        valueToKeep = current.Value.Slice(index, occurrence);
                        replacementList.Append(valueToKeep);
                    }

                    // move the cursor right after the current occurrence
                    index = occurrence + 1;
                }

                // we did all substitutions, but we did not reach the end of the original input
                // => copy all remaining original chars starting at index position  
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

        return replacementList;
    }

    public StringSegmentLinkedList Replace(Func<char, bool> predicate, IReadOnlyDictionary<char, ReadOnlyMemory<char>> replacement)
    {
        StringSegmentLinkedList replacementList = [];
        StringSegmentNode current = _head;

        while (current is not null)
        {
            int indexOfOldChar = -1;
            IEnumerable<int> occurrences = [];
            Parallel.Invoke(() => indexOfOldChar = current.Value.FirstOccurrence(predicate),
                () => occurrences = current.Value.Occurrences(predicate));

            if (indexOfOldChar >= 0)
            {
                ReadOnlyMemory<char> valueToKeep = current.Value[..indexOfOldChar];

                if (!replacement.TryGetValue(current.Value.Span[indexOfOldChar], out ReadOnlyMemory<char> replacementMemory))
                {
                    replacementMemory = ReadOnlyMemory<char>.Empty;
                }

                replacementList.Append(valueToKeep).Append(replacementMemory);

                int index = indexOfOldChar + 1;
                foreach (int occurrence in occurrences.Skip(1))
                {
                    replacementList = replacementList.Append(replacementMemory);
                    if (index < occurrence)
                    {
                        valueToKeep = current.Value.Slice(index, occurrence);
                        replacementList.Append(valueToKeep);
                    }

                    // move the cursor right after the current occurrence
                    index = occurrence + 1;
                }

                // we did all substitutions, but we did not reach the end of the original input
                // => copy all remaining original chars starting at index position  
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

        return replacementList;
    }

    /// <summary>
    /// Replaces all <paramref name="oldString"/> by <paramref name="newString"/>.
    /// </summary>
    /// <param name="oldString"><see langword="string"/> to replace.</param>
    /// <param name="newString"><see langword="string"/> that will replace <paramref name="oldString"/>.</param>
    /// <returns>The current list where all characters were replaced.</returns>
    /// <remarks>
    /// This method does its best to never allocated.
    /// Also, beware that the returned <see cref="StringSegmentLinkedList"/> may have more <see cref="StringSegmentNode">nodes</see> than
    /// the current instance had.
    /// </remarks>
    public StringSegmentLinkedList Replace(string oldString, string newString)
    {
        _replacements.Add(oldString, newString);

        return this;
    }

    /// <summary>
    /// Remove nodes by a predicate
    /// </summary>
    /// <param name="predicate">filter</param>
    /// <returns>A new <see cref="StringSegmentLinkedList"/> where all candidates <paramref name="predicate"/> were removed.</returns>
    public StringSegmentLinkedList RemoveBy(Func<ReadOnlyMemory<char>, bool> predicate)
    {
        StringSegmentNode current = _head;
        StringSegmentNode previous = null;
        while (current is not null)
        {
            if (predicate(current.Value))
            {
                if (previous is not null)
                {
                    previous.Next = current.Next;
                }
                else
                {
                    Debug.Assert(_head == current);
                    _head = _tail;
                }
            }

            previous = current;
            current = current.Next;
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
        if (current != EmptyNode)
        {
            while (current is not null)
            {
                result = result.Append(current.Value);
                current = current.Next;
            }
        }

        current = other._head;
        if (current != EmptyNode)
        {
            if (result.Count == 0)
            {
                result = new StringSegmentLinkedList(current.Value);
                current = current.Next;
            }

            while (current is not null)
            {
                result = result.Append(current.Value);
                current = current.Next;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public bool Equals(StringSegmentLinkedList other, IEqualityComparer<char> comparer = null)
    {
        bool equals = false;

        if (other is not null)
        {
            using IEnumerator<ReadOnlyMemory<char>> currentEnumerator = GetEnumerator();
            using IEnumerator<ReadOnlyMemory<char>> otherEnumerator = other.GetEnumerator();

            if (!currentEnumerator.MoveNext() && !otherEnumerator.MoveNext())
            {
                equals = true;
            }
            else
            {
                ReadOnlyMemory<char> current = currentEnumerator.Current;
                ReadOnlyMemory<char> otherCurrent = otherEnumerator.Current;

                if (current.Length == otherCurrent.Length)
                {
                    equals = current.Equals(otherCurrent);
                }
                else if (current.Length < otherCurrent.Length && otherCurrent.StartsWith(current))
                {
                    bool mismatchFound = false;
                    int index = current.Length;
                    Func<char, char, bool> predicate = comparer switch
                    {
                        null => (x, y) => Equals(x, y),
                        _ => comparer.Equals,
                    };
                    while (currentEnumerator.MoveNext() && !mismatchFound)
                    {
                        current = currentEnumerator.Current;
                        int j = 0;
                        while (j < current.Length && index < otherCurrent.Length && !mismatchFound)
                        {
                            mismatchFound = !current.Slice(j, 1).StartsWith(otherCurrent.Slice(index, 1), comparer);
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
                    }

                    equals = !mismatchFound;
                }
                else
                {
                    bool mismatchFound = false;
                    int index = otherCurrent.Length;
                    while (otherEnumerator.MoveNext() && !mismatchFound)
                    {
                        otherCurrent = otherEnumerator.Current;
                        int j = 0;
                        while (j < otherCurrent.Length && index < current.Length && !mismatchFound)
                        {
                            mismatchFound = !current.Slice(index, 1).StartsWith(otherCurrent.Slice(j, 1), comparer);
                            j++;
                            index++;

                            // No mismatch found, and we read current to the end => grab the next node (if any) and starts again.
                            if (!mismatchFound && index == current.Length && currentEnumerator.MoveNext())
                            {
                                current = currentEnumerator.Current;
                                index = 0;
                            }

                            // No mismatch found, and we read otherCurrent to the end => grab the next node (if any) and starts again.
                            if (!mismatchFound && j == otherCurrent.Length && otherEnumerator.MoveNext())
                            {
                                otherCurrent = otherEnumerator.Current;
                                j = 0;
                            }
                        }
                    }

                    equals = !mismatchFound;
                }
            }
        }

        return equals;
    }
}