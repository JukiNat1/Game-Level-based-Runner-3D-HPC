using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public Sound[] sounds;
    private static bool isSoundTurnedOn = true;
    public Button soundOnOffBtn;
    public Sprite soundOnImg;
    public Sprite soundOffImg;

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
        {
            PauseAudio();
        }           
        else
        {
            PlaySound("MainTheme");
        }
    }

    public void PlaySound(string name)
    {
        if (isSoundTurnedOn)
        {
            sounds.ToList()
                  .Find(x => x.name.Equals(name))
                  .source.Play();
        }
    }

    public void ChangeMainThemeVolume(float newVolume)
    {
        SetMusicVolume(newVolume);
    }

    /// <summary>
    /// Sets background music volume (sounds where isLooped = true).
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        foreach (var s in sounds)
        {
            if (s.source != null && s.isLooped)
                s.source.volume = volume;
        }
    }

    /// <summary>
    /// Sets SFX volume (sounds where isLooped = false).
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        foreach (var s in sounds)
        {
            if (s.source != null && !s.isLooped)
                s.source.volume = volume;
        }
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
        {
            audioSource.Stop();
        }
    }

    private void PauseAudio()
    {
        gameObject.GetComponent<AudioSource>().Pause();
        if (soundOnOffBtn != null)
            soundOnOffBtn.GetComponent<Image>().sprite = soundOffImg;
    }

    private void ResumeAudio()
    {
        gameObject.GetComponent<AudioSource>().Play();
        if (soundOnOffBtn != null)
            soundOnOffBtn.GetComponent<Image>().sprite = soundOnImg;
    }

    public void ToggleSoundPlaying()
    {
        if (isSoundTurnedOn)
        {
            PauseAudio();           
        } 
        else
        {
            ResumeAudio();
        }

        isSoundTurnedOn = !isSoundTurnedOn;
    }
}
