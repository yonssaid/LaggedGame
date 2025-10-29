using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName;

    [Header("SFX")]
    public AudioClip exitSfx;
    public float exitVolume = 1f;

    private SceneFader fader;
    private bool triggered;

    void Start()
    {
        fader = FindObjectOfType<SceneFader>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player") && fader != null)
        {
            triggered = true;
            if (exitSfx)
                AudioSource.PlayClipAtPoint(exitSfx, transform.position, exitVolume);

            if (!string.IsNullOrEmpty(nextSceneName))
                fader.FadeToScene(nextSceneName);
            else
                fader.FadeToNextScene();
        }
    }
}
