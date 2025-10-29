using UnityEngine;

public class ControlSwitcher : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;        // main character
    public PlayerController shadow;        // shadow character
    public KeyCode swapKey = KeyCode.LeftShift; // swap input
    public GameObject shadowAura;          // aura when swaps available
    public GameObject switchMessage;       // UI hint

    [Header("SFX")]
    public AudioClip swapSfx;              // sound on swap
    public AudioClip noOrbSfx;             // sound when blocked

    private AudioSource audioSrc;
    private bool controllingPlayer = true; // who has control now
    private Color shadowBaseColor;         // original shadow tint

    private bool hasOrb = false;           // any swaps available
    private int remainingSwaps = 0;        // swaps left from current orb
    private bool swapSfxPlayedThisOrb = false; // play swap sfx once per orb

    void Start()
    {
        player.isControlled = true;
        shadow.isControlled = false;

        var sR = shadow.GetComponent<SpriteRenderer>();
        shadowBaseColor = sR ? sR.color : Color.gray;

        audioSrc = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (shadowAura) shadowAura.SetActive(false);
        if (switchMessage) switchMessage.SetActive(false);

        UpdateTint(); // refresh visuals
    }

    void Update()
    {
        if (Input.GetKeyDown(swapKey))
            TrySwap();
    }

    public void GainOrb()
    {
        hasOrb = true;
        remainingSwaps = 2;            // two swaps per orb
        swapSfxPlayedThisOrb = false;
        if (shadowAura) shadowAura.SetActive(true);
        if (switchMessage) switchMessage.SetActive(true);
        Debug.Log("Switch Orb collected: 2 swaps ready.");
    }

    void TrySwap()
    {
        if (!hasOrb)
        {
            if (noOrbSfx) audioSrc.PlayOneShot(noOrbSfx);
            Debug.Log("No orb: cannot swap.");
            return;
        }

        if (remainingSwaps <= 0)
        {
            hasOrb = false; // clear orb UI/state
            if (shadowAura) shadowAura.SetActive(false);
            if (switchMessage) switchMessage.SetActive(false);
            swapSfxPlayedThisOrb = false;
            Debug.Log("Orb consumed: no swaps left.");
            return;
        }

        // toggle control
        controllingPlayer = !controllingPlayer;
        player.isControlled = controllingPlayer;
        shadow.isControlled = !controllingPlayer;
        UpdateTint();

        if (!swapSfxPlayedThisOrb && swapSfx)
        {
            audioSrc.PlayOneShot(swapSfx);
            swapSfxPlayedThisOrb = true;
        }

        remainingSwaps--;

        // consume orb when out of swaps
        if (remainingSwaps == 0)
        {
            hasOrb = false;
            if (shadowAura) shadowAura.SetActive(false);
            if (switchMessage) switchMessage.SetActive(false);
            swapSfxPlayedThisOrb = false;
            Debug.Log("Orb fully used up.");
        }
    }

    void UpdateTint()
    {
        var pR = player.GetComponent<SpriteRenderer>();
        var sR = shadow.GetComponent<SpriteRenderer>();

        // player dims when not controlled; shadow keeps base tint
        if (pR) pR.color = controllingPlayer ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        if (sR) sR.color = shadowBaseColor;
    }
}
