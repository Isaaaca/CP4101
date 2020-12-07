using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]private AudioSource audioSource = null;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public static void PlayClip(AudioClip clip)
    {
        instance.audioSource.PlayOneShot(clip);
    }
    
    public static void PlayClip(AudioClip clip, float volume)
    {
        instance.audioSource.PlayOneShot(clip, volume);
    }


    public float GetVolume()
    {
        return audioSource.volume;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }
}
