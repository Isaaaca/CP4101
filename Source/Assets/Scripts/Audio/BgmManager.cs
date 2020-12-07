using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;

    [SerializeField] protected AudioClip defaultBGM;
    [SerializeField] private AudioSource[] sources = new AudioSource[2];
    [SerializeField] private float defaultDuration = 3;
    [Range(0, 1)]
    public float maxVolume =1;
    private float duration = 2;

    private bool fadingIn = false;
    private bool crossfading = false;
    private bool fadingOut = false;
    private int currSource = 0;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.CrossFade(defaultBGM, defaultDuration);
            DontDestroyOnLoad(instance);
        }
        else
        {
            instance.defaultBGM = this.defaultBGM;
            instance.FadeToDefault(defaultDuration);
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if (fadingIn)
        {
            sources[currSource].volume = Mathf.Clamp(sources[currSource].volume + Time.deltaTime / duration, 0, maxVolume);
            if (sources[currSource].volume == maxVolume) fadingIn = false;
        }
        else if (fadingOut)
        {
            sources[currSource].volume = Mathf.Clamp(sources[currSource].volume - Time.deltaTime / duration, 0, maxVolume);
            if (sources[currSource].volume == 0)
            {
                sources[currSource].Stop();
                fadingOut = false;
            } 
        }
        else if (crossfading)
        {
            int altSource = 1 - currSource;
            sources[currSource].volume = Mathf.Clamp(sources[currSource].volume + Time.deltaTime / duration, 0, maxVolume);
            sources[altSource].volume = Mathf.Clamp(sources[altSource].volume - Time.deltaTime / duration, 0, maxVolume);
            if(sources[currSource].volume == maxVolume 
                && sources[altSource].volume == 0)
            {
                sources[altSource].Stop();
                crossfading = false;
            }
        }
    }

    public void FadeToDefault()
    {
        FadeToDefault(defaultDuration);
    }
    public void FadeToDefault(float duration)
    {
        if (sources[currSource].clip == null || defaultBGM==null || sources[currSource].clip.name != defaultBGM.name)
        {
            CrossFade(defaultBGM, duration);
        }
        else if (sources[currSource].volume != maxVolume)
        {
            FadeIn(duration);
        }
    }

    public void CrossFade(AudioClip clip)
    {
        CrossFade(clip, defaultDuration);
    }

    public void CrossFade(AudioClip clip, float duration)
    {
        OffAllToggles();
        currSource = 1 - currSource;
        sources[currSource].clip = clip;
        sources[currSource].volume = 0;
        sources[currSource].Play();
        this.duration = duration;
        crossfading = true;
    }
    public void FadeIn()
    {
        FadeIn(defaultDuration);

    }

    public void FadeIn(float duration)
    {
        OffAllToggles();
        this.duration = duration;
        sources[currSource].Play();
        fadingIn = true;

    }

    public void FadeOut()
    {
        FadeOut(defaultDuration);

    }

    public void FadeOut(float duration)
    {
        OffAllToggles();
        this.duration = duration;
        fadingOut = true;

    }

    private void OffAllToggles()
    {
        fadingIn = fadingOut = crossfading = false;
    }

    public float GetVolume()
    {
        return maxVolume;
    }


    public void SetVolume(float volume)
    {
        maxVolume = Mathf.Clamp01(volume); ;
        if (sources[currSource].isPlaying)
            sources[currSource].volume = maxVolume;
    }
}
