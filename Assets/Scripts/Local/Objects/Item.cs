using System.Collections.Generic;

public abstract class Item : Interactable {
    public readonly int size;

    private Container container;

    public override Tile Tile => container.Tile;

    protected Item(Container container, int size) {
        this.container = container;
        this.size = size;
    }

    public override IList<Interaction> GetInteractions(Character character) {
        var interactions = base.GetInteractions(character);
        interactions.Add(new Interaction("Drop", c => c.HasItem(this), Drop, false));
        interactions.Add(new Interaction("Pick up", c => !c.HasItem(this), PickUp, false));
        return interactions;
    }

    public void SetContainer(Container newContainer) {
        container.Remove(this);
        container = newContainer;
    }

    private void PickUp(Character character) {
        character.inventory.TryAddItem(this);
    }

    protected virtual void Drop(Character character) => character.Room.floor.TryAddItem(this);

    protected override string InspectText() => Name + "\nSize: " + size;
}
