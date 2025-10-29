using UnityEngine;

public enum Activator { Player, Shadow, Either } // defines who can trigger

[RequireComponent(typeof(Collider2D))]
public class Switch : MonoBehaviour
{
    public PuzzleManager puzzleManager; // reference to puzzle system
    public int doorId = 0;              // target door index

    [Header("Who can press this?")]
    public Activator activator = Activator.Shadow; // allowed activator

    [Header("Visual feedback")]
    public Color pressedTint = new Color(0.6f, 0.6f, 0.6f, 1f); // tint on press

    [Header("SFX")]
    public AudioClip pressSfx; // press sound

    bool hasTinted;            // prevents repeat tinting
    SpriteRenderer sr;
    Color baseColor;
    AudioSource audioSrc;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true; // make it a trigger zone

        if (!puzzleManager) puzzleManager = FindObjectOfType<PuzzleManager>();

        sr = GetComponent<SpriteRenderer>();
        if (sr) baseColor = sr.color;

        audioSrc = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    bool IsValidActivator(Collider2D other)
    {
        if (activator == Activator.Either)  return other.CompareTag("Player") || other.CompareTag("Shadow");
        if (activator == Activator.Player)  return other.CompareTag("Player");
                                            return other.CompareTag("Shadow");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidActivator(other)) return;

        // visual + audio feedback
        if (!hasTinted)
        {
            hasTinted = true;
            if (sr) sr.color = pressedTint;
            if (pressSfx) audioSrc.PlayOneShot(pressSfx);
        }

        puzzleManager?.Activate(doorId); // trigger puzzle logic
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsValidActivator(other)) return;
        puzzleManager?.Deactivate(doorId); // release trigger
    }
}
