using System.Collections.Generic;

public class StateMachine<T>
{
    private T currentState;
    private Dictionary<T, List<T>> transitions = new Dictionary<T, List<T>>();
    private Dictionary<T, T> coexistingStates = new Dictionary<T, T>();

    public T CurrentState { get { return currentState; } }

    public void AddState(T state)
    {
        if (!transitions.ContainsKey(state))
        {
            transitions[state] = new List<T>();
        }
    }

    public void AddTransition(T fromState, T toState)
    {
        if (!transitions.ContainsKey(fromState))
        {
            AddState(fromState);
        }

        transitions[fromState].Add(toState);
    }

    public void AddTransitions(T fromState, params T[] toStates)
    {
        if (!transitions.ContainsKey(fromState))
        {
            AddState(fromState);
        }

        transitions[fromState].AddRange(toStates);
    }

    public void AddMultipleStates(params T[] states)
    {
        foreach (var state in states)
        {
            AddState(state);
        }
    }

    public void AddCoexistingStates(T state1, T state2)
    {
        coexistingStates[state1] = state2;
        coexistingStates[state2] = state1;
    }

    public bool CanStatesCoexist(T state1, T state2)
    {
        return coexistingStates.ContainsKey(state1) && coexistingStates[state1].Equals(state2);
    }

    public void ChangeState(T newState)
    {
        currentState = newState;
    }
}
