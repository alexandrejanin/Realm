using System.Collections.Generic;

public abstract class Interactable : WorldEntity {
    public virtual IList<Interaction> GetInteractions(Character character) {
        return new List<Interaction> { new("Inspect", _ => true, Inspect, false) };
    }

    protected virtual string InspectText() => Name;

    private void Inspect(Character character) => Log.Add(InspectText());
}
