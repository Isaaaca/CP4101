using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDirector : MonoBehaviour
{
    public static event Action<Sequence> OnSequenceEnd = (senquence) => { };

    [SerializeField]private ScreenFader screen = null;
    [SerializeField]private DialogueManager dialogueManager = null;
    [SerializeField]private CamController camController = null;
    [SerializeField]private PlayerController player = null;
    private int index = -1;
    private Sequence currentSequence = null;
    private SequenceEvent.EventType currEventType = SequenceEvent.EventType.Null;
    private SequenceEvent currEvent = null;
    private bool waiting = true;

    private void OnDestroy()
    {
        DialogueManager.OnDialogueStartEnd -= HandleDialogueEvent;
    }
    private void Awake()
    {
        DialogueManager.OnDialogueStartEnd += HandleDialogueEvent;
    }



    // Update is called once per frame
    void Update()
    {
        if (!waiting)
        {
            if (index >= currentSequence.scriptedEvents.Length)
            {
                //End Sequence
                waiting = true;
                OnSequenceEnd(currentSequence);
            }
            else
            {
                currEvent = currentSequence.scriptedEvents[index];
                currEventType = currEvent.eventType;
                GameObject targetObject = null;
                switch (currEventType)
                {
                    case SequenceEvent.EventType.Dialogue:
                        dialogueManager.LoadDialogue(currEvent.dialogue);
                        waiting = true;
                        break;
                    case SequenceEvent.EventType.Fade:
                        screen.CustomFade(currEvent.opacity, currEvent.duration);
                        waiting = true;
                        break;
                    case SequenceEvent.EventType.Switchable:
                        if (currEvent.switchable == null)
                        {
                            targetObject = GameObject.Find(currEvent.target);
                            if (targetObject != null)
                                currEvent.switchable = targetObject.GetComponent<SwitchControllableObject>();

                        }
                        if (currEvent.switchable != null)
                        {
                            currEvent.switchable.OnSwitch();
                            waiting = true;
                            Invoke("StopWaiting", currEvent.switchable.GetDuration());
                        }
                        break;
                    case SequenceEvent.EventType.CameraJumpTo:
                        if (currEvent.targetTransform == null)
                        {
                            targetObject = GameObject.Find(currEvent.target);
                            if (targetObject != null)
                            {
                                currEvent.targetTransform = targetObject.transform;

                            }
                        }
                        if (currEvent.targetTransform != null)
                        {
                            camController.JumpToTarget(currEvent.targetTransform);
                            StopWaiting();
                        }
                        break;
                    case SequenceEvent.EventType.CameraPanTo:
                        if (currEvent.targetTransform == null)
                        {
                            targetObject = GameObject.Find(currEvent.target);
                            if (targetObject != null)
                            {
                                currEvent.targetTransform = targetObject.transform;

                            }
                        }
                        if (currEvent.targetTransform != null)
                        {
                            camController.PanToTarget(currEvent.targetTransform);
                            waiting = true;
                            Invoke("StopWaiting", 1f);
                        }
                        break;
                    case SequenceEvent.EventType.Teleport:
                        player.transform.position = currEvent.position;
                        StopWaiting();
                        break;
                    case SequenceEvent.EventType.TeleportRelative:
                        player.transform.position += (Vector3)currEvent.position;
                        StopWaiting();
                        break;
                    case SequenceEvent.EventType.Turn:
                        targetObject = GameObject.Find(currEvent.target);
                        if (targetObject != null)
                        {
                            CharacterMovementController controller = targetObject.GetComponent<CharacterMovementController>();
                            if (controller != null) controller.Turn();

                        }
                        StopWaiting();
                        break;
                    case SequenceEvent.EventType.Pause:
                        waiting = true;
                        Invoke("StopWaiting", currEvent.duration);
                        break;
                }
            }

        }
        else if (currEventType == SequenceEvent.EventType.Fade
            && index < currentSequence.scriptedEvents.Length)
        {
            if (!screen.isTransitioning())
            {
                StopWaiting();
            }
        }
    }

    public void PlaySequence(Sequence eventSequence)
    {
        currentSequence = eventSequence;
        index = -1;
        StopWaiting();
    }

    private void HandleDialogueEvent(bool start)
    {
        if (currEventType == SequenceEvent.EventType.Dialogue && !start)
        {
            StopWaiting();
        }
    }

    public void CutSequence()
    {
        dialogueManager.CloseDialogue();
        index = int.MaxValue;
        waiting = true;
    }

    private void StopWaiting()
    {
        waiting = false;
        index++;

    }
}
