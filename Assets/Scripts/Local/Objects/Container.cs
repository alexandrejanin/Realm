using System.Collections.Generic;

public abstract class Container : Interactable {
    public virtual HashSet<Item> Items { get; } = new();

    public virtual bool CanAddItem(Item item) => true;

    public int ItemCount => Items.Count;

    public virtual bool TryAddItem(Item item) {
        if (CanAddItem(item)) {
            item.SetContainer(this);
            return true;
        }

        return false;
    }

    public virtual void Remove(Item item) {
        Items.Remove(item);
    }

    public bool Contains(Item item) => Items.Contains(item);

    protected override string InspectText() => Name + $" ({ItemCount} items)";
}
