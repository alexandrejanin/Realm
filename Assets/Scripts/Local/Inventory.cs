using System.Linq;

public class Inventory : Container {
    public override string Name => character.Name + "'s inventory";
    public override Tile Tile => character.Tile;

    public readonly Character character;

    public readonly int maxSize;

    public int FreeSpace => maxSize - TakenSpace;
    public int TakenSpace => Items.Sum(item => item.size);

    public override bool CanAddItem(Item item) => base.CanAddItem(item) && FreeSpace >= item.size;

    public Inventory(Character character, int maxSize) {
        this.character = character;
        this.maxSize = maxSize;
    }
}
