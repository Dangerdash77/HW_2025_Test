using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 3f;

    [Tooltip("If player falls below this Y value, GameOver is triggered")]
    public float deathY = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (DiaryLoader.Diary != null && DiaryLoader.Diary.player_data != null)
            speed = DiaryLoader.Diary.player_data.speed;

    }

    void FixedUpdate()
    {
        if (rb == null) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h, 0f, v).normalized * speed;

        // use Rigidbody velocity assignment (same pattern you had)
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        // simple fall/death check
        if (transform.position.y <= deathY)
        {
            if (PulpitManager.Instance != null)
                PulpitManager.Instance.GameOver();
        }
    }
}
