using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 3f;

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

        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }
}
