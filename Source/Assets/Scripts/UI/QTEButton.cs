using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEButton : MonoBehaviour
{
    private Image image;
    private Color originalColor;
    // Start is called before the first frame update

    private void OnEnable()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        
    }

    public void Fade()
    {
        image.color -= Color.white * 0.2f;
    }

    public void Refresh()
    {
        image.color = originalColor;
    }
}
