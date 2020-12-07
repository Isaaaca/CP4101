using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventFeedback : LevelFeedback
{

    [SerializeField] FeedbackDictionary feedback = null;
    [TextArea]
    [SerializeField] string initialString = "";
    public override void LoadFeedback()
    {
        mainText.text = initialString;
        foreach(string condition in feedback.Keys)
        {
            if (SaveManager.CheckCondition(condition))
            {
                mainText.text += feedback[condition];
            }
        }
    }
}
