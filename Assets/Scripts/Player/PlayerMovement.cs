using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStats))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStats stats;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(x, y).normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * stats.MoveSpeed;
    }
}