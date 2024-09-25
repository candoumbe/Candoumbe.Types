using System;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Candoumbe.Types.Strings;

/// <summary>
/// Represents a linked list data structure for managing <see cref="StringSegment"/> nodes.
/// </summary>
public class StringSegmentLinkedList
{
    public StringSegmentNode Head { get; }
    private StringSegmentNode? tail;
    private int count;

    public StringSegmentLinkedList(StringSegment headValue)
    {
        Head = new StringSegmentNode(headValue);
        tail = Head;
        count = 1;
    }
    

    public void AddLast(StringSegment value)
    {
        StringSegmentNode newNode = new StringSegmentNode(value);
        tail.Next = newNode;
        tail = newNode;
        count++;
    }

    public void InsertAt(int index, StringSegment value)
    {
        if (index < 0 || index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        StringSegmentNode newNode = new StringSegmentNode(value);
        StringSegmentNode current = Head;

        for (int i = 0; i < index; i++)
        {
            current = current.Next;
        }

        newNode.Next = current.Next;
        current.Next = newNode;

        if (newNode.Next == null)
        {
            tail = newNode;
        }

        count++;
    }

    /// <summary>
    /// Gets the number of nodes in the current linked list
    /// </summary>
    public int Count => count;

    
    /// <summary>
    /// Computes the total length of the resulting string value
    /// </summary>
    /// <returns></returns>
    public int GetTotalLength()
    {
        int totalLength = 0;
        StringSegmentNode current = Head;
        while (current != null)
        {
            totalLength += current.Value.Length; // Ajoute la longueur de chaque StringSegment
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
        StringSegmentNode current = Head;
        while (current != null)
        {
            sb.Append(current.Value.Value);
            current = current.Next;
        }
        return sb.ToString();
    }
}