using UnityEngine;

public class Pulpit : MonoBehaviour
{
    [Tooltip("Lifetime in seconds (set by manager)")]
    public float lifetime = 5f;
    public bool isStepped = false;
    float remaining;
    public bool IsActive { get; private set; } = true;

    void OnEnable()
    {
        remaining = lifetime;
        IsActive = true;
    }

    void Update()
    {
        if (!IsActive) return;
        remaining -= Time.deltaTime;
        if (remaining <= 0f) DestroyPulpit();
    }

    public float RemainingTime() => remaining;

    void DestroyPulpit()
    {
        IsActive = false;
        Destroy(gameObject);
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
}
