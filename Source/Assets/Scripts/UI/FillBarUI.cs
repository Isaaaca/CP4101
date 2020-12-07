using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FillBarUI : MonoBehaviour
{

    [SerializeField] private Image bar = null;

    protected Meter meter=null;

    private void Update()
    {
        bar.fillAmount = meter.GetNormalised();
    }
}
