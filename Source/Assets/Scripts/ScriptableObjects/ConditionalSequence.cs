using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent/Conditional Sequence")]
public class ConditionalSequence : BaseGameEvent
{
    [SerializeField] private Sequence sequence = null;
    [SerializeField] private string[] condition = null;
    public override Sequence GetSequence()
    {
        foreach (string con in condition)
        {
            if (!SaveManager.CheckCondition(con))
                return null;
        }

        return sequence;
        /*if (SaveManager.CheckCondition(condition))
            return sequence;
        else
            return null;*/
    }
}
