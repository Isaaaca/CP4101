using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : SwitchControllableObject
{
    [SerializeField] private float distance=2f;
    [SerializeField] private bool isOpen = false;
    private bool shouldOpen = false;
    private Vector2 openPos = Vector2.zero;
    private Vector2 closePos = Vector2.zero;
    private float speed = 0f;

    private void Start()
    {
        shouldOpen = isOpen;
        speed = distance/duration;
        if (isOpen)
        {
            openPos = transform.position;
            closePos = openPos + Vector2.down * distance;
        }
        else
        {
            closePos = transform.position;
            openPos = closePos + Vector2.up * distance;
        }
    }
    private void Update()
    {
        if ((Vector2)transform.position != (shouldOpen ? openPos : closePos))
        {
            transform.position += (shouldOpen ? Vector3.up : Vector3.down) * speed * Time.deltaTime;
            if (transform.position.y > openPos.y || transform.position.y < closePos.y)
            {
                transform.position = shouldOpen ? openPos : closePos;
                isOpen = shouldOpen;
            }
        }
    }

    public override void OnSwitch()
    {
        shouldOpen = !shouldOpen;
    }

}
