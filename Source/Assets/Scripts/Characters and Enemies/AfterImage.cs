using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [SerializeField]protected CharacterMovementController controller;
    [SerializeField] protected Animator animator;
    private bool dashed = false;
    private bool paused = false;
    private float timer = float.MaxValue;
    private float dashDuration = 1;
    private float dashDistance = 18;
    private float pauseDuration = 0.4f;
    private Vector2 dashDir = Vector2.right;


    public void Dash(bool dashToRight, float windUpTime, float distance, float duration)
    {
        animator.Play("Boss_TeleportIn");
        if (dashToRight ? (!controller.IsFacingRight()) : (controller.IsFacingRight()))
        {
            controller.Turn();
        }
        dashDir = dashToRight ? Vector2.right : Vector2.left;
        animator.SetTrigger("Attack2");
        animator.SetBool("WindUp", true);
        timer = windUpTime;
        dashDuration = duration;
        dashDistance = distance;
        dashed = false;
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                {
                    if (!dashed)
                    {
                        animator.SetTrigger("Dash");
                        controller.Dash(dashDir, dashDistance, dashDuration, true, "EaseIn");
                        animator.SetBool("WindUp",false);
                        animator.SetBool("Running", true);
                        dashed = true;
                    }
                }
        }
        else
        {
            if (!controller.IsDashing())
            {
                if (!paused)
                {
                    timer = pauseDuration;
                    paused = true;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            animator.SetBool("Running", controller.IsDashing());
        }
       
    }
}

