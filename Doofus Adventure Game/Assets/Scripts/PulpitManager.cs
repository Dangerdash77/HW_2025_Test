using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class PulpitManager : MonoBehaviour
{
    public static PulpitManager Instance;   // scoring callback

    public GameObject pulpitPrefab;
    public int maxActive = 2;
    public float gridSpacing = 9f;
    public Vector3 startPosition = Vector3.zero;

    // UI
    public TextMeshProUGUI ScoreText;       // assign ScoreText
    public GameObject GameOverPanel;        // assign GameOver panel (disabled by default)
    public TextMeshProUGUI FinalScoreText;  // assign TMP text inside GameOverPanel (optional)
    public Button RestartButton;

    // ⭐ LEVEL 2 score
    public int score = 0;

    private List<GameObject> active = new List<GameObject>();
    private System.Random rng = new System.Random();
    private Vector3 lastPos;

    private bool isGameOver = false;

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
        }
        SpawnInitial();
        UpdateScoreUI();
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
        active.Add(go);
        lastPos = startPosition;
    }

    void SpawnAdjacent()
    {
        Vector3 pos = FindAdjacent(lastPos);
        var go = Instantiate(pulpitPrefab, pos, Quaternion.identity);
        SetupLifetime(go);
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
        Vector3 offset = Vector3.zero;
        switch (dir)
        {
            case 0: offset = new Vector3(gridSpacing, 0, 0); break;
            case 1: offset = new Vector3(-gridSpacing, 0, 0); break;
            case 2: offset = new Vector3(0, 0, gridSpacing); break;
            default: offset = new Vector3(0, 0, -gridSpacing); break;
        }
        return from + offset;
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

    // Hook for pulpit destroyed (kept minimal)
    public void OnPulpitDestroyed(Pulpit pulpit)
    {
        // reserved if you want bookkeeping later
    }

    // ----------------- End Game -----------------
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // show Game Over UI
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

    // Restart (hook to UI button)
    public void Restart()
    {
        // unpause then reload
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
