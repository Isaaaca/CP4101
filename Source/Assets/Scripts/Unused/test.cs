using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class test: MonoBehaviour
{
    public SequenceDictionary sd;
    public DialogueManager dm;
    public Dialogue d;
    public ScreenFader SF;
    public CutsceneDirector cd;
    public Sequence sequence;
    public Door door;
    private AudioSource audioSource;
    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void poop()
    {
        Debug.Log("poop");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9)) SF.FadeToBlack();
        if (Input.GetKeyDown(KeyCode.Alpha0)) SF.FadeIn();
        if (Input.GetKeyDown(KeyCode.Alpha8)) cd.PlaySequence(sequence);
        if (Input.GetKeyDown(KeyCode.Alpha7)) door.OnSwitch();
        if (Input.GetKeyDown(KeyCode.Alpha5)) audioSource.PlayOneShot(audioSource.clip);
    }
    void OnDrawGizmosSelected()
    {
        float topBorder=1;
        Bounds bounds = new Bounds
        {
            size = Vector3.zero // reset
        };
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            bounds.Encapsulate(col.bounds);
        }
        bounds.center += Vector3.up * (topBorder/2);
        bounds.size += Vector3.up * topBorder;
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
}
