using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Boss : BaseBossController
{
    [Header("Boss Settings")]
    [SerializeField] private float lungeDistance = 5;
    [SerializeField] private float aggroDisance = 4;
    [SerializeField] private float lungeDuration = 1;
    [SerializeField] private float pauseTime = 0.1f;
    [SerializeField] private float windUpTime = 0;
    [SerializeField] private float coolDownTime = 0;
    [SerializeField] private int phase1DashCount = 1;
    [Header("Phase 2")]
    [SerializeField] private int phase2DashCount = 6;
    [SerializeField] private float phase2PauseTime = 0.2f;
    [SerializeField] private float phase2PWindUpTime = 2f;
    [SerializeField] private float phase2CoolDownTime = 5f;
    private bool isWindUpDone = false;
    private bool isPauseDone = false;
    private int dashesLeft = 1;
    private Vector2 dirToPlayer;


    protected override void Phase1()
    {
        if (!InMidAction() 
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            float distanceToPlayer = player.transform.position.x - transform.position.x;
            if (dashesLeft <= 0)
            {
                //TODO: cooldown animation
                animator.SetBool("CoolDown",true);
                timer = coolDownTime;
                dashesLeft = phase1DashCount;
                isWindUpDone = false;
            }
            else if (dashesLeft > 0)
            {
                controller.HoriMove(0);
                animator.SetBool("CoolDown",false);
               
                if (!isWindUpDone)
                {
                    if (Mathf.Abs(distanceToPlayer) > aggroDisance)
                    {
                        controller.HoriMove(distanceToPlayer < 0 ? -1 : 1);
                        animator.SetBool("Running", true);
                    }
                    else
                    {
                        dirToPlayer = distanceToPlayer < 0 ? Vector2.left : Vector2.right;
                        if (!controller.IsFacing(player.transform.position)) controller.Turn();
                        animator.SetBool("WindUp",true);
                        timer = windUpTime;
                        isWindUpDone = true;
                    }
                    
                }
                else if (!isPauseDone)
                {
                    timer = pauseTime;
                    isPauseDone = true;
                }
                else
                {
                    animator.SetTrigger("Dash");
                    controller.Dash(dirToPlayer, lungeDistance, lungeDuration, true, "EaseIn");
                    dashesLeft--;
                    isPauseDone = false;
                    animator.SetBool("WindUp", false);
                }
            }
        }
        else
        {
            controller.HoriMove(0f);
            animator.SetBool("Running", false);
        }
        
    }

    protected override void Phase2()
    {
        if (!InMidAction()
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            float distanceToPlayer = player.transform.position.x - transform.position.x;
            if (dashesLeft <= 0)
            {
                //TODO: cooldown animation
                animator.SetBool("CoolDown",true);
                timer = phase2CoolDownTime;
                dashesLeft = phase2DashCount;
                isWindUpDone = false;
            }
            else if (dashesLeft > 0)
            {
                controller.HoriMove(0);
                animator.SetBool("CoolDown", false);

                if (!isWindUpDone)
                {
                    if (Mathf.Abs(distanceToPlayer) > aggroDisance)
                    {
                        controller.HoriMove(distanceToPlayer < 0 ? -1 : 1);
                        animator.SetBool("Running", true);
                    }
                    else
                    {
                        if (!controller.IsFacing(player.transform.position)) controller.Turn();
                        animator.SetTrigger("Attack2");
                        animator.SetBool("WindUp", true);
                        timer = phase2PWindUpTime;
                        isWindUpDone = true;
                    }

                }
                else if (!isPauseDone)
                {
                    dirToPlayer = distanceToPlayer < 0 ? Vector2.left : Vector2.right;
                    animator.SetBool("WindUp",true);
                    timer = phase2PauseTime;
                    isPauseDone = true;
                }
                else
                {
                    float randomModifier = Random.Range(0.6f, 1.6f);
                    animator.SetTrigger("Dash");
                    controller.Dash(dirToPlayer, Mathf.Max(distanceToPlayer+3, lungeDistance*randomModifier), lungeDuration, true, "EaseIn");
                    dashesLeft--;
                    animator.SetBool("WindUp", false);
                    isPauseDone = false;
                }
            }
        }
        else
        {
            if(isWindUpDone && !controller.IsDashing() && !controller.IsFacing(player.transform.position)) controller.Turn();
            controller.HoriMove(0f);
            animator.SetBool("Running", false);
        }
    }

    protected override void TriggerPhase2()
    {  
        dashesLeft = phase2DashCount;
        isWindUpDone = false;
    }
}
