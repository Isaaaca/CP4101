using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudio : MonoBehaviour, ISelectHandler, ISubmitHandler// required interface when using the OnSelect method.
{

    [SerializeField] private AudioClip selectAudioClip = null;
    [SerializeField] private AudioClip pressAudioClip = null;

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        if (selectAudioClip != null) AudioManager.PlayClip(selectAudioClip);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (pressAudioClip != null) AudioManager.PlayClip(pressAudioClip);
    }
}
