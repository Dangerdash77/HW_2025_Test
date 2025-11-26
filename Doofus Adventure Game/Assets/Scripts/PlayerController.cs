using UnityEngine;

// Ensures a Rigidbody component exists on this GameObject.
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // Private Fields
    private Rigidbody rb;
    private float speed = 3f;

    // --- Inspector Fields ---

    [Tooltip("If player falls below this Y value, GameOver is triggered")]
    public float deathY = -10f;

    // UNITY LIFECYCLE METHODS

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Load speed from the configuration diary if available
        if (DiaryLoader.Diary != null && DiaryLoader.Diary.player_data != null)
            speed = DiaryLoader.Diary.player_data.speed;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Get player input (raw ensures only -1, 0, or 1)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Calculate movement vector: Horizontal (X) and Vertical (Z, forward/back)
        Vector3 movement = new Vector3(h, 0f, v).normalized * speed;

        // Apply movement using Rigidbody's linearVelocity property
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        // Death check
        if (transform.position.y <= deathY)
        {
            // Trigger game over state if the player falls too far
            if (PulpitManager.Instance != null)
                PulpitManager.Instance.GameOver();
        }
    }
}