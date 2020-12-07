using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePlayer : MonoBehaviour
{
    [SerializeField] private AudioDictionary audioDictionary = null;
    [SerializeField] private bool falloff = false;
    [SerializeField] private float minDis = 10;
    [SerializeField] private float maxDis = 20;

    public void PlayAudio(string clipName)
    {
        AudioClip clip = audioDictionary[clipName];
        if (clip != null)
        {
            if (falloff)
            {
                float distance = (Camera.main.transform.position - this.transform.position).magnitude;
                float volume = Mathf.Clamp01(-Mathf.Log10((distance - minDis) / (maxDis - minDis)));
                AudioManager.PlayClip(clip, volume);
            }
            else
            {
                AudioManager.PlayClip(clip);
            }
        }
        else
        {
            Debug.LogWarning("Audio clip \"" + clipName + "\" not found.");
        }
    }
}
