namespace Candoumbe.Types.Strings.UnitTests;

public record StringSegmentLinkedListState(string[] Inputs, string Value)
{
    /// <inheritdoc />
    public override string ToString() => $@"Inputs: [{string.Join(", ", Inputs)}], Value: ""{Value}""";
}