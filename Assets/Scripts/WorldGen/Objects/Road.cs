using System.Collections.Generic;

public class Road : WorldEntity {
    public readonly Town town;

    private readonly List<Tile> tiles = new();
    public IReadOnlyList<Tile> Tiles => tiles.AsReadOnly();

    private readonly string name;

    public int Population => town.population;

    public Road(Town town) : base(town.Tile) {
        this.town = town;
        town.roads.Add(this);

        name = $"{town.Race.GetPlaceName()} Road";

        AddTile(town.Tile);
    }

    public void AddTile(Tile tile) {
        tiles.Add(tile);
        if (!tile.roads.Contains(this)) tile.roads.Add(this);
    }

    public override string ToString() {
        return name;
    }
}
