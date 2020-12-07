using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwitchControllableObject : MonoBehaviour
{
    [SerializeField] protected float duration=1f;

    public virtual float GetDuration()
    {
        return duration;
    }

    public abstract void OnSwitch();
}
