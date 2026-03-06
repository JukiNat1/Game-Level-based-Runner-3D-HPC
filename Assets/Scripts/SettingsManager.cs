using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages Settings (background music and SFX volume).
/// Attach directly to the Settings Panel in each scene.
/// Synchronized via PlayerPrefs — all scenes use the same keys.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Value display text (optional)")]
    public Text musicValueText;
    public Text sfxValueText;

    private const string MUSIC_VOL_KEY = "musicVolume";
    private const string SFX_VOL_KEY   = "sfxVolume";

    /// <summary>
    /// Runs every time the panel is enabled.
    /// Reads values from PlayerPrefs to stay in sync across scenes.
    /// </summary>
    void OnEnable()
    {
        float savedMusic = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1f);
        float savedSFX   = PlayerPrefs.GetFloat(SFX_VOL_KEY,   1f);

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        UpdateValueTexts(savedMusic, savedSFX);

        // Apply volume immediately when panel is enabled
        ApplyMusicVolume(savedMusic);
        ApplySFXVolume(savedSFX);
    }

    public void OnMusicVolumeChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, value);
        PlayerPrefs.Save();
        if (musicValueText != null)
            musicValueText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void OnSFXVolumeChanged(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, value);
        PlayerPrefs.Save();
        if (sfxValueText != null)
            sfxValueText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void ApplyMusicVolume(float value)
    {
        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null) am.SetMusicVolume(value);
    }

    private void ApplySFXVolume(float value)
    {
        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null) am.SetSFXVolume(value);
    }

    private void UpdateValueTexts(float music, float sfx)
    {
        if (musicValueText != null)
            musicValueText.text = Mathf.RoundToInt(music * 100) + "%";
        if (sfxValueText != null)
            sfxValueText.text = Mathf.RoundToInt(sfx * 100) + "%";
    }
}
