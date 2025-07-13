using Candoumbe.Types.Strings;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Fluent;

namespace Candoumbe.Types.Strings.UnitTests.Strings;

/// <summary>
/// Operation of replacing a <see langword="char"/> by another.
/// </summary>
internal class ReplaceCharWithChar : Operation<StringSegmentLinkedList, StringSegmentLinkedListState>
{
    private readonly char _oldValue;
    private readonly char _newValue;

    /// <summary>
    /// Builds a new <see cref="ReplaceCharWithChar"/> instance.
    /// </summary>
    /// <param name="oldValue">The character to replace</param>
    /// <param name="newValue">The character that will replace <paramref name="oldValue"/></param>
    internal ReplaceCharWithChar(char oldValue, char newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    /// <inheritdoc />
    public override Property Check(StringSegmentLinkedList sut, StringSegmentLinkedListState state)
    {
        sut = sut.Replace(_oldValue, _newValue);
        string actual = sut.ToStringValue();

        return ( actual == state.Value ).Label($"""
                                                Actual : "{actual}"
                                                Expect : "{state.Value}"
                                                """);
    }

    /// <inheritdoc />
    public override StringSegmentLinkedListState Run(StringSegmentLinkedListState state)
        => state with {Value = state.Value.Replace(_oldValue, _newValue)};

    /// <inheritdoc />
    public override string ToString() => $"replace char '{_oldValue}' (ascii: {(int)_oldValue}) with char '{_newValue}' (ascii: {(int)_newValue})";
}