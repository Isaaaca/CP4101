using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float defaultDuration =1;
    [SerializeField] private float targetOpacity = 0;

    private float duration =1;
    private Image image;
    private bool inTransition = false;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        duration = defaultDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (image.color.a > targetOpacity)
        {
            image.color -= new Color(0, 0, 0, 1) * Time.deltaTime / duration;
            if (image.color.a < 0)
            {
                image.color = new Color(0, 0, 0, 0);
                inTransition = false;
            }
        }
        else if(image.color.a < targetOpacity)
        {
            image.color += new Color(0, 0, 0, 1) * Time.deltaTime / duration;
            if (image.color.a > 1)
            {
                image.color = new Color(0, 0, 0, 1);
                inTransition = false;
            }

        }
        else if (image.color.a == targetOpacity)
        {
            inTransition = false;
        }
    }

    public void FadeToBlack()
    {
        inTransition = true;
        duration = defaultDuration;
        targetOpacity = 1;
    }
    
    public void CustomFade(float opacity, float duration)
    {
        inTransition = true;
        targetOpacity = opacity;
        this.duration = duration;
    }

    public void FadeIn()
    {
        inTransition = true;
        targetOpacity = 0;
        duration = defaultDuration;
    }

    public bool isTransitioning()
    {
        return inTransition;
    }

    public float getOpacity()
    {
        return image.color.a;
    }
}
