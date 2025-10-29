using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShadowController : MonoBehaviour
{
    public MovementRecorder playerRecorder;   // source of delayed inputs
    public PlayerController shadowPC;         // ensures this isn't player-controlled

    private Rigidbody2D rb;

    [Header("Shadow movement tuning (match PlayerController)")]
    public float moveSpeed = 12f;     // max horizontal speed
    public float groundAccel = 60f;   // accel on ground
    public float airAccel = 30f;      // accel in air
    public float jumpForce = 18f;     // jump impulse

    public float groundCheckDistance = 0.05f; // ray length for ground check
    public LayerMask groundMask = ~0;         // ground layers
    private bool isGrounded;

    // Animation
    private Animator anim;
    private SpriteRenderer sr;
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimJump = Animator.StringToHash("Jump");
    private static readonly int AnimGrounded = Animator.StringToHash("isGrounded");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (shadowPC == null) shadowPC = GetComponent<PlayerController>(); // optional guard

        anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>(); // find animator
        sr = GetComponentInChildren<SpriteRenderer>(); // sprite for flips
    }

    void FixedUpdate()
    {
        // skip if this object is actively player-controlled
        if (shadowPC != null && shadowPC.isControlled) return;

        // ground check via short downward ray
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.51f;
        int mask = groundMask.value == 0 ? ~0 : groundMask.value;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, mask);
        isGrounded = hit.collider != null;

        if (anim) anim.SetBool(AnimGrounded, isGrounded); // update grounded flag

        if (playerRecorder != null && playerRecorder.TryGetDelayedState(out var s))
        {
            // horizontal move towards recorded input
            float targetVx = s.inputX * moveSpeed;
            float accel = isGrounded ? groundAccel : airAccel;
            float newVx = Mathf.MoveTowards(rb.linearVelocity.x, targetVx, accel * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newVx, rb.linearVelocity.y);

            // face movement direction when input present
            if (sr && Mathf.Abs(s.inputX) > 0.01f) sr.flipX = s.inputX < 0f;

            // drive blend tree with actual speed
            if (anim) anim.SetFloat(AnimSpeed, Mathf.Abs(newVx));

            // perform recorded jump when grounded
            if (s.jump && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (anim) anim.SetTrigger(AnimJump);
            }
        }
        else
        {
            // no input yet: idle speed for animator
            if (anim) anim.SetFloat(AnimSpeed, 0f);
        }
    }
}
