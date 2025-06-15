using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
public class StringSegmentLinkedList : IEnumerable<ReadOnlyMemory<char>>, IEquatable<StringSegmentLinkedList>
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
    public StringSegmentLinkedList(ReadOnlyMemory<char> head, params ReadOnlyMemory<char>[] others)
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
    public StringSegmentLinkedList Append(StringSegment value) => Append(value.AsMemory());

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
    /// <param name="index">0-based index where <paramref name="value"/> will be inserted</param>
    /// <param name="value">The value of the node to insert.</param>
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
    /// <param name="other">The value of the node to insert</param>
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
    /// Gets the number of nodes in the current linked list.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Computes the total length of the resulting string value.
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
    /// This method does its best to never allocate.
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
                // => copy all remaining original chars starting at the index position
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
    /// var result = linkedList.Replace(c => c == 'a' || c == 'b', replacements);
    /// // `result` will have 'a' replaced with "alpha" and 'b' with "beta"
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
                if (!replacements.TryGetValue(current.Value.Span[indexOfOldChar], out ReadOnlyMemory<char> replacement))
                {
                    replacement = ReadOnlyMemory<char>.Empty;
                }

                ReadOnlyMemory<char> value;
                if (indexOfOldChar is 0)
                {
                    value = replacement;
                    replacementList = replacementList.Append(value);
                }
                else
                {
                    value = current.Value[..indexOfOldChar];
                    replacementList = replacementList.Append(value).Append(replacement);
                }

                int index = indexOfOldChar;
                foreach (int occurrence in occurrences.Skip(1))
                {
                    if (!replacements.TryGetValue(current.Value.Span[occurrence], out replacement))
                    {
                        replacement = ReadOnlyMemory<char>.Empty;
                    }

                    if (index < occurrence)
                    {
                        ReadOnlyMemory<char> valueToKeep = current.Value[( index + 1 ) .. occurrence];
                        replacementList = replacementList.Append(valueToKeep)
                            .Append(replacement);
                    }
                    else
                    {
                        replacementList = replacementList.Append(replacement);
                    }

                    index = occurrence;
                }

                // we did all substitutions, but we did not reach the end of the original input
                // => copy all remaining original chars starting at the index position
                if (index < current.Value.Length)
                {
                    replacementList = replacementList.Append(current.Value[( index + 1 )..]);
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
    /// This method does its best to never allocate.
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

    /// <summary>
    /// Tests whether the current instance and <paramref name="other"/> are "equivalent".
    /// </summary>
    /// <param name="other">the other list to test</param>
    /// <param name="comparer">A comparer to use when checking for equality</param>
    /// <returns><see langword="true"/> if the current instance and <paramref name="other"/> are equivalent, and <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <para>
    /// "Equivalency" means that both the current instance and <paramref name="other"/> would output a string that is equivalent to each other.
    /// </para>
    /// <para>
    /// This method tries its best to not allocate the whole string.
    /// </para>
    /// </remarks>
    public bool IsEquivalentTo(StringSegmentLinkedList other, IEqualityComparer<char> comparer)
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

    ///<inheritdoc/>
    public bool Equals(StringSegmentLinkedList other) => other switch
    {
        null => false,
        _    => ReferenceEquals(this, other) || IsEquivalentTo(other, null)
    };
}