using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

// Manages the main menu UI and scene navigation.
public class MainMenuManager : MonoBehaviour
{
    // Constants
    const string PrefMutedKey = "Doofus_Muted";

    // --- Inspector Fields ---

    [Header("Scene")]
    [Tooltip("Name of the gameplay scene to load when START is pressed.")]
    public string gameSceneName = "GameScene";

    [Header("UI")]
    public Button startButton;
    public Button volumeButton;
    public Button exitButton;

    [Tooltip("TMP text component inside the volume button to show 'Mute'/'Unmute' or 'Volume: On/Off'.")]
    public TextMeshProUGUI volumeButtonText;

    // UNITY LIFECYCLE METHODS

    void Awake()
    {
        // Wire up button click events
        if (startButton != null) startButton.onClick.AddListener(OnStartPressed);
        if (volumeButton != null) volumeButton.onClick.AddListener(OnVolumePressed);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitPressed);

        // Find Text Mesh for Volume button
        if (volumeButtonText == null && volumeButton != null)
        {
            var tmp = volumeButton.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) volumeButtonText = tmp;
        }

        // Set the initial volume button text based on saved preference
        ApplySavedVolumeState();
    }

    void OnDestroy()
    {
        if (startButton != null) startButton.onClick.RemoveListener(OnStartPressed);
        if (volumeButton != null) volumeButton.onClick.RemoveListener(OnVolumePressed);
        if (exitButton != null) exitButton.onClick.RemoveListener(OnExitPressed);
    }

    // PUBLIC EVENT HANDLERS

    public void OnStartPressed()
    {
        // Play click sound 
        if (AudioManagerExists())
            AudioManager.Instance.PlaySfx();

        if (string.IsNullOrEmpty(gameSceneName))
        {
            UnityEngine.Debug.LogError("[MainMenu] gameSceneName not set!");
            return;
        }

        Time.timeScale = 1f; // Ensure time is running before loading
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnVolumePressed()
    {
        // Option 1: AudioManager exists (standard scenario)
        if (AudioManagerExists())
        {
            AudioManager.Instance.ToggleMute();
            UpdateVolumeButtonText(AudioManager.Instance.IsMuted);
            AudioManager.Instance.PlaySfx();
            return;
        }

        // Option 2: Fallback (if AudioManager hasn't loaded yet)
        bool currentlyMuted = PlayerPrefs.GetInt(PrefMutedKey, 0) == 1;
        bool next = !currentlyMuted;
        PlayerPrefs.SetInt(PrefMutedKey, next ? 1 : 0);
        PlayerPrefs.Save();

        // Apply mute directly to the global listener
        AudioListener.volume = next ? 0f : 1f;
        UpdateVolumeButtonText(next);
    }

    public void OnExitPressed()
    {
        // Exit application (different method for Editor vs. Build)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // PRIVATE HELPER METHODS

    // Checks PlayerPrefs or AudioManager for the initial volume state.
    void ApplySavedVolumeState()
    {
        // If AudioManager is already active, use its state
        if (AudioManagerExists())
        {
            bool muted = AudioManager.Instance.IsMuted;
            UpdateVolumeButtonText(muted);
            // Sync PlayerPrefs with AudioManager's current state
            PlayerPrefs.SetInt(PrefMutedKey, muted ? 1 : 0);
            PlayerPrefs.Save();
            return;
        }

        // Fallback: Check PlayerPrefs directly and apply to AudioListener
        bool mutedPref = PlayerPrefs.GetInt(PrefMutedKey, 0) == 1;
        AudioListener.volume = mutedPref ? 0f : 1f;
        UpdateVolumeButtonText(mutedPref);
    }

    // Updates the text displayed on the volume button.
    void UpdateVolumeButtonText(bool muted)
    {
        if (volumeButtonText != null)
            volumeButtonText.text = muted ? "Volume: Off" : "Volume: On";
    }

    // Utility to safely check if the AudioManager instance exists.
    bool AudioManagerExists()
    {
        return (AudioManager.Instance != null);
    }
}