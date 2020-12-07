using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LustFillBarUI : FillBarUI
{
    // Start is called before the first frame update
    [SerializeField] private GameObject character = null;


    private void Start()
    {
        meter = character.GetComponent<PlayerController>().GetLust();
    }
}
