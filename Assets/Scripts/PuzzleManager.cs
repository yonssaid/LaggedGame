using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public enum DoorMode { ToggleOpen, HoldToOpen } // door behavior types (hold to open unused)

    [System.Serializable]
    public class DoorEntry
    {
        public GameObject door;                  // door root object
        public DoorMode mode = DoorMode.ToggleOpen;

        [HideInInspector] public int activeCount;    // active triggers for HoldToOpen
        [HideInInspector] public int requestVersion; // cancels in-flight opens

        public void SetClosed(bool closed)
        {
            if (!door) return;
            var col = door.GetComponent<Collider2D>();
            var sr  = door.GetComponent<SpriteRenderer>();
            if (col) col.enabled = closed; // solid when closed
            if (sr)  sr.enabled = closed;  // visible when closed
        }
    }

    [Header("Doors (order = ids: 0,1,2,...)")]
    public List<DoorEntry> doors = new List<DoorEntry>(); // indexed door list

    [Header("SFX")]
    public AudioClip doorPopSfx; // sound on open

    private AudioSource audioSrc; // local audio player

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>(); // ensure source
    }

    void Start()
    {
        // start with all doors closed
        for (int i = 0; i < doors.Count; i++)
            doors[i].SetClosed(true);
    }

    bool Valid(int id) => id >= 0 && id < doors.Count; // id bounds check

    public void Activate(int id)
    {
        if (!Valid(id)) { Debug.LogError($"[PuzzleManager] Bad door id {id}"); return; }
        var d = doors[id];

        d.requestVersion++; // invalidate pending opens

        if (d.mode == DoorMode.HoldToOpen)
        {
            d.activeCount++; // track active hold
            StartCoroutine(OpenAfterDelay(id, 0.5f, d.requestVersion));
        }
        else // ToggleOpen
        {
            StartCoroutine(OpenAfterDelay(id, 0.5f, d.requestVersion));
        }
    }

    public void Deactivate(int id)
    {
        if (!Valid(id)) return;
        var d = doors[id];

        if (d.mode == DoorMode.HoldToOpen)
        {
            d.activeCount = Mathf.Max(0, d.activeCount - 1); // decrement hold
            d.requestVersion++; // cancel pending open if any
            if (d.activeCount == 0) d.SetClosed(true); // close when no holds
        }
        // ToggleOpen ignores release
    }

    IEnumerator OpenAfterDelay(int doorIndex, float delaySeconds, int version)
    {
        yield return new WaitForSeconds(delaySeconds);

        var entry = doors[doorIndex];

        if (entry.requestVersion != version) yield break; // canceled during wait
        if (entry.mode == DoorMode.HoldToOpen && entry.activeCount <= 0) yield break; // no longer active

        // optional pop animation
        if (entry.door && entry.door.TryGetComponent(out DoorAnimator anim))
            yield return StartCoroutine(anim.Pop());

        entry.SetClosed(false); // open door

        if (doorPopSfx) audioSrc.PlayOneShot(doorPopSfx); // play sfx
    }
}
