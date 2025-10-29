using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage; // overlay image for fading
    public float fadeDuration = 0.5f; // fade speed

    bool isFading; // prevents overlapping fades

    void Start()
    {
        // start by fading in from black
        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
            StartCoroutine(Fade(1f, 0f));
        }
    }

    public void FadeToNextScene()
    {
        if (isFading) return; // skip if already fading
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next >= SceneManager.sceneCountInBuildSettings) next = 0; // loop to first scene
        StartCoroutine(FadeOutAndLoadIndex(next));
    }

    public void FadeToScene(string sceneName)
    {
        if (isFading || string.IsNullOrEmpty(sceneName)) return; // validate input
        StartCoroutine(FadeOutAndLoadName(sceneName));
    }

    IEnumerator FadeOutAndLoadIndex(int buildIndex)
    {
        isFading = true;
        yield return StartCoroutine(Fade(0f, 1f)); // fade to black
        yield return SceneManager.LoadSceneAsync(buildIndex);
    }

    IEnumerator FadeOutAndLoadName(string sceneName)
    {
        isFading = true;
        yield return StartCoroutine(Fade(0f, 1f)); // fade to black
        yield return SceneManager.LoadSceneAsync(sceneName);
    }

    IEnumerator Fade(float from, float to)
    {
        if (!fadeImage) yield break;

        float t = 0f;
        var c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration); // interpolate alpha
            fadeImage.color = c;
            yield return null;
        }
        c.a = to; // finalize alpha
        fadeImage.color = c;
    }
}
