using UnityEngine;
using TMPro;
using System.Collections;

// Controls the behavior of a single platform (pulpit), including timing and visual effects.
public class Pulpit : MonoBehaviour
{
    // Public Properties
    public bool IsActive { get; private set; } = true;

    // Private Fields
    float remaining; // The internal timer

    // --- Inspector Fields ---

    [Header("Platform Config")]
    [Tooltip("Lifetime in seconds (set by manager)")]
    public float lifetime = 5f; // Public field updated by manager for debugging
    public bool isStepped = false;

    [Header("Scaling Effects")]
    public float scaleDownDuration = 0.5f; // Time taken to shrink to zero
    //Size of Pulpit
    public Vector3 targetScale = new Vector3(9f, 1f, 9f);

    [Header("UI Timer")]
    // Timer Text placed on the pulpit
    public TextMeshPro TimerText;

    // UNITY LIFECYCLE METHODS

    void OnEnable()
    {
        // Reset state when the object is enabled
        IsActive = true;
    }

    void Update()
    {
        if (!IsActive) return;

        remaining -= Time.deltaTime;

        UpdateTimerText();

        // Start shrinking when remaining time hits the scaleDownDuration threshold
        if (remaining <= scaleDownDuration)
        {
            if (IsActive) // Ensures the destruction sequence is only initiated once
            {
                IsActive = false; // Disable to prevent further Update() calls
                StartCoroutine(ScaleAndDestroy());
            }
        }
    }

    // PUBLIC METHODS (API)

    // Used by PulpitManager to set the random starting time.
    public void SetInitialRemainingTime(float time)
    {
        this.remaining = time;
        UpdateTimerText();
    }

    public float RemainingTime() => remaining;

    // Starts the visual scale-up animation upon spawning.
    public void ScaleUpOnSpawn(float duration)
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp(duration));
    }

    // COROUTINES

    IEnumerator ScaleUp(float duration)
    {
        float timer = 0f;
        Vector3 endScale = targetScale;
        Vector3 startScale = Vector3.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // Animate scale from 0 to targetScale
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        transform.localScale = endScale;
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

            // Animate scale from current size to zero
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        transform.localScale = Vector3.zero;

        // Final step: destroy the object
        FinalCleanup();
    }

    // PRIVATE METHODS

    // Handles the final destruction of the GameObject. 
    void FinalCleanup()
    {
        Destroy(gameObject);
    }

    // Updates the Timer Text.
    void UpdateTimerText()
    {
        if (TimerText != null)
        {
            // Format time to two decimal places
            TimerText.text = remaining.ToString("F2");
        }
    }

    // --- Scoring Logic ---
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Only count the score if the player hasn't stepped on this platform yet
            if (!isStepped)
            {
                isStepped = true;
                PulpitManager.Instance.OnPulpitStepped(this);
            }
        }
    }
}