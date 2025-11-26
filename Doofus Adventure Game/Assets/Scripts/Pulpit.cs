using UnityEngine;
using TMPro;
using System.Collections;

public class Pulpit : MonoBehaviour
{
    [Tooltip("Lifetime in seconds (set by manager)")]
    public float lifetime = 5f;
    public bool isStepped = false;
    public float scaleDownDuration = 0.15f; 

    [Header("UI Timer")]
    public TextMeshPro TimerText;

    float remaining;
    public bool IsActive { get; private set; } = true;

    void OnEnable()
    {
        remaining = lifetime;
        IsActive = true;
        UpdateTimerText();
    }

    void Update()
    {
        if (!IsActive) return;

        remaining -= Time.deltaTime;

        UpdateTimerText();

        if (remaining <= 0.15f) DestroyPulpit();
    }

    void UpdateTimerText()
    {
        if (TimerText != null)
        {
            TimerText.text = remaining.ToString("F2");
        }
    }

    public float RemainingTime() => remaining;

    void DestroyPulpit()
    {
        if (remaining <= 0.15f)
        {
            StartCoroutine(ScaleAndDestroy());
            IsActive = false; 
        }
    }

    void FinalCleanup() 
    {
        Destroy(gameObject);
    }

    IEnumerator ScaleAndDestroy()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float timer = 0f;
        Vector3 startScale = transform.localScale;

        while (timer < scaleDownDuration)
        {
            timer += Time.deltaTime;
            float t = timer / scaleDownDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null; 
        }

        transform.localScale = Vector3.zero;

        FinalCleanup();
    }

    //Score
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isStepped)
            {
                isStepped = true;
                PulpitManager.Instance.OnPulpitStepped(this);
            }
        }
    }

    public Vector3 targetScale = new Vector3(9f, 1f, 9f);

    public void ScaleUpOnSpawn(float duration)
    {
        transform.localScale = Vector3.zero;

        StartCoroutine(ScaleUp(duration));
    }

    IEnumerator ScaleUp(float duration)
    {
        float timer = 0f;
        Vector3 endScale = targetScale;
        Vector3 startScale = Vector3.zero; 

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration; 

            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null; 
        }

        transform.localScale = endScale;
    }

}
