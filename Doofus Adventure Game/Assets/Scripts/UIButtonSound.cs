using UnityEngine;
using UnityEngine.UI;

//Button component Required
[RequireComponent(typeof(Button))]

public class UIButtonSound : MonoBehaviour
{
    // --- Inspector Fields ---
    public AudioClip overrideClip;
    Button btn;

    void OnClicked()
    {
        // Play the custom clip
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(overrideClip);
    }
}