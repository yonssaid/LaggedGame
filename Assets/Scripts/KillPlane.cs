using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlane : MonoBehaviour
{
    [Header("References")]
    public Transform player; // player transform
    public Transform shadow; // shadow transform

    [Header("Settings")]
    public float minY = -30f; // death height

    [Header("SFX")]
    public AudioClip deathSfx; // death sound
    public float deathVolume = 1f;

    private bool reloading; // prevent double reloads

    void Update()
    {
        if (reloading) return;

        // auto reset if either falls below minY
        if (player.position.y < minY || shadow.position.y < minY)
            TriggerDeath();

        // manual reset
        if (Input.GetKeyDown(KeyCode.R))
            TriggerDeath();
    }

    void TriggerDeath()
    {
        if (reloading) return;
        reloading = true;

        if (deathSfx)
            AudioSource.PlayClipAtPoint(deathSfx, transform.position, deathVolume);

        Invoke(nameof(RestartLevel), 0.25f); // small delay for sfx
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload scene
    }
}
