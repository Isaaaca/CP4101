using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent/Branching Sequence")]
public class BranchingSequence : BaseGameEvent
{
    [SerializeField] private SequenceDictionary sequenceDictionary = new SequenceDictionary();

    public override Sequence GetSequence()
    {
        foreach (string s in sequenceDictionary.Keys)
        {
            if (SaveManager.CheckCondition(s))
                return sequenceDictionary[s];
        }
        return null;
    }
}
