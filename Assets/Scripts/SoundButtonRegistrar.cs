using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this script to every SoundOnOffBtn in every scene.
/// It automatically registers itself with AudioManager on Start,
/// so the button always stays in sync regardless of which scene loads first.
/// </summary>
[RequireComponent(typeof(Button))]
public class SoundButtonRegistrar : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();

        if (AudioManager.Instance != null && btn != null)
        {
            AudioManager.Instance.RegisterSoundButton(btn);

            // Auto-setup the click event so users don't have to manually link it in the inspector
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(AudioManager.Instance.ToggleSoundPlaying);
        }
    }
}
