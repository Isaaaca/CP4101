using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFillBarUI : FillBarUI
{
    [SerializeField] private GameObject character = null;


    private void Start()
    {
        meter = character.GetComponent<Character>().GetHealth();
    }
}
