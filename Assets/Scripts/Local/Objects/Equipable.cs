using System.Linq;
using UnityEngine;

public class Equipable : Item {
    public readonly Slot slot;
    public readonly int slotSize;
    public readonly StatModifier[] modifiers;

    public sealed override string Name {
        get {
            return slot switch {
                Slot.Feet => "Boots",
                Slot.Hand => slotSize > 1 ? "Longsword" : "Sword",
                Slot.Head => "Helmet",
                Slot.Legs => "Pants",
                Slot.Neck => "Necklace",
                Slot.Torso => "Shirt",
                Slot.None => "You shouldn't be seeing this.",
                _ => "Error: invalid slot"
            };
        }
    }

    public Equipable(Container container, int size, Slot slot) : base(container, size) {
        this.slot = slot;

        slotSize = slot == Slot.Feet || slot == Slot.Legs || (slot == Slot.Hand && World.random.Bool()) ? 2 : 1;
        modifiers = new[] { new StatModifier(Name, "Stat boost from " + Name, Utility.RandomValue<Stat>(World.random), Random.Range(1, 10)) };
    }

    protected override string InspectText() => $"{base.InspectText()}\nSlot: {slot}{(slotSize > 1 ? " (x" + slotSize + ")" : "")}{modifiers.Aggregate("", (current, equipableModifier) => "\n" + current + equipableModifier)}";

    private void Equip(Character character) => character.equipment.TryAddItem(this);

    private void Unequip(Character character) => character.equipment.UnequipItem(this);


    protected override void Drop(Character character) {
        Unequip(character);
        base.Drop(character);
    }
}

public enum Slot {
    None,
    Head,
    Torso,
    Legs,
    Feet,
    Hand,
    Neck
}
