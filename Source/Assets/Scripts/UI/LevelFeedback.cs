using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class LevelFeedback : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI mainText = null;


    public abstract void LoadFeedback();


}
