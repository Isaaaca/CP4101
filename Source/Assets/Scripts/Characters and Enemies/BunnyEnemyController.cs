using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyEnemyController : Character
{
    [Header("Bunny Settings")]
    [SerializeField] private float minRunDuration = 0.2f;
    [SerializeField] private float maxRunDuration = 1f;
    [SerializeField] private float minIdleDuration = 1f;
    [SerializeField] private float maxIdleDuration = 5f;

    [SerializeField] private float jumpWeight = 1f;
    [SerializeField] private float idleWeight = 1f;
    [SerializeField] private float runWeight = 1f;
    [SerializeField] private float timingOffset = 0f;

    private bool runDir = false;
    private float timer = 1f;
    private enum Action
    {
        Jump,
        Run,
        Idle
    }

    private Action currAction = Action.Idle;

    private void Start()
    {
        float totalWeight = jumpWeight + idleWeight + runWeight;
        jumpWeight /= totalWeight;
        idleWeight /= totalWeight;
        runWeight /= totalWeight;
        timer += timingOffset;
    }


    public override void OnDeath()
    {
        base.OnDeath();
    }

    protected override void Update()
    {
        if (!dead)
        {


            if (currAction == Action.Jump)
            {
                if (controller.GetVelocity().y < 0)
                    controller.ReleaseJump();
                else if (controller.IsGrounded())
                    SetNextAction();
            }
            else if (currAction == Action.Run)
            {
                controller.HoriMove(runDir ? -1 : 1);
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    controller.Halt();
                    SetNextAction();
                }
            }

            UpdateAnimationState();
            base.Update();
        }
    }

    private void SetNextAction()
    {
        if(currAction != Action.Idle)
        {
            timer = Random.Range(minIdleDuration, maxIdleDuration);
            currAction = Action.Idle;
        }
        else
        {
            float choice = Random.value;
                if (choice <= jumpWeight && currAction != Action.Jump)
                {
                    if (controller.IsGrounded())
                    {
                        controller.Jump();
                        animator.SetTrigger("Jump");
                        currAction = Action.Jump;
                    }
                } 
                else
                {
                    choice -= jumpWeight;
                    if (choice <= runWeight)
                    {
                        runDir = Random.value > 0.5f;
                        timer = Random.Range(minRunDuration, maxRunDuration);
                        currAction = Action.Run;
                    }
                    else
                    {
                        timer = Random.Range(minIdleDuration, maxIdleDuration);
                        currAction = Action.Idle;
                    }
                }
        }
      
    }

    private void UpdateAnimationState()
    {
        animator.SetBool("Grounded", controller.IsGrounded());
        animator.SetBool("Running", Mathf.Abs(controller.GetVelocity().x) > 0);
        animator.SetFloat("yVelocity", controller.GetVelocity().y);
    }
}
