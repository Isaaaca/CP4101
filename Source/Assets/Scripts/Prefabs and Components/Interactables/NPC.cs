using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterMovementController))]
public class NPC : Interactable
{
    private CharacterMovementController controller;
    public enum ConversationType
    {
        Cycle,
        Onetime
    }

    [SerializeField]private ConversationType convoType = ConversationType.Cycle;
    [SerializeField]private Sequence[] sequences = new Sequence[1];
    private int currSeqIndex = -1;
    private bool initialFacingDir = false;

    private void Start()
    {
        controller = GetComponent<CharacterMovementController>();
        initialFacingDir = controller.IsFacingRight();
        CutsceneDirector.OnSequenceEnd += (Sequence) => { OnEndInteraction(); };
    }

    private void OnEndInteraction()
    {
        if (initialFacingDir != controller.IsFacingRight()) controller.Turn();
    }

    public override Sequence GetSequence()
    {
        return sequences[currSeqIndex];
    }

    protected override void OnInteract()
    {
        if (!controller.IsFacing(player.transform.position))
        {
            initialFacingDir = controller.IsFacingRight();
            controller.Turn();
        }
        if (currSeqIndex == -1) currSeqIndex = 0;
        else if (convoType == ConversationType.Cycle)
        {
            currSeqIndex = (currSeqIndex + 1) % sequences.Length;
        }
        else if (convoType == ConversationType.Onetime)
        {
            currSeqIndex += currSeqIndex < (sequences.Length - 1) ? 1 : 0;
        }
        base.OnInteract();
    }
}
