using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public Sound[] sounds;

    // Sound state – persisted to PlayerPrefs so it survives scene transitions
    private bool isSoundTurnedOn = true;
    private const string SOUND_KEY = "soundEnabled";

    // Inspector reference (kept for backward compat with PauseManager.soundOnOffBtn)
    public Button soundOnOffBtn;
    public Sprite soundOnImg;
    public Sprite soundOffImg;

    // All currently registered buttons (each scene can register its own button)
    private readonly List<Button> registeredButtons = new List<Button>();

    void Awake()
    {
        // Singleton: keep only one instance across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Restore sound state from PlayerPrefs
        isSoundTurnedOn = PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;
    }

    void Start()
    {
        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.isLooped;
            s.source.name = s.name;
            s.source.volume = s.volume;
        }

        // Load volume settings from PlayerPrefs
        float savedMusic = PlayerPrefs.GetFloat("musicVolume", 1f);
        float savedSFX   = PlayerPrefs.GetFloat("sfxVolume",   1f);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        if (!isSoundTurnedOn)
            PauseAudio();
        else
            PlaySound("MainTheme");
    }

    /// <summary>
    /// Call this from any scene's SoundOnOffBtn (via SoundButtonRegistrar).
    /// Keeps the icon in sync with the current sound state immediately.
    /// Cleans up destroyed (null) buttons automatically.
    /// </summary>
    public void RegisterSoundButton(Button btn)
    {
        if (btn == null) return;

        // Remove any destroyed references
        registeredButtons.RemoveAll(b => b == null);

        if (!registeredButtons.Contains(btn))
            registeredButtons.Add(btn);

        // Also keep the legacy single reference in sync
        soundOnOffBtn = btn;

        // Sync icon immediately
        SyncButtonIcon(btn);
    }

    // ─── Audio Playback ────────────────────────────────────────────────────────

    public void PlaySound(string name)
    {
        var s = System.Array.Find(sounds, x => x.name.Equals(name));
        if (s != null && s.source != null)
            s.source.Play();
    }

    public void ChangeMainThemeVolume(float newVolume) => SetMusicVolume(newVolume);

    /// <summary>Sets background music volume (sounds where isLooped = true).</summary>
    public void SetMusicVolume(float volume)
    {
        foreach (var s in sounds)
            if (s.source != null && s.isLooped)
                s.source.volume = volume;
    }

    /// <summary>Sets SFX volume (sounds where isLooped = false).</summary>
    public void SetSFXVolume(float volume)
    {
        foreach (var s in sounds)
            if (s.source != null && !s.isLooped)
                s.source.volume = volume;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime, float endLevel)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > endLevel)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }

        if (endLevel == 0.0f)
            audioSource.Stop();
    }

    // ─── Toggle ────────────────────────────────────────────────────────────────

    public void ToggleSoundPlaying()
    {
        if (isSoundTurnedOn)
            PauseAudio();
        else
            ResumeAudio();

        isSoundTurnedOn = !isSoundTurnedOn;

        // Persist state so the next scene loads with the correct state
        PlayerPrefs.SetInt(SOUND_KEY, isSoundTurnedOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ─── Private helpers ───────────────────────────────────────────────────────

    private void PauseAudio()
    {
        AudioListener.volume = 0f;
        UpdateAllButtonIcons(isSoundOn: false);
    }

    private void ResumeAudio()
    {
        AudioListener.volume = 1f;
        UpdateAllButtonIcons(isSoundOn: true);
    }

    /// <summary>Updates icon on ALL registered buttons (one per active scene).</summary>
    private void UpdateAllButtonIcons(bool isSoundOn)
    {
        registeredButtons.RemoveAll(b => b == null);
        foreach (var btn in registeredButtons)
            UpdateButtonIcon(btn, isSoundOn);
    }

    private void SyncButtonIcon(Button btn)
    {
        if (btn == null) return;
        UpdateButtonIcon(btn, isSoundTurnedOn);
    }

    private void UpdateButtonIcon(Button btn, bool isSoundOn)
    {
        if (btn == null) return;
        var img = btn.GetComponent<Image>();
        if (img != null)
            img.sprite = isSoundOn ? soundOnImg : soundOffImg;
    }
}
