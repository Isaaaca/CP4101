using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Boss : BaseBossController
{
    [Header("Lunge Settings")]
    [SerializeField] private float aggroDistance = 3;
    [SerializeField] private float lungeDistance = 5;
    [SerializeField] private float lungeDuration = 1;
    [SerializeField] private float windUpTime = 0;
    [SerializeField] private float coolDownTime = 0;
    private Vector2 dirToPlayer;
    private bool isWindUpDone = false;


    [Header("Boulder Drop")]
    [SerializeField] private float pauseTime = 0.1f;
    [SerializeField] private int numGaps = 3;
    [SerializeField] private float boulderTimingOffset = 0.2f;
    [SerializeField] private float slamCamShakeIntensity = 0;
    [SerializeField] private float slamCamShakeduration = 0;
    [SerializeField] private Projectile[] boulders = null;
    private float minX = float.MaxValue; 
    private float maxX = float.MaxValue;
    private float xOffset = float.MaxValue;
    private float boulderY = float.MaxValue;
    private bool hasDashed = false;
    private int boulderToDrop = 0;
    private float boulderTimer = 0;
    private bool dropBouldersFromLeft = true;
    private bool droppingBoulders = false;
    private HashSet<int> bouldersToSkip = new HashSet<int>();



    protected override void Start()
    {
        base.Start();
        GetMinMaxPos();
        xOffset = (maxX - minX) / (boulders.Length - 1);
        boulderY = boulders[0].transform.position.y;
    }

    protected override void Phase1()
    {

        if (!InMidAction() 
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            float distanceToPlayer = player.transform.position.x - transform.position.x;
            controller.HoriMove(0);
            animator.SetBool("CoolDown", false);
            if (CheckBouldersReady())
            {
                animator.SetTrigger("Attack2");
                CamController.Instance.CameraShake(slamCamShakeduration, slamCamShakeIntensity);
                timer = pauseTime;
                StartBolderDrop(distanceToPlayer<0);
            }
            else
            {
                if (!isWindUpDone)
                {
                    if (Mathf.Abs(distanceToPlayer) > aggroDistance)
                    {
                        controller.HoriMove(distanceToPlayer < 0 ? -1 : 1);
                        animator.SetBool("Running", true);
                    }
                    else
                    {
                        if (!controller.IsFacing(player.transform.position)) controller.Turn();
                        dirToPlayer = distanceToPlayer > 0 ? Vector2.right : Vector2.left;
                        animator.SetBool("WindUp",true);
                        timer = windUpTime;
                        isWindUpDone = true;
                        controller.Halt();
                    }
                }
                else
                {
                    if (!hasDashed)
                    {
                        animator.SetBool("WindUp", false);
                        animator.SetTrigger("Dash");
                        controller.Dash(dirToPlayer, lungeDistance, lungeDuration, true, "EaseIn");
                        hasDashed = true;
                    }
                    else
                    {
                        animator.SetBool("CoolDown", true);
                        timer = coolDownTime;
                        isWindUpDone = false;
                        hasDashed = false;
                    }
                }

            }
        }

        if (droppingBoulders)
        {
            boulderTimer += Time.deltaTime;

            int currBoulder = (int)(boulderTimer / boulderTimingOffset);
            if (!dropBouldersFromLeft)
                currBoulder = (boulders.Length - 1) - currBoulder;

            if(currBoulder == boulderToDrop)
            {
                if(!bouldersToSkip.Contains(currBoulder))
                    DropBoulder(currBoulder);

                boulderToDrop = dropBouldersFromLeft ? boulderToDrop + 1 : boulderToDrop - 1;
                if(boulderToDrop>=boulders.Length|| boulderToDrop < 0)
                {
                    droppingBoulders = false;
                }
            }
        }
    }

    private void DropBoulder(int i)
    {
        boulders[i].gameObject.SetActive(true);
        boulders[i].transform.position = new Vector3(minX + (i * xOffset), boulderY);
        boulders[i].Reset();
    }

    private void StartBolderDrop(bool leftToRight)
    {
        bouldersToSkip.Clear();
        while (bouldersToSkip.Count < numGaps)
        {
            int num = Random.Range(0, boulders.Length);
            if (!bouldersToSkip.Contains(num)) bouldersToSkip.Add(num);
        }
        dropBouldersFromLeft = leftToRight;
        droppingBoulders = true;
        boulderTimer = 0;
        boulderToDrop = leftToRight ? 0 : boulders.Length - 1;
    }

    protected override void Phase2()
    {
        Phase1();
    }

    protected override void TriggerPhase2()
    {
        hasDashed = false;
        isWindUpDone = false;
    }

    private bool CheckBouldersReady()
    {
        for (int i = 0; i < boulders.Length; i++)
        {
            if (boulders[i].isActiveAndEnabled)
            {
                return false;
            }
        }
        return true;
    }

    private void GetMinMaxPos()
    {
        minX = float.MaxValue;
        maxX = float.MinValue;
        for (int i = 0; i < boulders.Length; i++)
        {
            float xPos = boulders[i].transform.position.x;
            if (xPos > maxX) maxX = xPos;
            if (xPos < minX) minX = xPos;
        }
    }

    public override void Reset()
    {
        base.Reset();
        for (int i = 0; i < boulders.Length; i++)
        {
            boulders[i].Reset();
            boulders[i].gameObject.SetActive(false);
        }
        droppingBoulders = false;
    }

#if UNITY_EDITOR
    [ContextMenu("DistributeBoulders")]
    void Distribute()
    {
        GetMinMaxPos();

        xOffset = (maxX - minX) / (boulders.Length - 1);
        for (int i = 0; i < boulders.Length; i++)
        {
            Vector3 pos = boulders[i].transform.position;
            boulders[i].transform.position = new Vector3(minX + (i * xOffset), pos.y);
        }
    }

   
#endif
}
