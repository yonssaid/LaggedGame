using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LightBeam : MonoBehaviour
{
    [Header("Buzz Settings")]
    public AudioClip buzzSfx;        // ambient hum
    public float maxVolume = 1f;     // volume near shadow
    public float minVolume = 0.05f;  // volume far away
    public float maxDistance = 10f;  // distance for min volume

    [Header("Death Settings")]
    public AudioClip deathSfx;       // sound on contact
    public float deathVolume = 1f;

    private Transform shadow;        // tracked shadow
    private AudioSource audioSrc;    // looping buzz
    private bool reloading;          // prevents multiple reloads

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        audioSrc.loop = true;
        audioSrc.playOnAwake = false;
        audioSrc.spatialBlend = 0f;

        // start buzzing sound
        if (buzzSfx)
        {
            audioSrc.clip = buzzSfx;
            audioSrc.Play();
        }

        // find shadow reference
        GameObject shadowObj = GameObject.FindGameObjectWithTag("Shadow");
        if (shadowObj) shadow = shadowObj.transform;
    }

    void Update()
    {
        if (!shadow) return;

        // adjust buzz volume based on distance
        float distance = Vector2.Distance(transform.position, shadow.position);
        float t = Mathf.Clamp01(distance / maxDistance);
        float targetVolume = Mathf.Lerp(maxVolume, minVolume, t);
        audioSrc.volume = targetVolume;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (reloading) return;

        // kill shadow on contact
        if (other.CompareTag("Shadow"))
        {
            reloading = true;
            if (deathSfx)
                AudioSource.PlayClipAtPoint(deathSfx, transform.position, deathVolume);

            Invoke(nameof(ReloadScene), 0.25f); // small delay for sfx
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload scene
    }
}
