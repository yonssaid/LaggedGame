using UnityEngine;
using System.Collections.Generic;

public struct PlayerState
{
    public Vector3 position;
}

public class MovementRecorder : MonoBehaviour
{
    public float delay = 1f; 
    private readonly List<PlayerState> history = new List<PlayerState>();
    private int framesToDelay;

    void Start()
    {
        framesToDelay = Mathf.CeilToInt(delay / Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        history.Insert(0, new PlayerState { position = transform.position });


        int max = framesToDelay + 10;
        if (history.Count > max)
            history.RemoveAt(history.Count - 1);
    }

    public bool TryGetDelayedState(out PlayerState state)
    {
        int idx = framesToDelay;
        if (history.Count > idx)
        {
            state = history[idx];
            return true;
        }
        state = default;
        return false;
    }

    public void ClearHistory()
    {
        history.Clear();
    }
}
