using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public bool isControlled = true;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 9f;

    [Header("Ground Check")]
    public LayerMask groundMask;   
    public float groundCheckDistance = 0.05f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isControlled) return; 

        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }


    void FixedUpdate()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.51f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, ~0);

        isGrounded = hit.collider != null && hit.collider.CompareTag("Ground");
    }
}
