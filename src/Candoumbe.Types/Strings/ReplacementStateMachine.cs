using System;
using Stateless;

namespace Candoumbe.Types.Strings;

internal class ReplacementStateMachine
{
    private readonly ReadOnlyMemory<char> _oldString;
    private readonly ReadOnlyMemory<char> _newString;
    private readonly StateMachine<ReplacementState, Trigger> _stateMachine;

    internal ReplacementStateMachine(ReadOnlySpan<char> oldString, ReadOnlySpan<char> newString)
    {
        _oldString = oldString.ToArray();
        _newString = newString.ToArray();
    }
}

internal enum ReplacementState
{
    NotStarted,
    Matching,
    Matched,
    NotMatched,
    Ended
}

internal record Trigger {}