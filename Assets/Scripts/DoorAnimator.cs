using System.Collections;
using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    public float popScale = 1.15f; // scale multiplier
    public float popTime  = 0.10f; // animation duration

    public IEnumerator Pop()
    {
        Vector3 start = transform.localScale;       // original scale
        Vector3 target = start * popScale;          // enlarged target scale
        float t = 0f;

        while (t < popTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, target, t / popTime); // smooth scale up
            yield return null;
        }
        transform.localScale = start; // reset scale
    }
}
