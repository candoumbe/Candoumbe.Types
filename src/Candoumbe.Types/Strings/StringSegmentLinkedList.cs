using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

/// <summary>
/// Represents a linked list data structure for managing <see cref="StringSegment"/> nodes.
/// </summary>
public class StringSegmentLinkedList : IEnumerable<StringSegment>
{
    private StringSegmentNode _head;
    private StringSegmentNode _tail;

    /// <summary>
    /// Builds a new instance of <see cref="StringSegmentLinkedList"/>.
    /// </summary>
    /// <param name="head">the value of the head</param>
    /// <param name="others">Additional <see cref="StringSegment"/>s to append to the list</param>
    public StringSegmentLinkedList(StringSegment head, params StringSegment[] others)
    {
        _head = new StringSegmentNode(head);
        Count = 1;

        foreach (StringSegment next in others)
        {
            Append(next);
        }
    }

    /// <summary>
    /// Appends <paramref name="value"/> to the end of the list
    /// </summary>
    /// <param name="value"></param>
    /// <remarks>The current instance remains untouched if <paramref name="value"/> is empty.</remarks>
    public StringSegmentLinkedList Append(StringSegment value)
    {
        if (value is { Length: > 0 })
        {
            StringSegmentNode newNode = new StringSegmentNode(value);
            AppendInternal(newNode);
        }

        return this;
    }

    private void AppendInternal(StringSegmentNode newNode)
    {
        if (_tail is null)
        {
            _tail = newNode;
            _head.Next = _tail;
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
    /// Inserts a new link containing <paramref name=""/> at the given <paramref name="index"/>.
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

        StringSegmentNode newNode = new StringSegmentNode(value);
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
    public void InsertAt(int index, StringSegmentLinkedList other)
    {
        if (index > 0)
        {
            InsertAtInternal(index, other._head);
        }
        else if (index == 0)
        {
            other.AppendInternal(_head);
            _head = other._head;
        }
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
    /// Computes the <see cref="string"/> values
    /// </summary>
    /// <returns></returns>
    public string ToStringValue()
    {
        StringBuilder sb = new StringBuilder();
        StringSegmentNode current = _head;
        while (current is not null)
        {
            sb.Append(current.Value.Value);
            current = current.Next;
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    public IEnumerator<StringSegment> GetEnumerator()
    {
        yield return _head.Value;

        StringSegmentNode current = _head.Next;
        while (current is not null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    /// <summary>
    /// Replaces
    /// </summary>
    /// <param name="oldChar"><see langword="character"/> to replace.</param>
    /// <param name="newChar"><see langword="character"/> to use as replacement</param>
    /// <returns>The current list where all characters were replaced.</returns>
    public StringSegmentLinkedList Replace(char oldChar, char newChar)
    {
        StringSegmentLinkedList replacementList = this;
        StringSegmentNode current = _head;
        StringSegment replacement = newChar.ToString();
        while (current is not null)
        {
            int indexOfOldChar = current.Value.IndexOf(oldChar);
            if (indexOfOldChar >= 0)
            {
                StringTokenizer tokenizer = current.Value.Split([oldChar]);
                replacementList = new(tokenizer.First());

                foreach (StringSegment segment in tokenizer.Skip(1))
                {
                    replacementList.Append(replacement)
                        .Append(segment);
                }

                current = replacementList._head;
            }

            current = current.Next;
        }

        return replacementList;
    }
}