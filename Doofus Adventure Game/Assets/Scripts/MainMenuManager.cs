using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Name of the gameplay scene to load when START is pressed.")]
    public string gameSceneName = "GameScene";

    [Header("UI")]
    public Button startButton;
    public Button volumeButton;
    public Button exitButton;

    [Tooltip("TMP text component inside the volume button to show 'Mute'/'Unmute' or 'Volume: On/Off'.")]
    public TextMeshProUGUI volumeButtonText;

    const string PrefMutedKey = "Doofus_Muted";

    void Awake()
    {
        if (startButton != null) startButton.onClick.AddListener(OnStartPressed);
        if (volumeButton != null) volumeButton.onClick.AddListener(OnVolumePressed);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitPressed);

        if (volumeButtonText == null && volumeButton != null)
        {
            var tmp = volumeButton.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) volumeButtonText = tmp;
        }

        ApplySavedVolumeState();
    }

    void OnDestroy()
    {
        if (startButton != null) startButton.onClick.RemoveListener(OnStartPressed);
        if (volumeButton != null) volumeButton.onClick.RemoveListener(OnVolumePressed);
        if (exitButton != null) exitButton.onClick.RemoveListener(OnExitPressed);
    }

    void ApplySavedVolumeState()
    {
        bool muted = PlayerPrefs.GetInt(PrefMutedKey, 0) == 1;
        SetMuted(muted, initial: true);
    }

    void SetMuted(bool muted, bool initial = false)
    {
        AudioListener.volume = muted ? 0f : 1f;

        if (volumeButtonText != null)
            volumeButtonText.text = muted ? "Volume: Off" : "Volume: On";

        if (!initial)
            PlayerPrefs.SetInt(PrefMutedKey, muted ? 1 : 0);
    }

    public void OnStartPressed()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            UnityEngine.Debug.LogError("[MainMenu] gameSceneName not set!");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnVolumePressed()
    {
        bool currentlyMuted = PlayerPrefs.GetInt(PrefMutedKey, 0) == 1;
        bool next = !currentlyMuted;
        SetMuted(next, initial: false);
    }

    public void OnExitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
