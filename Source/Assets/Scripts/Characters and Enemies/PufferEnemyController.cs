using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PufferEnemyController : Character
{
    // Start is called before the first frame update

    [Header("Character Settings")]
    [SerializeField] private float range = 0;
    [SerializeField] private float windUpTime = 0;
    [SerializeField] private float coolDownTime = 0;
    [SerializeField] private Transform frontGrndDetection = null;
    [SerializeField] private Transform backGrndDetection = null;
    [SerializeField] private GameObject projectilePrefab = null;
    


    private PuffProjectile projectile = null;
    private int layer_mask;
    private GameObject player;
    private float windUpTimer = 0;
    private bool windingUp = false;
    private float coolDownTimer = 0;
    private bool coolDown = false;


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
        layer_mask = LayerMask.GetMask("Obstacle","OneWay");
        player = GameManager.GetPlayer();
    }
    protected override void Update()
    {
        if (!dead)
        {
            Vector2 vectToPlayer = (Vector2)player.transform.position - rb2d.position;
            float distance = vectToPlayer.magnitude;
            bool isFacingPlayer = controller.IsFacing(player.transform.position);
            if (distance < range && isFacingPlayer)
            {
                if (!windingUp && !coolDown)
                {
                    windingUp = true;
                }
            }
            if (windingUp)
            {
                windUpTimer += Time.deltaTime;
                if (windUpTimer >= windUpTime)
                {

                    animator.SetTrigger("Fire");
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
                animator.SetBool("Inflate", coolDownTimer >= coolDownTime - 1);
                if (coolDownTimer >= coolDownTime)
                {
                    coolDownTimer = 0;
                    coolDown = false;
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
            RaycastHit2D groundInfo = Physics2D.Raycast(coolDown ? backGrndDetection.position : frontGrndDetection.position, Vector2.down, 0.3f, layer_mask);
            Collider2D wallInfo = Physics2D.OverlapPoint(frontGrndDetection.position, layer_mask);
            if (groundInfo.collider == false || groundInfo.normal.y != 1 || wallInfo != false)
            {
                if (controller.IsDashing()) controller.Halt();
                else controller.HoriMove(0);
            }
            UpdateAnimationState();
        }
       
        base.Update();
    }


    private void UpdateAnimationState()
    {
        animator.SetBool("Cooldown", coolDown);
        animator.SetBool("Running", Mathf.Abs(controller.GetVelocity().x) > 0);
        animator.SetBool("Grounded", controller.IsGrounded());
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
                RaycastHit2D groundInfo = Physics2D.Raycast(frontGrndDetection.position, Vector2.down, 0.3f, layer_mask);
                if (groundInfo.collider==null || groundInfo.normal.y != 1)
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
    public void FireProjectile()
    {
        RaycastHit2D floorBehind = Physics2D.Raycast(backGrndDetection.position, Vector2.down, 0.3f, layer_mask);
        if (floorBehind.collider != null && floorBehind.normal.y == 1)
            controller.Dash(controller.IsFacingRight() ? Vector2.left : Vector2.right, 01f, 0.3f, false, "easeIn");
        if (projectile == null)
        {
            projectile = Instantiate(projectilePrefab).GetComponent<PuffProjectile>();
        }
        projectile.gameObject.SetActive(true);
        Vector2 dir = controller.IsFacingRight() ? Vector2.right : Vector2.left;
        projectile.transform.localScale = dir;
        projectile.Fire(rb2d.position, dir);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
