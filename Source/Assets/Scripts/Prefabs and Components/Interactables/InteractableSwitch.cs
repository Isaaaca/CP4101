using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSwitch : Interactable
{
    private Sequence sequence = null;
    [SerializeField] SwitchControllableObject controlledObject = null;
    [SerializeField] private Animator animator = null;
    public void Awake()
    {
        //create sequence
        sequence = ScriptableObject.CreateInstance<Sequence>();
        sequence.scriptedEvents = new SequenceEvent[2];
        SequenceEvent  seqEvent = new SequenceEvent
        {
            eventType = SequenceEvent.EventType.CameraPanTo,
            targetTransform = controlledObject.transform
        };
        sequence.scriptedEvents[0] = seqEvent;
        
        seqEvent = new SequenceEvent
        {
            eventType = SequenceEvent.EventType.Switchable,
            switchable = controlledObject
        };
       
        sequence.scriptedEvents[1] = seqEvent;
        sequence.name = name+"seq";

    }
    protected override void OnInteract()
    {
        animator.SetTrigger("Switch");
        base.OnInteract();
    }

    public override Sequence GetSequence()
    {
        return sequence;
    }

}
