using System.Diagnostics;
using UnityEngine;

// This class manages all music and sound effects, ensuring only one instance exists.
public class AudioManager : MonoBehaviour
{
    // Singleton Instance property
    public static AudioManager Instance { get; private set; }

    // Constants
    private const string MuteKey = "Doofus_Muted";

    // Public Properties
    public bool IsMuted { get; private set; }

    // Private Fields
    private AudioSource musicSource;

    // --- Inspector Fields ---

    [Header("Clips")]
    [Tooltip("Background music (looping)")]
    public AudioClip backgroundMusic;

    [Tooltip("Optional small click SFX (assign if you have one). If not assigned, PlaySfx() does nothing.")]
    public AudioClip uiClick;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // UNITY LIFECYCLE METHODS

    void Awake()
    {
        // Enforce Singleton pattern: destroy duplicate instances
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Keep the Audio Manager alive across scene loads
        DontDestroyOnLoad(gameObject);

        // Setup the AudioSource for background music
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;

        // Load the saved mute state from PlayerPrefs
        IsMuted = PlayerPrefs.GetInt(MuteKey, 0) == 1;
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            if (!IsMuted)
                musicSource.Play();
        }

        // Apply mute settings loaded in Awake
        ApplyMute();
    }

    // PUBLIC METHODS (API)

    // Plays an SFX clip as a one-shot at the camera's position.
    public void PlaySfx(AudioClip clip = null, float volume = 1f)
    {
        if (IsMuted) return;

        AudioClip toPlay = clip != null ? clip : uiClick;
        if (toPlay == null) return;

        // Get the current main camera position to play the sound
        Vector3 pos = Vector3.zero;
        if (Camera.main != null) pos = Camera.main.transform.position;

        // Play the clip, respecting SFX volume settings(Default set)
        AudioSource.PlayClipAtPoint(toPlay, pos, Mathf.Clamp01(volume * sfxVolume));
    }

    // Changes the background music clip and optionally plays it immediately.
    public void PlayMusic(AudioClip clip, bool playImmediately = true)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        if (playImmediately && !IsMuted) musicSource.Play();
    }

    // Stops the currently playing background music.
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Flips the current mute state (Muted -> Unmuted, Unmuted -> Muted).
    public void ToggleMute()
    {
        SetMuted(!IsMuted);
    }

    // Sets the mute state and saves it to PlayerPrefs.
    public void SetMuted(bool mute)
    {
        IsMuted = mute;
        PlayerPrefs.SetInt(MuteKey, mute ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMute();
        UnityEngine.Debug.Log("[AudioManager] Muted = " + IsMuted);
    }

    // PRIVATE METHODS

    // Applies the current mute state to the AudioSource and the global AudioListener.
    public void ApplyMute()
    {
        // Mute/Unmute the background music source
        musicSource.volume = IsMuted ? 0f : musicVolume;
        // Mute/Unmute all sounds globally (Music and SFX)
        AudioListener.volume = IsMuted ? 0f : 1f;
    }
}