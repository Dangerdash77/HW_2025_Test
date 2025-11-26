using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PulpitManager : MonoBehaviour
{
    public static PulpitManager Instance;

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

    public int score = 0;

    private List<GameObject> active = new List<GameObject>();
    private System.Random rng = new System.Random();
    private Vector3 lastPos;

    private bool isGameOver = false;

    [Header("Pulpit Effects")]
    public float scaleUpDuration = 0.15f; 

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lastPos = startPosition;

        if (GameOverPanel != null) GameOverPanel.SetActive(false);

        if (RestartButton != null)
        {
            RestartButton.onClick.RemoveAllListeners();
            RestartButton.onClick.AddListener(Restart);
            UnityEngine.Debug.Log("[PulpitManager] RestartButton wired.");
        }
        else
        {
            UnityEngine.Debug.Log("[PulpitManager] RestartButton NOT assigned in inspector.");
        }

        if (MainMenuButton != null)
        {
            MainMenuButton.onClick.RemoveAllListeners();
            MainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            UnityEngine.Debug.Log("[PulpitManager] MainMenuButton wired.");
        }
        else
        {
            UnityEngine.Debug.Log("[PulpitManager] MainMenuButton NOT assigned in inspector.");
        }

        SpawnInitial();
        UpdateScoreUI();
    }

    void OnMainMenuButtonClicked()
    {
        UnityEngine.Debug.Log("[PulpitManager] MainMenuButton clicked (UI event).");
        GoToMainMenu();
    }

    void Update()
    {
        if (isGameOver) return;

        active.RemoveAll(a => a == null);
        if (active.Count == 0) return;

        var oldest = active[0].GetComponent<Pulpit>();
        if (oldest == null) return;

        var pul = DiaryLoader.Diary.pulpit_data;
        float min = pul.min_pulpit_destroy_time;
        float max = pul.max_pulpit_destroy_time;
        float configuredX = pul.pulpit_spawn_time;

        float x = (configuredX <= 0f) ? RandomRange(min, max) : configuredX;

        if (oldest.RemainingTime() <= x)
        {
            if (active.Count < maxActive) SpawnAdjacent();
        }
    }

    void SpawnInitial()
    {
        var go = Instantiate(pulpitPrefab, startPosition, Quaternion.identity);
        SetupLifetime(go);
        SetupScaleUp(go);
        active.Add(go);
        lastPos = startPosition;
    }

    void SpawnAdjacent()
    {
        Vector3 pos = FindAdjacent(lastPos);
        var go = Instantiate(pulpitPrefab, pos, Quaternion.identity);
        SetupLifetime(go);
        SetupScaleUp(go);
        active.Add(go);
        lastPos = pos;

        while (active.Count > maxActive)
        {
            if (active[0] != null) Destroy(active[0]);
            active.RemoveAt(0);
        }
    }

    Vector3 FindAdjacent(Vector3 from)
    {
        int dir = rng.Next(0, 4);
        switch (dir)
        {
            case 0: return from + new Vector3(gridSpacing, 0, 0);
            case 1: return from + new Vector3(-gridSpacing, 0, 0);
            case 2: return from + new Vector3(0, 0, gridSpacing);
            default: return from + new Vector3(0, 0, -gridSpacing);
        }
    }

    void SetupLifetime(GameObject g)
    {
        var p = g.GetComponent<Pulpit>();
        if (p == null) return;

        var cfg = DiaryLoader.Diary.pulpit_data;
        p.lifetime = UnityEngine.Random.Range(cfg.min_pulpit_destroy_time, cfg.max_pulpit_destroy_time);
    }

    float RandomRange(float a, float b)
    {
        return (float)(a + this.rng.NextDouble() * (b - a));
    }

    // Score
    public void OnPulpitStepped(Pulpit pulpit)
    {
        if (isGameOver) return;

        score++;

        UpdateScoreUI();
        UnityEngine.Debug.Log("[PulpitManager] Score = " + score);
    }

    void UpdateScoreUI()
    {
        if (ScoreText != null)
            ScoreText.text = "Score: " + score;
    }

    public void OnPulpitDestroyed(Pulpit pulpit)
    {
        
    }

    // End Game
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

        // pause game
        Time.timeScale = 0f;

        UnityEngine.Debug.Log("[PulpitManager] GameOver - final score: " + score);
    }

    // Restart 
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        UnityEngine.Debug.Log("[PulpitManager] GoToMainMenu called. Trying to load scene: " + mainMenuName);

        if (UnityEngine.Application.CanStreamedLevelBeLoaded(mainMenuName))
        {
            UnityEngine.Debug.Log($"[PulpitManager] Found scene \"{mainMenuName}\" in Build Settings. Loading by name.");
            SceneManager.LoadScene(mainMenuName);
            return;
        }

    }

    void SetupScaleUp(GameObject g)
    {
        var p = g.GetComponent<Pulpit>();
        if (p == null) return;

        p.ScaleUpOnSpawn(scaleUpDuration);
    }
}
