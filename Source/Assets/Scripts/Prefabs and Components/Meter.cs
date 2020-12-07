using System;
using UnityEngine;

[Serializable]
public class Meter
{
    [SerializeField] protected float max = 100;
    [SerializeField] protected float current = 100;

    public float GetNormalised()
    {
        return current / max;
    }
    public float Get()
    {
        return current;
    } 
    
    public float GetMax()
    {
        return max;
    }

    public float Modify(float amt)
    {
        current = Mathf.Clamp(current + amt, 0, max);
        return current;
    }

    public void SetMax(float newMax)
    {
        max = newMax;
    }

    public void Set(float amt)
    {
        current = amt;
    }
}
