using System.Diagnostics;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Clips")]
    [Tooltip("Background music (looping)")]
    public AudioClip backgroundMusic;

    [Tooltip("Optional small click SFX (assign if you have one). If not assigned, PlaySfx() does nothing.")]
    public AudioClip uiClick;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource musicSource;

    private const string MuteKey = "Doofus_Muted";
    public bool IsMuted { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;

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

        ApplyMute();
    }

    public void ToggleMute()
    {
        SetMuted(!IsMuted);
    }

    public void SetMuted(bool mute)
    {
        IsMuted = mute;
        PlayerPrefs.SetInt(MuteKey, mute ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMute();
        UnityEngine.Debug.Log("[AudioManager] Muted = " + IsMuted);
    }

    public void ApplyMute()
    {
        musicSource.volume = IsMuted ? 0f : musicVolume;
        AudioListener.volume = IsMuted ? 0f : 1f;
    }

    public void PlaySfx(AudioClip clip = null, float volume = 1f)
    {
        if (IsMuted) return;

        AudioClip toPlay = clip != null ? clip : uiClick;
        if (toPlay == null) return;

        Vector3 pos = Vector3.zero;
        if (Camera.main != null) pos = Camera.main.transform.position;
        AudioSource.PlayClipAtPoint(toPlay, pos, Mathf.Clamp01(volume * sfxVolume));
    }

    public void PlayMusic(AudioClip clip, bool playImmediately = true)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        if (playImmediately && !IsMuted) musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
