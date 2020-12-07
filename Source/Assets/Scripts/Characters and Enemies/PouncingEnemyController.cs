using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PouncingEnemyController : Character
{
    [Header("Character Settings")]
    [SerializeField] private float aggroRange = 0;
    [SerializeField] private float lungeDistance = 0;
    [SerializeField] private float lungeDuration = 0;
    [SerializeField] private float windUpTime = 0;
    [SerializeField] private float coolDownTime = 0;
    [SerializeField] private Transform groundDetection = null;
    private int layer_mask;
    private GameObject player;
    private float windUpTimer = 0;
    private bool windingUp = false;
    private float coolDownTimer = 0;
    private bool coolDown = false;
    private Vector2 lungeDir = Vector2.right;

    [Header("Roam Settings")]
    [SerializeField] private float minRunDuration = 0.2f;
    [SerializeField] private float maxRunDuration = 1f;
    [SerializeField] private float minIdleDuration = 1f;
    [SerializeField] private float maxIdleDuration = 5f;
    [SerializeField] private float idleWeight = 1f;
    [SerializeField] private float runWeight = 1f;
    private bool runDir = false;
    private float timer = 1f;
    private bool idle = true;

    private void Start()
    {
        float totalWeight = idleWeight + runWeight;
        idleWeight /= totalWeight;
        runWeight /= totalWeight;
        layer_mask = LayerMask.GetMask("Obstacle");
        player = GameManager.GetPlayer();
    }
    protected override void Update()
    {
       
       

        Vector2 vectToPlayer = (Vector2)player.transform.position - rb2d.position;
        float distance = vectToPlayer.magnitude;
        bool isFacingPlayer = vectToPlayer.x > 0 ? controller.IsFacingRight() : !controller.IsFacingRight();
        if (!controller.IsDashing())
        {
            if (distance < aggroRange && isFacingPlayer)
            {
                if (!windingUp && !coolDown)
                {
                    windingUp = true;
                    animator.SetTrigger("WindUp");
                    lungeDir = Vector2.right * Mathf.Sign(vectToPlayer.x);
                }
            }

            if (windingUp)
            {
                windUpTimer += Time.deltaTime;
                if (windUpTimer >= windUpTime)
                {
                    animator.SetTrigger("Dash");
                    controller.Dash(lungeDir, lungeDistance, lungeDuration, true, "easeIn");
                    windUpTimer = 0;
                    windingUp = false;
                    coolDown = true;
                }
                else
                {
                    controller.SetVelocity(Vector2.zero);
                    controller.HoriMove(0);
                    controller.VertMove(0);
                }
            }
            else if (coolDown)
            {
                coolDownTimer += Time.deltaTime;
                if (coolDownTimer >= coolDownTime)
                {
                    coolDownTimer = 0;
                    coolDown = false;
                }
            }  


        }

        if (!coolDown && !windingUp)
        {
            if (!idle)
            {
                controller.HoriMove(runDir ? 1 : -1);
                if (runDir != controller.IsFacingRight()) controller.Turn();
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    controller.HoriMove(0);
                    SetNextAction();
                }
            }
        }
        else { controller.HoriMove(0); }
        
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 0.3f, layer_mask);
        Collider2D wallInfo = Physics2D.OverlapPoint(groundDetection.position, layer_mask);
        if (groundInfo.collider == false || groundInfo.normal.y != 1 || wallInfo != false)
        {
            if (controller.IsGrounded()) controller.Halt();
            else controller.HoriMove(0);
        }

        UpdateAnimationState();
        base.Update();
    }
    private void UpdateAnimationState()
    {
        animator.SetBool("Running", Mathf.Abs(controller.GetVelocity().x) > 0);
        animator.SetFloat("Velocity", controller.GetVelocity().magnitude);

    }

    private void SetNextAction()
    {
        if (!idle)
        {
            timer = Random.Range(minIdleDuration, maxIdleDuration);
            idle = true;
        }
        else
        {
            float choice = Random.value;
            if (choice <= runWeight)
            {
                RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 0.3f, layer_mask);
                if (groundInfo.collider == null || groundInfo.normal.y != 1)
                {
                    runDir = !controller.IsFacingRight();
                }
                else
                    runDir = Random.value > 0.5f;
                timer = Random.Range(minRunDuration, maxRunDuration);
                idle = false;
            }
            else
            {
                timer = Random.Range(minIdleDuration, maxIdleDuration);
                idle = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lungeDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
