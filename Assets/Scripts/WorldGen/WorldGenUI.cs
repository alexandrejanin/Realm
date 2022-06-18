using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class WorldGenUI : MonoBehaviour {
    [SerializeField, Required] private Text tileInfo, mapInfo;
    [SerializeField, Required] private Dropdown mapSizeDropdown, mapDrawModeDropdown, raceDropdown;

    private static World World => GameController.World;

    private MapDisplay mapDisplay;

    private void Awake() {
        mapDisplay = GetComponent<MapDisplay>();

        mapSizeDropdown.value = GameController.Presets.Contains(GameController.WorldParameters)
            ? GameController.Presets.IndexOf(GameController.WorldParameters)
            : 0;

        mapDrawModeDropdown.FromList(Enum.GetNames(typeof(MapDrawMode)));

        OnDrawModeChanged();

        raceDropdown.FromList(GameController.Races.Select(race => race.collectiveName.Capitalize()).ToArray());

        OnRaceChanged();
    }

    [UsedImplicitly]
    public void OnDrawModeChanged() {
        var mapDrawMode = (MapDrawMode)mapDrawModeDropdown.value;

        mapDisplay.drawMode = mapDrawMode;

        mapDisplay.DrawTexture();
    }

    [UsedImplicitly]
    public void OnTransparencyChanged(float f) {
        mapDisplay.DrawTexture();
    }

    public void OnRaceChanged() {
        var race = GameController.Races[raceDropdown.value];

        mapDisplay.race = race;

        mapDisplay.DrawTexture();
    }

    public void OnMapChanged() {
        var mapText = $"Seed: {World.seed}\nPopulation: {World.towns.Sum(t => t.population)}\nDay: {World.Day}";

        foreach (var climate in GameController.Climates) {
            var validRegions = World.regions.Where(region => region.climate == climate).ToList();
            var regionsCount = validRegions.Count;
            if (regionsCount == 0) continue;
            var tilesCount = validRegions.Sum(region => region.Size);

            mapText += $"\n{regionsCount} {climate.name}s ({tilesCount} tiles)";
        }

        mapInfo.text = mapText;
    }

    private void Update() {
        RaycastHit hit;
        if (Physics.Raycast(GameController.WorldCamera.camera.ScreenPointToRay(Input.mousePosition), out hit)) {
            var pos = WorldGenUtility.MeshToWorldPoint(hit.point);

            var tile = World.GetTile(pos);

            if (tile == null) return;

            float height = GameController.WorldGenUtility.WorldHeightToMeters(tile.height);
            float temp = GameController.WorldGenUtility.TemperatureToCelsius(tile.temp);

            var text = $"Position: {tile.position}\nHeight: {height}m ({tile.height:F3})\nTemp: {temp}°C ({tile.temp:F3})\nRegion: {tile.region}";

            if (tile.location != null) text += $"\n{tile.location}";

            var town = tile.Town;
            if (town != null) {
                text += $"\nPopulation: {town.population}";
            }

            tileInfo.text = text;

            if (Input.GetMouseButtonDown(0) && tile.location != null) {
                GameController.Instance.LoadLocation(tile.location);
            }

            if (Input.GetMouseButtonDown(1)) {
                GameController.WorldCamera.target = hit.point;
            }
        }
    }
}
