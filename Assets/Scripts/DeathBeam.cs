using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathBeam : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip deathSfx;  // sound on death
    public float deathVolume = 1f;

    private bool reloading; // prevents multiple reloads

    void OnTriggerEnter2D(Collider2D other)
    {
        if (reloading) return;

        // reload when player touches beam
        if (other.CompareTag("Player"))
        {
            reloading = true;
            if (deathSfx)
                AudioSource.PlayClipAtPoint(deathSfx, transform.position, deathVolume);

            Invoke(nameof(ReloadScene), 0.25f); // delay for sfx
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload current scene
    }
}
