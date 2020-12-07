using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCounterFeedback : LevelFeedback
{
    [SerializeField] string counterCode = "";
    public override void LoadFeedback()
    {
        mainText.text = SaveManager.GetCounter(counterCode).ToString("N0");
        
    }
}
