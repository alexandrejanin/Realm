using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public class MapDisplay : MonoBehaviour {
    [SerializeField, Required] private Transform townsParent, settlersParent, roadsParent;
    [SerializeField, Required] private MeshFilter meshFilter;
    [SerializeField, Required] private MeshRenderer meshRenderer;
    [SerializeField, Required] private MeshCollider meshCollider;
    [SerializeField, Required] private Slider transparencySlider;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private float heightMultiplier;

    [HideInInspector] public MapDrawMode drawMode;
    [HideInInspector] public Race race;

    private float HeightMultiplier => Mathf.Sqrt(World.LongSide) * heightMultiplier;

    private static World World => GameController.World;

    private readonly Dictionary<Town, TownObject> townObjects = new();
    private readonly Dictionary<Settler, SettlerObject> settlerObjects = new();
    private readonly Dictionary<Road, RoadObject> roadObjects = new();

    public void DrawMap(bool reset) {
        if (reset) {
            var mapMesh = MeshGenerator.GenerateTerrainMesh(World, GetHeight);
            meshFilter.sharedMesh = mapMesh;
            meshCollider.sharedMesh = mapMesh;
            meshFilter.transform.position = new Vector3(World.width / 2f, 0, World.height / 2f);

            DrawTexture();
        }

        DisplayObjects(reset);
    }

    public float GetHeight(int x, int y) {
        var tile = GameController.World.GetTile(x, y);
        return tile == null ? 0 : heightCurve.Evaluate(tile.height) * HeightMultiplier;
    }

    public void DrawTexture() {
        var mapTexture = GetTexture();
        meshRenderer.sharedMaterial.mainTexture = mapTexture;
    }

    private void DisplayObjects(bool reset) {
        if (reset) DestroyChildren();

        DisplayTowns(reset);
        DisplaySettlers(reset);
        DisplayRoads(reset);
    }

    private void DisplayTowns(bool reset) {
        if (reset) townObjects.Clear();

        foreach (var town in GameController.World.towns) {
            if (!townObjects.ContainsKey(town)) {
                var townObject = InstantiateOnMap(PrefabManager.Town, town.Tile.position, townsParent);
                townObject.Init(town);

                townObjects.Add(town, townObject);
            }
        }
    }

    private void DisplaySettlers(bool reset) {
        if (reset) settlerObjects.Clear();

        foreach (var town in GameController.World.towns) {
            foreach (var settler in town.settlers) {
                if (!settlerObjects.ContainsKey(settler)) {
                    var settlerObject = InstantiateOnMap(PrefabManager.Settler, settler.tile.position, settlersParent);
                    settlerObject.Init(settler);

                    settlerObjects.Add(settler, settlerObject);
                }
            }
        }
    }

    private void DisplayRoads(bool reset) {
        if (reset) roadObjects.Clear();

        foreach (var town in GameController.World.towns) {
            foreach (var road in town.roads) {
                if (!roadObjects.ContainsKey(road)) {
                    var roadObject = InstantiateOnMap(PrefabManager.Road, town.Tile.position, roadsParent);
                    roadObject.Init(road);

                    roadObjects.Add(road, roadObject);
                }
            }
        }
    }

    private void DestroyChildren() {
        var children = townsParent.Cast<Transform>().Concat(settlersParent.Cast<Transform>()).Concat(roadsParent.Cast<Transform>()).ToArray();

        foreach (var child in children) {
            SafeDestroy(child.gameObject);
        }
    }

    public static void SafeDestroy(GameObject o) {
        if (Application.isPlaying) {
            Destroy(o);
        } else {
            DestroyImmediate(o);
        }
    }

    private static T InstantiateOnMap<T>(T t, Vector2Int position, Transform parent) where T : Object {
        var instance = Instantiate(t, WorldGenUtility.WorldToMeshPoint(position), Quaternion.identity, parent);
        return instance;
    }

    private Texture2D GetTexture() {
        var colors = new Color[World.TileCount];

        for (var y = 0; y < World.height; y++) {
            for (var x = 0; x < World.width; x++) {
                colors[x + World.width * y] = World.GetTile(x, y).GetColor(drawMode, transparencySlider.value, race);
            }
        }

        var texture = new Texture2D(World.width, World.height) { filterMode = FilterMode.Point };

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
}
