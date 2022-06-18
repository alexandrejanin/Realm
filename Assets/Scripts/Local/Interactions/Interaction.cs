using System;

public class Interaction {
    public readonly string name;
    public readonly Predicate<Character> canInteract;
    private readonly Action<Character> action;
    public readonly bool skipTurn;

    public Interaction(string name, Predicate<Character> canInteract, Action<Character> action, bool skipTurn) {
        this.name = name;
        this.canInteract = canInteract;
        this.action = action;
        this.skipTurn = skipTurn;
    }

    public virtual void Invoke(Character character) => action.Invoke(character);
}
