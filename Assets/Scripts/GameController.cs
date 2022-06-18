using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameController : MonoBehaviour {
    private static GameController instance;
    public static GameController Instance => instance ? instance : instance = FindObjectOfType<GameController>();

    [SerializeField] private bool randomSeed;

    [SerializeField] private int seed;

    [Header("World Settings"), SerializeField]
    private bool startAutoUpdate;

    [SerializeField, Range(0.01f, 1)] private float secondsPerStep;
    [HideInInspector] public bool autoUpdateRunning;

    [SerializeField] private WorldParameters worldParameters;
    public static WorldParameters WorldParameters => Instance.worldParameters;

    [SerializeField] private List<WorldParameters> presets;
    public static List<WorldParameters> Presets => instance.presets;

    public static World World { get; private set; }

    public static Location Location { get; private set; }

    public static Race[] Races {
        get {
            if (DatabaseManager.races == null || DatabaseManager.races.Length == 0) DatabaseManager.LoadDatabase();

            return DatabaseManager.races;
        }
    }

    public static Climate[] Climates {
        get {
            if (DatabaseManager.climates == null || DatabaseManager.climates.Length == 0)
                DatabaseManager.LoadDatabase();

            return DatabaseManager.climates;
        }
    }

    private static MapDisplay mapDisplay;
    public static MapDisplay MapDisplay => mapDisplay ? mapDisplay : mapDisplay = Instance.GetComponent<MapDisplay>();

    private static WorldGenUI worldGenUI;
    public static WorldGenUI WorldGenUI => worldGenUI ? worldGenUI : worldGenUI = Instance.GetComponent<WorldGenUI>();

    private static WorldLogUI worldLogUI;

    private static WorldLogUI WorldLogUI =>
        worldLogUI ? worldLogUI : worldLogUI = Instance.GetComponent<WorldLogUI>();

    private static WorldGenUtility worldGenUtility;

    public static WorldGenUtility WorldGenUtility => worldGenUtility
        ? worldGenUtility
        : worldGenUtility = Instance.GetComponent<WorldGenUtility>();

    private static WorldCamera worldCamera;

    public static WorldCamera WorldCamera =>
        worldCamera ? worldCamera : worldCamera = FindObjectOfType<WorldCamera>();

    private static DialogueManager dialogueManager;

    public static DialogueManager DialogueManager =>
        dialogueManager ? dialogueManager : dialogueManager = FindObjectOfType<DialogueManager>();

    private static DatabaseManager databaseManager;

    private static DatabaseManager DatabaseManager => databaseManager
        ? databaseManager
        : databaseManager = Instance.GetComponent<DatabaseManager>();

    private static AsyncOperation loadingLevel;

    private void Awake() {
        DontDestroyOnLoad(this);

        GenerateWorld();
    }

    [UsedImplicitly]
    public void OnWorldPresetChanged(int i) {
        worldParameters = presets[i];
        GenerateWorld();
    }

    [Button(ButtonSizes.Medium)]
    public void GenerateWorld() {
        StopAutoUpdate();

        if (randomSeed)
            seed = UnityEngine.Random.Range(0, 9999999);

        World = new World(worldParameters, seed);
        World.Generate();

        if (startAutoUpdate) StartAutoUpdate();

        OnWorldUpdated(true);
    }

    private static bool WorldReady() {
        return World != null;
    }

    [Button(ButtonSizes.Medium), ShowIf(nameof(WorldReady))]
    public void UpdateWorld() {
        World.Update();
        OnWorldUpdated(false);
    }

    private void OnWorldUpdated(bool reset) {
        MapDisplay.DrawMap(reset);
        WorldGenUI.OnMapChanged();

        WorldLogUI.UpdateLog();

        if (reset) {
            WorldCamera.Set(new Vector3(World.width / 2, World.LongSide, World.height / 2));
        }
    }

    private void StartAutoUpdate() {
        if (!Application.isPlaying) return;

        StopAutoUpdate();
        StartCoroutine(nameof(AutoUpdate));
        autoUpdateRunning = true;
    }

    private void StopAutoUpdate() {
        StopCoroutine(nameof(AutoUpdate));
        autoUpdateRunning = false;
    }

    private IEnumerator AutoUpdate() {
        while (World != null) {
            yield return new WaitForSeconds(secondsPerStep);
            UpdateWorld();
        }
    }

    public void LoadLocation(Location location) {
        StartCoroutine(LoadLocationCoroutine(location));
    }

    private IEnumerator LoadLocationCoroutine(Location location) {
        if (loadingLevel != null && !loadingLevel.isDone) yield break;

        Location = location;
        loadingLevel = SceneManager.LoadSceneAsync("Local", LoadSceneMode.Single);

        while (!loadingLevel.isDone) {
            yield return null;
        }
    }
}
