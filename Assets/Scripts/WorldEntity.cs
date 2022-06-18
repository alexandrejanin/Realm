/// <summary>
/// Any entity that exists in the world.
/// </summary>
public abstract class WorldEntity {
    public abstract string Name { get; }
    public abstract Tile Tile { get; }

    public World World => Tile.world;

    public virtual void TakeTurn() { }

    public override string ToString() => Name;
}
