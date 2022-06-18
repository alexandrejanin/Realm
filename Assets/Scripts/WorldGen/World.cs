using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = System.Random;

public class World {
    public readonly WorldParameters parameters;
    public readonly int seed;
    public readonly Random random;

    public readonly int height;
    public readonly int width;

    public int TileCount => height * width;
    public int LongSide => Mathf.Max(height, width);
    public int ShortSide => Mathf.Min(height, width);

    public readonly WorldLog log;

    private readonly Tile[,] tileMap;
    private float[,] heightMap;

    public readonly List<Region> regions = new();
    public readonly List<Faction> factions = new();
    public readonly List<Town> towns = new();

    public int Day { get; private set; }

    public World(WorldParameters parameters, int seed) {
        this.parameters = parameters;
        this.seed = seed;
        random = new Random(seed);

        height = parameters.Height;
        width = parameters.Width;


        tileMap = new Tile[height, width];
        heightMap = new float[height, width];

        log = new WorldLog(this);
    }

    public void Generate() {
        GenerateTileMap();
        GenerateRegions();
        GenerateCivs();
        while (Day < parameters.days)
            Update();
    }

    [CanBeNull]
    public Tile GetTile(Vector2Int v) => GetTile(v.x, v.y);

    [CanBeNull]
    public Tile GetTile(int x, int y) => IsInMap(x, y) ? tileMap[y, x] : null;

    private bool IsInMap(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    private void GenerateTileMap() {
        heightMap = parameters.GenerateHeightMap(seed);
        var tempMap = parameters.GenerateTempMap(heightMap);
        var humidityMap = parameters.GenerateHumidityMap(heightMap, seed);

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                tileMap[x, y] = new Tile(this, x, y, heightMap[x, y], tempMap[x, y], humidityMap[x, y]);
            }
        }
    }

    private void GenerateRegions() {
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var tile = tileMap[x, y];
                if (tile.region == null) FindRegion(tile);
            }
        }
    }

    private void FindRegion(Tile firstTile) {
        var climate = firstTile.climate;

        var tiles = new HashSet<Tile>();
        var queue = new Queue<Tile>();
        queue.Enqueue(firstTile);

        while (queue.Count > 0) {
            var tile = queue.Dequeue();
            tiles.Add(tile);

            for (var j = -1; j <= 1; j++) {
                for (var i = -1; i <= 1; i++) {
                    var newTile = GetTile(tile.x + i, tile.y + j);

                    if (newTile != null && newTile.climate == climate && !tiles.Contains(newTile) && !queue.Contains(newTile)) {
                        queue.Enqueue(newTile);
                    }
                }
            }
        }

        var region = new Region(climate, tiles);
        regions.Add(region);
    }

    private void GenerateCivs() {
        var failedFactions = 0;
        while (factions.Count + failedFactions < parameters.factions) {
            var race = GameController.Races.RandomItem(random);
            var faction = new Faction(race);
            factions.Add(faction);

            var tile = GetBestTile(race);

            if (tile == null) {
                Debug.Log($"Could not find suitable tile for {race}");
                failedFactions++;
                continue;
            }

            var population = (int)(tile.GetRaceCompatibility(race) * 5000);
            faction.capital = new Town(tile, faction, population, null);

            towns.Add(faction.capital);
        }
    }

    private Tile GetBestTile(Race race) {
        Tile bestTile = null;
        foreach (var tile in tileMap) {
            if (bestTile == null || tile.GetTownCompatibility(race) > bestTile.GetTownCompatibility(race)) bestTile = tile;
        }

        return bestTile;
    }

    public void Update() {
        for (var i = 0; i < towns.Count; i++) {
            var town = towns[i];
            town.Update();
        }

        Day++;
    }
}

public enum MapDrawMode {
    Normal,
    Height,
    Temperature,
    Humidity,
    Region,
    Race,
    Town
}
