using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
    public static event Action<char> OnGameOver = (type) => { };

    [Header("Character Settings")]
    public Vector3 attackRangeCenter;
    public float attackRange;
    public float backHopDistance = 2.5f;
    public float backHopDuration = 0.32f;
    public float dashDistance = 2.5f;
    public float dashDuration = 0.32f;
    [SerializeField] private Meter lust = null;
    [SerializeField] int jumpAbility = 1;
    [SerializeField] bool dashAbility= true;
    private bool invul = false;

    [Header("Child Scripts")]
    [SerializeField] private QuickTimeEvent qte = null;
    [SerializeField] private Hurtbox sword = null;
    [SerializeField] private Transform foot = null;

    //private bool playerInControl;
    private bool inputControllable = true;
    private bool dashed = false;
    private float knockbackTime = 0.4f;
    private int jumpsLeft = 0;



    protected override void Awake()
    {
        base.Awake();
        SavePoint.OnEnterSavePoint += HandleEnterSavePoint;
        GameManager.SetGameplayEnabled += SetInputControllable;
        lust.Set(SaveManager.playerLust);
        if (SaveManager.playerSpawnPoint != Vector2.zero)
            transform.position = initialPosition = SaveManager.playerSpawnPoint;
    }
    private void OnDestroy()
    {
        SavePoint.OnEnterSavePoint -= HandleEnterSavePoint;
        GameManager.SetGameplayEnabled -= SetInputControllable;
    }

    private void HandleEnterSavePoint(Vector2 savePointPos)
    {
        initialPosition = savePointPos;
    }

   /* protected void Start()
    {
        InvokeRepeating("UpdatePlayerControl", 2.0f, 2f);
    }*/

    protected override void Update()
    {
        if(controller.IsGrounded())
        {
            jumpsLeft = jumpAbility;
            if (!controller.IsDashing()) dashed = false;
        }
        /*if (!playerInControl)
        {
            if (qte.state != QuickTimeEvent.State.Neutral)
            {
                if (qte.state == QuickTimeEvent.State.Failed)
                    Attack(10f);
                else if (qte.state == QuickTimeEvent.State.PassedAggro)
                {
                    Attack(30f);
                    Debug.Log("Aggro");
                }

                qte.enabled = false;
            }
            playerInControl = !qte.enabled;
        }
        else*/ if (inputControllable)
        {
            GetInputs();
        }
        else
        {
            ReleaseInputs();
        }


        if (invulTimer > 0 && !dead)
        {
            invulTimer -= Time.deltaTime;
            if (invulTimer % 0.1f > 0.05f)
            {
                sprite.enabled = true;
            }
            else
            {
                sprite.enabled = false;
            }
            if (invulTimer <= 0)
            {
                sprite.enabled = true;
                invul = false;
            }
        }

        UpdateAnimationState();

    }

    private void ReleaseInputs()
    {
        controller.HoriMove(0);
        controller.ReleaseJump();
    }

    private void GetInputs()
    {
        if (!(invulTimer > (invulnerabilityDuration - knockbackTime)))
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
                && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            {
                controller.HoriMove(Input.GetAxis("Horizontal"));
                if (Input.GetButtonDown("Jump"))
                {
                    if (CanJump())
                    {
                        controller.Jump(); 
                        animator.SetTrigger("Jump");
                        jumpsLeft--;
                    }
                }
               /* if (controller.IsGrounded() && Input.GetKeyDown(KeyCode.E))
                {
                    Vector2 dir = controller.IsFacingRight() ? Vector2.left : Vector2.right;
                    if (controller.Dash(dir, backHopDistance, backHopDuration, false, "Sine"))
                    {
                        animator.SetTrigger("Dash");
                    }
                }*/
                if (dashAbility && !dashed && Input.GetKeyDown(KeyCode.D))
                {
                    Vector2 dir = controller.IsFacingRight() ? Vector2.right : Vector2.left;
                    if (controller.Dash(dir, dashDistance, dashDuration, true, "easein"))
                    {
                        dashed = true;
                        animator.SetTrigger("Dash");
                    }
                }
            }
            else if (!controller.IsGrounded())
            {
                controller.HoriMove(Input.GetAxis("Horizontal"));
            }
            else
            {
                controller.HoriMove(0);
            }
        }
        if (Input.GetButtonUp("Jump")) controller.ReleaseJump();
        if (Input.GetKeyDown(KeyCode.A)) Attack(10f);
    }

    private void UpdateAnimationState()
    {
        animator.SetBool("Grounded", controller.IsGrounded());
        animator.SetBool("Running", Mathf.Abs(controller.GetVelocity().x)>0);
        animator.SetFloat("yVelocity", controller.GetVelocity().y);
    }

    /*private void UpdatePlayerControl()
    {
        if (UnityEngine.Random.value <= lust.GetNormalised())
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.localPosition + attackRangeCenter, attackRange, LayerMask.GetMask("Enemy"));
            if (hitColliders.Length > 0)
            {
                float minDis = float.MaxValue;
                float distance;
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    //TODO: get closest
                    distance = hitColliders[i].transform.position.x - transform.position.x;
                    if (Mathf.Abs(distance) < Mathf.Abs(minDis)) minDis = distance;


                }
                controller.HoriMove(0f);
                if ((minDis > 0) != controller.IsFacingRight())
                    controller.Turn();
                qte.enabled = true;
                playerInControl = false;
            }
        }
    }*/

    public void Attack(float dmg)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")||animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            animator.SetTrigger("Attack2");
        }
        else
        {
            animator.SetTrigger("Attack");
        }
        sword.SetDamage(dmg);
    }

    public override void TakeDamage(float dmg, Vector2 source)
    {

        if (!invul && health.Get() > 0)
        {
            invulTimer = invulnerabilityDuration;
            invul = true;
            health.Modify(-dmg);
            controller.Halt();
            Vector2 knockbackDir = (rb2d.position - source);
            if (knockbackDir.x < 0 != controller.IsFacingRight()) controller.Turn();
            controller.Arc(Vector2.up, 0.4f, Mathf.Sign(knockbackDir.x) * -1f, 0.5f, false, "easein", "linear");
            animator.SetTrigger("Hurt");
            if(health.Get() == 0)
            {
                dead = true;
                animator.SetBool("Dead",true);
                invulTimer = 0;
                SetInputControllable(false);
            }
        }
    }


    public void ModifyLust(float amt)
    {
        
        if (lust.Get()!= lust.GetMax()
            && lust.Modify(amt) == lust.GetMax())
        {
            animator.Play("Transform");
            SetInputControllable(false);
            dead = true;
            invul = true;
        }
    }

    public Meter GetLust()
    {
        return lust;
    }

    public void SetInputControllable(bool inputControllable)
    {
        this.inputControllable = inputControllable;
    }
    
    //used by animator
    public void MakeInputControllable()
    {
        SetInputControllable(true);
    }
    
    public bool IsFacing(Vector2 target)
    {
        return controller.IsFacing(target);
    } 
    public bool IsFacingRight()
    {
        return controller.IsFacingRight();
    }
    
    public Vector2 GetVelocity()
    {
        return controller.GetVelocity();
    }
    public void PlayWakeAnim()
    {
        animator.SetTrigger("Wake");
    }

    public void Respawn()
    {
        animator.SetBool("Dead", false);
        rb2d.position = initialPosition;
        health.Set(health.GetMax());
        animator.Play("Sleep");
        invul = false;
        dead = false;
    }

    public void SetRespawnPoint(Vector2 respawnPoint)
    {
        initialPosition = respawnPoint;
    }

    public override void OnDeath()
    {
        OnGameOver('D');
    }
    public void OnTransform()
    {
        OnGameOver('L');
    }

    private bool CanJump()
    {
        int groundLayerMask = LayerMask.GetMask("Obstacle","OneWay");
        RaycastHit2D hit2D = Physics2D.Raycast(foot.position, Vector2.down, 0.5f, groundLayerMask);
        Collider2D overlap2D = Physics2D.OverlapPoint(foot.position, groundLayerMask);

        return controller.IsGrounded() || (hit2D.collider != null && overlap2D == null && jumpsLeft>0);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.localPosition + attackRangeCenter, attackRange);
    }
}
