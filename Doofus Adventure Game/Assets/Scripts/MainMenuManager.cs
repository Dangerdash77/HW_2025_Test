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
        if (AudioManagerExists())
        {
            bool muted = AudioManager.Instance.IsMuted;
            UpdateVolumeButtonText(muted);
            PlayerPrefs.SetInt(PrefMutedKey, muted ? 1 : 0);
            PlayerPrefs.Save();
            return;
        }

        bool mutedPref = PlayerPrefs.GetInt(PrefMutedKey, 0) == 1;
        AudioListener.volume = mutedPref ? 0f : 1f;
        UpdateVolumeButtonText(mutedPref);
    }

    void UpdateVolumeButtonText(bool muted)
    {
        if (volumeButtonText != null)
            volumeButtonText.text = muted ? "Volume: Off" : "Volume: On";
    }

    bool AudioManagerExists()
    {
        return (AudioManager.Instance != null);
    }

    public void OnStartPressed()
    {
        if (AudioManagerExists())
            AudioManager.Instance.PlaySfx(); 

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
        if (AudioManagerExists())
        {
            AudioManager.Instance.ToggleMute();
            UpdateVolumeButtonText(AudioManager.Instance.IsMuted);
            AudioManager.Instance.PlaySfx();
            return;
        }

        bool currentlyMuted = PlayerPrefs.GetInt(PrefMutedKey, 0) == 1;
        bool next = !currentlyMuted;
        PlayerPrefs.SetInt(PrefMutedKey, next ? 1 : 0);
        PlayerPrefs.Save();

        AudioListener.volume = next ? 0f : 1f;
        UpdateVolumeButtonText(next);
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
