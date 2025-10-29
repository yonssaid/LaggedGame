using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public bool isControlled = true; // allow direct input

    [HideInInspector] public float inputX;              // horizontal input
    [HideInInspector] public bool jumpPressedThisFrame; // jump edge trigger

    [Header("Movement")]
    public float moveSpeed = 12f;   // horizontal speed
    public float jumpForce = 18f;   // jump impulse

    [Header("Ground Check")]
    public LayerMask groundMask;        // layers considered ground
    public float groundCheckDistance = 0.05f; // ray length

    [Header("SFX")]
    public AudioClip jumpSfx;      // jump sound
    public AudioClip footstepSfx;  // looped walk sound

    private AudioSource audioSrc;  // footsteps loop
    private AudioSource sfxSrc;    // one-shots
    private float footstepTimer;   // reserved
    private Rigidbody2D rb;
    private bool isGrounded;

    // animation
    private Animator anim;
    private SpriteRenderer sr;
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimJump = Animator.StringToHash("Jump");
    private static readonly int AnimGrounded = Animator.StringToHash("isGrounded");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // audio setup
        audioSrc = GetComponent<AudioSource>();
        if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = false;

        sfxSrc = gameObject.AddComponent<AudioSource>();
        sfxSrc.playOnAwake = false;
        sfxSrc.loop = false;

        // animator and sprite
        anim = GetComponent<Animator>();
        if (!anim) anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!isControlled) return; // AI or replay controls when false

        // read input and move
        inputX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        // face move direction
        if (sr && Mathf.Abs(inputX) > 0.01f) sr.flipX = inputX < 0f;

        // jump on press while grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpPressedThisFrame = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (anim) anim.SetTrigger(AnimJump);
            if (jumpSfx) sfxSrc.PlayOneShot(jumpSfx);
        }

        // drive run blend
        if (anim) anim.SetFloat(AnimSpeed, Mathf.Abs(rb.linearVelocity.x));
    }

    public bool ConsumeJumpFlag()
    {
        bool wasPressed = jumpPressedThisFrame; // one-frame pulse
        jumpPressedThisFrame = false;
        return wasPressed;
    }

    void FixedUpdate()
    {
        // ground check from box bottom
        var box = GetComponent<BoxCollider2D>();
        Vector2 origin = (Vector2)box.bounds.center + Vector2.down * (box.bounds.extents.y - 0.01f);
        int mask = groundMask.value == 0 ? ~0 : groundMask.value;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, mask);
        isGrounded = hit.collider != null;

        if (anim) anim.SetBool(AnimGrounded, isGrounded);

        // footsteps loop when moving on ground
        if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.2f)
        {
            if (footstepSfx && (!audioSrc.isPlaying || audioSrc.clip != footstepSfx))
            {
                audioSrc.clip = footstepSfx;
                audioSrc.loop = true;
                audioSrc.Play();
            }
        }
        else if (audioSrc.clip == footstepSfx && audioSrc.isPlaying)
        {
            audioSrc.Stop();
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // visualize ground check ray
        Gizmos.color = Color.yellow;
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.51f;
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
    }
#endif
}
