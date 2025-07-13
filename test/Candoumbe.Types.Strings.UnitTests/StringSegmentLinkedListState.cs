namespace Candoumbe.Types.Strings.UnitTests.Strings;

public record StringSegmentLinkedListState(string[] Inputs, string Value)
{
    /// <inheritdoc />
    public override string ToString() => $@"Inputs: [{string.Join(", ", Inputs)}], Value: ""{Value}""";
}