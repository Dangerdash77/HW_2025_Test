using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Main game manager responsible for spawning platforms, tracking score, and managing game state.
public class PulpitManager : MonoBehaviour
{
    // Singleton Instance property
    public static PulpitManager Instance;

    // Public Score
    public int score = 0;

    // Private Fields
    private List<GameObject> active = new List<GameObject>();
    private System.Random rng = new System.Random();
    private Vector3 lastPos;
    private bool isGameOver = false;

    // --- Inspector Fields ---

    [Header("Spawner")]
    public GameObject pulpitPrefab;
    public int maxActive = 2;
    public float gridSpacing = 9f;
    public Vector3 startPosition = Vector3.zero;

    [Header("UI")]
    public TextMeshProUGUI ScoreText;
    public GameObject GameOverPanel;
    public TextMeshProUGUI FinalScoreText;
    public Button RestartButton;
    public Button MainMenuButton;

    [Header("Settings")]
    public string mainMenuName = "MainMenu";

    [Header("Pulpit Effects")]
    public float scaleUpDuration = 0.5f;

    // UNITY LIFECYCLE METHODS

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lastPos = startPosition;

        if (GameOverPanel != null) GameOverPanel.SetActive(false);

        // Wire up UI buttons
        SetupUIButtons();

        SpawnInitial();
        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // Destroyed platform
        active.RemoveAll(a => a == null);
        if (active.Count == 0) return;

        // Get the oldest active platform (the first one spawned)
        var oldest = active[0].GetComponent<Pulpit>();
        if (oldest == null) return;

        // Load configuration data for spawning logic
        var pul = DiaryLoader.Diary.pulpit_data;
        float min = pul.min_pulpit_destroy_time;
        float max = pul.max_pulpit_destroy_time;
        float configuredX = pul.pulpit_spawn_time;
        float x = (configuredX <= 0f) ? RandomRange(min, max) : configuredX;

        // Check if the oldest platform is nearing its destruction time
        if (oldest.RemainingTime() <= x)
        {
            // Spawn a new platform if we are below the active platform limit
            if (active.Count < maxActive) SpawnAdjacent();
        }
    }

    // PUBLIC GAME LOGIC

    // Called by a Pulpit when the player steps on it for the first time.
    public void OnPulpitStepped(Pulpit pulpit)
    {
        if (isGameOver) return;
        score++;
        UpdateScoreUI();
        UnityEngine.Debug.Log("[PulpitManager] Score = " + score);
    }

    // Triggers the Game Over state.
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
            if (FinalScoreText != null)
                FinalScoreText.text = "Score: " + score;
        }

        // Pause game time
        Time.timeScale = 0f;

        UnityEngine.Debug.Log("[PulpitManager] GameOver - final score: " + score);
    }

    // PUBLIC UI HANDLERS

    // Restarts the current game scene.
    public void Restart()
    {
        Time.timeScale = 1f; // Unpause time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Exit to main menu scene.
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Unpause time

        UnityEngine.Debug.Log("[PulpitManager] GoToMainMenu called. Trying to load scene: " + mainMenuName);

        if (UnityEngine.Application.CanStreamedLevelBeLoaded(mainMenuName))
        {
            UnityEngine.Debug.Log($"[PulpitManager] Found scene \"{mainMenuName}\" in Build Settings. Loading by name.");
            SceneManager.LoadScene(mainMenuName);
        }
    }

    // PRIVATE SPAWNING/SETUP

    // Spawn the very first platform.
    void SpawnInitial()
    {
        var go = Instantiate(pulpitPrefab, startPosition, Quaternion.identity);
        SetupLifetime(go);
        SetupScaleUp(go);
        active.Add(go);
        lastPos = startPosition;
    }

    // Spawns a new platform adjacent to the last one.
    void SpawnAdjacent()
    {
        Vector3 pos = FindAdjacent(lastPos);
        var go = Instantiate(pulpitPrefab, pos, Quaternion.identity);
        SetupLifetime(go);
        SetupScaleUp(go);
        active.Add(go);
        lastPos = pos;

        // Clean up oldest platforms if maxActive limit is exceeded
        while (active.Count > maxActive)
        {
            if (active[0] != null) Destroy(active[0]);
            active.RemoveAt(0);
        }
    }

    // Determines a random position adjacent to the given position (X/Z grid).
    Vector3 FindAdjacent(Vector3 from)
    {
        // 0=Right (+X), 1=Left (-X), 2=Forward (+Z), 3=Back (-Z)
        int dir = rng.Next(0, 4);
        switch (dir)
        {
            case 0: return from + new Vector3(gridSpacing, 0, 0);
            case 1: return from + new Vector3(-gridSpacing, 0, 0);
            case 2: return from + new Vector3(0, 0, gridSpacing);
            default: return from + new Vector3(0, 0, -gridSpacing);
        }
    }

    // Calculates a random lifetime and assigns it to the new platform.
    void SetupLifetime(GameObject g)
    {
        var p = g.GetComponent<Pulpit>();
        if (p == null) return;

        var cfg = DiaryLoader.Diary.pulpit_data;

        float randomLifetime = UnityEngine.Random.Range(cfg.min_pulpit_destroy_time, cfg.max_pulpit_destroy_time);
        p.lifetime = randomLifetime;
        p.SetInitialRemainingTime(randomLifetime);
    }

    // Initiates the scale-up visual effect on the spawned platform.
    void SetupScaleUp(GameObject g)
    {
        var p = g.GetComponent<Pulpit>();
        if (p == null) return;

        p.ScaleUpOnSpawn(scaleUpDuration);
    }

    // UI event handler for the Main Menu button.
    void OnMainMenuButtonClicked()
    {
        UnityEngine.Debug.Log("[PulpitManager] MainMenuButton clicked (UI event).");
        GoToMainMenu();
    }

    // Helper to set up button listeners.
    void SetupUIButtons()
    {
        if (RestartButton != null)
        {
            RestartButton.onClick.RemoveAllListeners();
            RestartButton.onClick.AddListener(Restart);
            UnityEngine.Debug.Log("[PulpitManager] RestartButton wired.");
        }

        if (MainMenuButton != null)
        {
            MainMenuButton.onClick.RemoveAllListeners();
            MainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            UnityEngine.Debug.Log("[PulpitManager] MainMenuButton wired.");
        }
    }

    // Helper for calculating a random float value.
    float RandomRange(float a, float b)
    {
        return (float)(a + this.rng.NextDouble() * (b - a));
    }

    // Updates the score display text.
    void UpdateScoreUI()
    {
        if (ScoreText != null)
            ScoreText.text = "Score: " + score;
    }
}