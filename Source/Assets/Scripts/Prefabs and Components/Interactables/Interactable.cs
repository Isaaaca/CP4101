using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    public static event Action<string, Interactable> OnInteractEvent = (id, interactable) => { };

    [SerializeField] private SpriteRenderer tooltip = null;

    private bool focused = false;
    private bool interactable = true;
    protected PlayerController player = null;

    private void Awake()
    {
        GameManager.SetGameplayEnabled += (enabled) => SetInteractable(enabled);
    }

    private void OnDestroy()
    {
        GameManager.SetGameplayEnabled -= (enabled) => SetInteractable(enabled);
    }

    // Update is called once per frame
    void Update()
    {
        if (focused && interactable)
        {
            if (Input.GetKeyDown(KeyCode.R)) OnInteract();
        }
    }

    protected virtual void OnInteract()
    {
        OnInteractEvent(gameObject.name, this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.Equals("Player"))
        {
            player = collision.gameObject.GetComponent<PlayerController>();
            if (player.IsFacing(this.transform.position))
            {
                Focus();

            }
            else
            {
                Unfocus();

            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Unfocus();
    }

    private void Focus()
    {
        if (!focused)
        {
            focused = true;
            tooltip.enabled = true;
        }
    }

    private void Unfocus()
    {
        if (focused)
        {
            focused = false;
            tooltip.enabled = false;
        }
    }


    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    public abstract Sequence GetSequence();
}
