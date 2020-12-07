using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservableObject : Interactable
{


    [SerializeField]private Sequence sequence = null;
    public override Sequence GetSequence()
    {
        return sequence;
    }

}
