using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public AudioClip overrideClip;
    Button btn;
    void OnClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(overrideClip);
    }
}