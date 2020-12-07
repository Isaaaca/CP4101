using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent/Single Sequence")]
public class SingleSequence : BaseGameEvent
{
    [SerializeField]private Sequence sequence = null;
    public override Sequence GetSequence()
    {
        return sequence;
    }
}
