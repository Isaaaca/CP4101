using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public static event Action<string> OnEnterAreaTrigger = (eventCode) => { };

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnterAreaTrigger(gameObject.name);
    }
}
