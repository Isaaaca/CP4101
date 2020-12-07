using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class testing : CharacterMovementController
{

    private void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)){
            Arc(Vector2.down, 1f, 2f, 1f, true, "linear","sine");
        }
        base.Update();
    }
   
}
