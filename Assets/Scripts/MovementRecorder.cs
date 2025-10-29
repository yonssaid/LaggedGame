using UnityEngine;
using System.Collections.Generic;

public struct PlayerState
{
    public float inputX; // horizontal input
    public bool jump;    // jump input flag
}

public class MovementRecorder : MonoBehaviour
{
    public float delay = 0.7f; // time delay in seconds
    public List<PlayerState> history = new List<PlayerState>(); // recorded states

    private int framesToDelay; // delay in frames
    private PlayerController pc; // reference to player controller

    void Awake()
    {
        pc = GetComponent<PlayerController>(); // cache player controller
    }

    void Start()
    {
        framesToDelay = Mathf.CeilToInt(delay / Time.fixedDeltaTime); // convert delay to frames
    }

    void FixedUpdate()
    {
        // record current input each physics tick
        history.Insert(0, new PlayerState
        {
            inputX = pc != null ? pc.inputX : 0f,
            jump = pc != null && pc.ConsumeJumpFlag()
        });

        // trim history to prevent overflow
        int max = framesToDelay + 10;
        if (history.Count > max)
            history.RemoveAt(history.Count - 1);
    }

    public bool TryGetDelayedState(out PlayerState state)
    {
        // get input state from "delay" frames ago
        int idx = framesToDelay;
        if (history.Count > idx)
        {
            state = history[idx];
            return true;
        }

        state = default;
        return false;
    }

    public void ClearHistory() => history.Clear(); // reset history
}
