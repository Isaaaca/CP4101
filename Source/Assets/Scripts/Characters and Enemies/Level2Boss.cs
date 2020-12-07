using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Boss : BaseBossController
{
    [Header("Boss Settings")]
    [SerializeField] private float lungeDistance = 5;
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

    [SerializeField] private Transform[] tpPoints=new Transform[8];
    [SerializeField] private AfterImage[] afterImages=new AfterImage[2];
    private bool isWindUpDone = false;
    private bool isPauseDone = false;
    private int dashesLeft = 4;
    private Vector2 dirToDash;
    private int pointToSpawn = 0;
    


    protected override void Phase1()
    {
        if (!InMidAction() 
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
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
                if (!isPauseDone)
                {
                    timer = pauseTime;
                    isPauseDone = true;
                }
                else if (!isWindUpDone)
                {
                    pointToSpawn = 0;
                    float playerHeight = player.transform.position.y;
                    dirToDash = Vector2.left;
                    for (int i = 3; i > -1; i--)
                    {
                        if (playerHeight <= tpPoints[i].transform.position.y)
                        {
                            pointToSpawn = i;
                        }
                    }
                    //spawn at point further from player
                    if (tpPoints[0].position.x - player.transform.position.x < player.transform.position.x - tpPoints[4].position.x)
                    {
                        dirToDash = Vector2.right;
                        pointToSpawn += 4;
                    }
                    animator.SetTrigger("Teleport");
                    timer = windUpTime;
                    isWindUpDone = true;

                }
                else
                {
                    animator.SetTrigger("Dash");
                    controller.Dash(dirToDash, lungeDistance, lungeDuration, true, "EaseIn");
                    dashesLeft--;
                    isPauseDone = false;
                    isWindUpDone = false;
                    animator.SetBool("WindUp", false);
                }
            }
        }
        else
        {
            controller.HoriMove(0f);
        }
        
    }

    protected override void Phase2()
    {
        if (!InMidAction()
           && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
           && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")
           && !animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Roar"))
        {
            if (dashesLeft <= 0)
            {
                //TODO: cooldown animation
                animator.SetBool("CoolDown", true);
                timer = phase2CoolDownTime;
                dashesLeft = phase2DashCount;
                isWindUpDone = false;
                print("cool");
            }
            else if (dashesLeft > 0)
            {
                controller.HoriMove(0);
                animator.SetBool("CoolDown", false);

                if (!isPauseDone)
                {

                    timer = phase2PauseTime;
                    isPauseDone = true;
                }
                else if (!isWindUpDone)
                {
                    pointToSpawn = 0;
                    float playerHeight = player.transform.position.y;
                    dirToDash = Vector2.left;
                    for (int i = 3; i > -1; i--)
                    {
                        if (playerHeight <= tpPoints[i].transform.position.y)
                        {
                            pointToSpawn = i;
                        }
                    }
                    //spawn at point further from player
                    if (tpPoints[0].position.x - player.transform.position.x < player.transform.position.x - tpPoints[4].position.x)
                    {
                        dirToDash = Vector2.right;
                        pointToSpawn += 4;
                    }
                    animator.SetTrigger("Teleport");
                    timer = phase2PWindUpTime;
                    isWindUpDone = true;
                }
                else
                {
                    animator.SetTrigger("Dash");
                    controller.Dash(dirToDash, lungeDistance, lungeDuration, true, "EaseIn");
                    dashesLeft--;
                    isPauseDone = false;
                    isWindUpDone = false;
                    animator.SetBool("WindUp", false);
                }
            }
        }
        else
        {
            controller.HoriMove(0f);
        }
    }

    public void Teleport()
    {
        transform.position = tpPoints[pointToSpawn].position;
        if ((pointToSpawn > 3) ? (!controller.IsFacingRight()) : (controller.IsFacingRight()))
        {
            controller.Turn();
        }
        animator.SetTrigger("Attack2");
        animator.SetBool("WindUp", true);

        if(phase2Triggered)
        {
            List<int> spotsLeft = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            spotsLeft.Remove(pointToSpawn);
            spotsLeft.Remove((pointToSpawn+4)%7);

            print(afterImages.Length);
            for(int i = 0; i<afterImages.Length; i++)
            {
                int spot = spotsLeft[Random.Range(0, spotsLeft.Count)];
                spotsLeft.Remove(spot);
                afterImages[i].gameObject.SetActive(true);
                afterImages[i].transform.position = tpPoints[spot].position;
                afterImages[i].Dash(spot > 3, phase2PWindUpTime, lungeDistance, lungeDuration);
            }
        }

    }
    public override void TakeDamage(float dmg, Vector2 source)
    {
        base.TakeDamage(dmg, source);
        if (dead)
        {
            for (int i = 0; i < afterImages.Length; i++)
            {
                afterImages[i].gameObject.SetActive(false);
            }
        }
    }

    protected override void TriggerPhase2()
    {
        dashesLeft = phase2DashCount;
        isWindUpDone = false;
    }

}
