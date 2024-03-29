﻿using UnityEngine;

public class WorldGenUtility : MonoBehaviour {
    [SerializeField] private AnimationCurve temperatureCurve;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private float curveToDegreesRatio;
    [SerializeField] private float curveToMetersRatio;

    public int TemperatureToCelsius(float temp) => Mathf.RoundToInt(curveToDegreesRatio * temperatureCurve.Evaluate(temp));

    public int WorldHeightToMeters(float height) => Mathf.RoundToInt(curveToMetersRatio * heightCurve.Evaluate(height));

    public static Vector2Int MeshToWorldPoint(Vector3 pos) {
        var x = Mathf.FloorToInt(pos.x);
        var y = GameController.World.height - Mathf.CeilToInt(pos.z) - 1;

        return new Vector2Int(x, y);
    }

    public static Vector3 WorldToMeshPoint(Vector2Int pos) {
        var x = pos.x + .5f;
        var y = (GameController.MapDisplay.GetHeight(pos.x, pos.y) +
                 GameController.MapDisplay.GetHeight(pos.x + 1, pos.y) +
                 GameController.MapDisplay.GetHeight(pos.x, pos.y + 1) +
                 GameController.MapDisplay.GetHeight(pos.x + 1, pos.y + 1)) / 4;
        var z = GameController.World.height - pos.y - 1.5f;

        return new Vector3(x, y, z);
    }
}
