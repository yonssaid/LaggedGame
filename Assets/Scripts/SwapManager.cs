using UnityEngine;

public class SwapManager : MonoBehaviour
{
    public Transform player;
    public Transform shadow;
    public MovementRecorder recorder; 

    public KeyCode swapKey = KeyCode.LeftShift;


    public LayerMask solidMask;
    public float checkRadius = 0.35f;

    void Update()
    {
        if (Input.GetKeyDown(swapKey))
        {
            TrySwap();
        }
    }

    void TrySwap()
    {
        Vector3 playerDest = shadow.position;
        Vector3 shadowDest = player.position;


        bool blocked = Physics2D.OverlapCircle(playerDest, checkRadius, solidMask);
        if (blocked) return;

        // Do swap
        Vector3 temp = player.position;
        player.position = playerDest;
        shadow.position = shadowDest;

        recorder.ClearHistory();
    }
}
