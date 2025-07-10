using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Candoumbe.MiscUtilities.Comparers;
using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

/// <summary>
/// Represents a linked list data structure for managing <see cref="StringSegment"/> nodes.
/// </summary>
/// <remarks>
/// This implementation is specifically designed to not allow appending <see cref="StringSegment.Empty"/> values.
/// </remarks>
public class StringSegmentLinkedList : IEnumerable<ReadOnlyMemory<char>>, IEquatable<StringSegmentLinkedList>
{
    private StringSegmentNode _head;
    private StringSegmentNode _tail;

    private static StringSegmentNode EmptyNode => new(ReadOnlySpan<char>.Empty);

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/> that is empty.
    /// </summary>
    public StringSegmentLinkedList(): this(ReadOnlySpan<char>.Empty)
    {
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">The value of the head</param>
    public StringSegmentLinkedList(ReadOnlySpan<char> head)
    {
        (_head, Count) = head.IsEmpty
                    ? (EmptyNode, 1)
                    : (new StringSegmentNode(head), 1);
    }

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">the value of the head</param>
    /// <param name="others">Additional elements to append to the list.</param>
    public StringSegmentLinkedList(ReadOnlySpan<char> head, params ReadOnlySpan<StringSegment> others)
    {
        _head = new StringSegmentNode(head);
        Count = 1;

        foreach (ReadOnlyMemory<char> next in others)
        {
            Append(next.Span);
        }
    }

    /// <summary>
    /// Appends <paramref name="value"/> to the end of the list.
    /// </summary>
    /// <param name="value">The value to append</param>
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    public StringSegmentLinkedList Append(ReadOnlySpan<char> value)
    {
        if (value is { Length: > 0 })
        {
            StringSegmentNode newNode = new(value);
            AppendInternal(newNode);
        }

        return this;
    }

    private void AppendInternal(StringSegmentNode newNode)
    {
        if (_tail is null)
        {
            _tail = newNode;
            if (_head.Equals(EmptyNode))
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
    /// <param name="index">0-based index where <paramref name="value"/> will be inserted</param>
    /// <param name="value">The value of the node to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="index"/> is &lt; 0. or &gt; <see cref="Count"/>.</exception>
    public void InsertAt(int index, ReadOnlySpan<char> value)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        if (value is not { Length: 0 })
        {
            StringSegmentNode newNode = new(value);
            InsertAtInternal(index, newNode);
        }
    }

    private void InsertAtInternal(int index, StringSegmentNode newNode)
    {
        if (index is 0)
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

        return sb.ToString();
    }

    /// <inheritdoc />
    public IEnumerator<ReadOnlyMemory<char>> GetEnumerator()
    {
        if (_head.Equals(EmptyNode))
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
    /// This method does its best to never allocate.
    /// Also, beware that the returned <see cref="StringSegmentLinkedList"/> may have more <see cref="StringSegmentNode">nodes</see> than
    /// the current instance.
    /// </remarks>
    public StringSegmentLinkedList Replace(char oldChar, char newChar) => Replace(oldChar, [newChar]);

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
    public StringSegmentLinkedList Replace(Func<char, bool> oldChar, char newChar) => Replace(oldChar, [newChar]);

    /// <summary>
    /// Replaces all <paramref name="oldChar"/> by <paramref name="replacement"/>.
    /// </summary>
    /// <param name="oldChar"><see langword="character"/> to replace.</param>
    /// <param name="replacement"><see langword="string"/> that will replace <paramref name="oldChar"/>.</param>
    /// <returns>The current list where all characters were replaced.</returns>
    /// <remarks>
    /// This method does its best to never allocate.
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
    /// has been replaced with <paramref name="replacement"/>.
    /// </returns>
    public StringSegmentLinkedList Replace(Func<char, bool> predicate, ReadOnlySpan<char> replacement)
    {
        StringSegmentLinkedList replacementList = [];
        StringSegmentNode current = _head;
        ReadOnlyMemory<char> replacementMemory = replacement.ToArray();

        while (current is not null)
        {
            int indexOfOldChar = current.Value.FirstOccurrence(predicate);
            IEnumerable<int> occurrences = current.Value.Occurrences(predicate);

            if (indexOfOldChar >= 0)
            {
                ReadOnlySpan<char> valueToKeep;
                if (indexOfOldChar > 0)
                {
                    valueToKeep = current.Value[..indexOfOldChar].Span;
                    replacementList = replacementList.Append(valueToKeep);
                }

                replacementList = replacementList.Append(replacement);

                int index = indexOfOldChar + 1;
                foreach (int occurrence in occurrences.Skip(1))
                {
                    //replacementList = replacementList.Append(replacementMemory.Span);
                    if (index < occurrence)
                    {
                        valueToKeep = current.Value.Span.Slice(index, occurrence - index);
                        replacementList = replacementList.Append(valueToKeep);
                    }

                    replacementList = replacementList.Append(replacement);

                    // move the cursor right after the current occurrence
                    index = occurrence + 1;
                }

                // we did all substitutions, but we did not reach the end of the original input
                // => copy all remaining original chars starting at the index position
                if (index < current.Value.Length)
                {
                    replacementList = replacementList.Append(current.Value[index..].Span);
                }
            }
            else
            {
                replacementList = replacementList.Append(current.Value.Span);
            }

            current = current.Next;
        }

        return replacementList;
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
        StringSegmentLinkedList replacementList = [];
        StringSegmentNode current = _head;

        while (current is not null)
        {
            int indexOfOldChar = current.Value.FirstOccurrence(predicate);
            IEnumerable<int> occurrences = current.Value.Occurrences(predicate);

            if (indexOfOldChar >= 0)
            {
                replacementList = replacementList.Append(current.Value.Span[..indexOfOldChar]);

                char candidate = current.Value.Span[indexOfOldChar];
                if (!replacements.TryGetValue(candidate, out ReadOnlyMemory<char> replacement))
                {
                    replacement = ReadOnlyMemory<char>.Empty;
                }

                replacementList = replacementList.Append(replacement.Span);

                int index = indexOfOldChar;
                foreach (int occurrence in occurrences.Skip(1))
                {
                    replacementList = replacementList.Append(replacement.Span);
                    if (index < occurrence)
                    {
                        ReadOnlyMemory<char> valueToKeep = current.Value.Slice(index, occurrence);
                        replacementList = replacementList.Append(valueToKeep.Span);
                    }

                    // move the cursor right after the current occurrence
                    indexOfOldChar = occurrence;
                }

                // we did all substitutions, but we did not reach the end of the original input
                // => copy all remaining original chars starting at index position
                if (indexOfOldChar < current.Value.Length)
                {
                    index = indexOfOldChar + 1;
                    replacementList = replacementList.Append(current.Value[index..].Span);
                }
            }
            else
            {
                replacementList = replacementList.Append(current.Value.Span);
            }

            current = current.Next;
        }

        _head = replacementList._head;

        return this;
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
        StringSegmentLinkedList replacementList = [];
        StringSegmentNode current = _head;
        int oldStringLength = oldValue.Length;

        while (current is not null)
        {
            ReadOnlySpan<char> subject = current.Value.Span;
            int index = subject.IndexOf(oldValue[0]);

            // no match in the current node
            if (index is -1)
            {
                replacementList = replacementList.Append(subject);
            }
            else
            {
                int previousIndex = 0;
                do
                {
                    replacementList = replacementList.Append(subject[..index]);
                    int offset = 0;
                    bool mismatchFound;
                    do
                    {
                        mismatchFound = subject[index] != oldValue[offset];
                        index++;
                        offset++;
                    } while (offset < oldStringLength && index < subject.Length && !mismatchFound);

                    replacementList = mismatchFound switch
                    {
                        // a mismatch was found: we need to rewind and copy from the previous index up to the current one
                        true => replacementList.Append(subject[( previousIndex + 1 )..index]),
                        // we looped through the whole span and, up to here, everything matches => we just append newValue
                        _ => replacementList.Append(newValue)
                    };

                    subject = subject[index..];
                    previousIndex = index;
                    index = subject.IndexOf(oldValue[0]);
                } while (index != -1 && subject.Length > 0);

                if (!subject.IsEmpty)
                {
                    replacementList = replacementList.Append(subject);
                }
            }

            current = current.Next;
        }

        return replacementList;
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
                    Debug.Assert(_head.Equals(current));
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

        StringSegmentLinkedList result = [];
        StringSegmentNode current = _head;
        if (!current.Equals(EmptyNode))
        {
            while (current is not null)
            {
                result = result.Append(current.Value.Span);
                current = current.Next;
            }
        }

        current = other._head;
        if (!current.Equals(EmptyNode))
        {
            if (result.Count == 0)
            {
                result = new StringSegmentLinkedList(current.Value.Span);
                current = current.Next;
            }

            while (current is not null)
            {
                result = result.Append(current.Value.Span);
                current = current.Next;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is StringSegmentLinkedList other && Equals(other, CharComparer.InvariantCultureIgnoreCase);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        HashCode hashCode = new();

        StringSegmentNode current = _head;

        while (current is not null)
        {
            hashCode.Add(current.Value.Span.GetHashCode());
            current = current.Next;
        }

        return hashCode.ToHashCode();
    }

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
                        mismatchFound = !( current.StartsWith(otherCurrent, comparer) || otherCurrent.StartsWith(current, comparer) );
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
                            } while (j < current.Length  && index < otherCurrent.Length && !mismatchFound);
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
                } while (!mismatchFound && ( !hasReachedCurrentEnd || !hasReachedOtherEnd ));
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
                var valueSpan = currentNode.Value.Span;

                for (int i = 0; i < valueSpan.Length && !found; i++)
                {
                    if (searchIndex < search.Length && comparer.Equals(valueSpan[i],search[searchIndex]))
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
}