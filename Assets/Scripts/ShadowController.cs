using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShadowController : MonoBehaviour
{
    public MovementRecorder playerRecorder; 

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (playerRecorder == null) return;

        if (playerRecorder.TryGetDelayedState(out var state))
        {

            rb.MovePosition(state.position + new Vector3(0f, -10f, 0f)); 
        }
    }
}
