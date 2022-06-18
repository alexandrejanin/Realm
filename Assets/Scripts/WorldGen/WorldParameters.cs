using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

[Serializable, CreateAssetMenu]
public class WorldParameters : ScriptableObject {
    [SerializeField] private bool updateOnRefresh;

    [field: SerializeField, OnValueChanged(nameof(OnUpdate))]
    public int Height { get; private set; }

    [field: SerializeField, OnValueChanged(nameof(OnUpdate))]
    public int Width { get; private set; }

    [Header("Heightmap"), SerializeField] private NoiseSettings heightSettings;

    [Range(0, 10)] public float falloffA;
    [Range(1, 10)] public float falloffB;
    [Range(0, 1)] public float falloffMultiplier;

    [Header("Temperature")] [Range(0, 10)] public float tempA;

    [Range(0.5f, 1.5f)] public float tempB;
    [Range(0, 1)] public float maxTempHeight = .33f, heightTempMultiplier;

    [Header("Humidity"), SerializeField] private NoiseSettings humiditySettings;
    [Range(0, 1)] public float maxHumidityHeight = .33f, heightHumidityMultiplier;

    [Header("Civilizations")] [MinValue(0)]
    public int factions;

    [MinValue(0)] public int days;

    public float[,] GenerateHeightMap(int seed) {
        var heightMap = GenerateNoiseMap(Height, Width, seed, heightSettings);
        var falloffMap = GenerateFalloffMap(Height, Width, falloffA, falloffB);

        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                heightMap[y, x] = Mathf.Clamp01(heightMap[y, x] - falloffMap[y, x] * falloffMultiplier);
            }
        }

        return heightMap;
    }

    public float[,] GenerateHumidityMap(float[,] heightMap, int seed) {
        var humidityMap = GenerateNoiseMap(Height, Width, 397 * seed, humiditySettings);

        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var humidity = (heightMap[y, x] * heightMap[y, x] - maxHumidityHeight * maxHumidityHeight) * heightHumidityMultiplier;

                humidityMap[y, x] = Mathf.Clamp01(humidityMap[y, x] - humidity);
            }
        }

        return humidityMap;
    }

    public float[,] GenerateTempMap(float[,] heightMap) {
        var tempMap = new float[Height, Width];

        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var heightTemp = Mathf.Abs(heightMap[y, x] * heightMap[y, x] - maxTempHeight * maxTempHeight) * heightTempMultiplier;
                var latitudeTemp = 1 - Mathf.Abs(Height - y) / (float)Height;

                tempMap[y, x] = Evaluate(Mathf.Clamp01(latitudeTemp - heightTemp), tempA, tempB);
            }
        }

        return tempMap;
    }

    private static float[,] GenerateNoiseMap(int height, int width, int seed, NoiseSettings noiseSettings) {
        var noiseMap = new float[height, width];
        var random = new Random(seed);

        var octaveOffsets = new Vector2[noiseSettings.octaves];
        for (var i = 0; i < noiseSettings.octaves; i++) {
            float offsetX = random.Next(-99999, 99999);
            float offsetY = random.Next(-99999, 99999);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        var maxNoiseHeight = float.MinValue;
        var minNoiseHeight = float.MaxValue;

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (var i = 0; i < noiseSettings.octaves; i++) {
                    var sampleX = (x - width / 2 + octaveOffsets[i].x) / noiseSettings.scale * frequency;
                    var sampleY = (y - height / 2 + octaveOffsets[i].y) / noiseSettings.scale * frequency;
                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= noiseSettings.persistence;
                    frequency *= noiseSettings.lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                }

                if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[y, x] = noiseHeight;
            }
        }

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                noiseMap[y, x] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[y, x]);
            }
        }

        return noiseMap;
    }

    private static float[,] GenerateFalloffMap(int height, int width, float falloffA, float falloffB) {
        var map = new float[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var value = Mathf.Max(Mathf.Abs(y / (float)height * 2 - 1), Mathf.Abs(x / (float)width * 2 - 1));
                var falloff = Evaluate(value, falloffA, falloffB);
                map[y, x] = falloff;
            }
        }

        return map;
    }

    private static float Evaluate(float value, float a, float b) => Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));

    private void OnUpdate() {
        if (updateOnRefresh) GameController.Instance.GenerateWorld();
    }
}

[Serializable]
public struct NoiseSettings {
    [Range(1, 8)] public int octaves;
    [Range(0, 1)] public float persistence;
    [Range(1, 5)] public float lacunarity;
    [Range(10, 500)] public int scale;
}
