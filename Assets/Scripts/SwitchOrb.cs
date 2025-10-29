using UnityEngine;

public class SwitchOrb : MonoBehaviour
{
    [Header("Visuals & Audio")]
    public GameObject pickupBurst; // effect on pickup
    public AudioClip pickupSfx;    // sound on pickup

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // notify control switcher
            ControlSwitcher cs = FindFirstObjectByType<ControlSwitcher>();
            if (cs) cs.GainOrb();

            // spawn visual effect
            if (pickupBurst)
                Instantiate(pickupBurst, transform.position, Quaternion.identity);

            // play sound
            if (pickupSfx)
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position);

            // remove orb
            Destroy(gameObject);
        }
    }
}
