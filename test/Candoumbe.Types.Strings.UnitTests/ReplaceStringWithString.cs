using Candoumbe.Types.Strings;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Fluent;

namespace Candoumbe.Types.Strings.UnitTests.Strings;

/// <summary>
/// Operation of replacing a <see langword="string"/> by another.
/// </summary>
internal class ReplaceStringWithString : Operation<StringSegmentLinkedList, StringSegmentLinkedListState>
{
    private readonly string _oldValue;
    private readonly string _newValue;

    /// <summary>
    /// Builds a new <see cref="ReplaceStringWithString"/> instance.
    /// </summary>
    /// <param name="oldValue">The character to replace</param>
    /// <param name="newValue">The character that will replace <paramref name="oldValue"/></param>
    internal ReplaceStringWithString(string oldValue, string newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    /// <inheritdoc />
    public override bool Pre(StringSegmentLinkedListState currentState) => currentState.Value.Length > 0;

    /// <inheritdoc />
    public override Property Check(StringSegmentLinkedList sut, StringSegmentLinkedListState state)
    {
        StringSegmentLinkedList result  = sut.Replace(_oldValue, _newValue);
        string actual = result.ToStringValue();
        return ( actual == state.Value ).Label($"""
                                                Actual : "{actual}"
                                                Expect : "{state.Value}"
                                                """);
    }

    /// <inheritdoc />
    public override StringSegmentLinkedListState Run(StringSegmentLinkedListState state)
        => state with { Value = state.Value.Replace(_oldValue, _newValue) };

    /// <inheritdoc />
    public override string ToString() => $@"replace string ""{_oldValue}"" with string ""{_newValue}""";
}