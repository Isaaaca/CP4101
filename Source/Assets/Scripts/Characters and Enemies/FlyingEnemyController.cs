using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : Character
{
    public enum State
    {
        patrol,
        chase
    }

    [HideInInspector] public State state = State.patrol;

    [Header("Character Settings")]
    [SerializeField] private float aggroRange = 0;
    [SerializeField] private float deaggroRange = 0;
    [SerializeField] private float lungeRange = 0;
    [SerializeField] private float windUpTime = 0;
    [SerializeField] private float diveDuration = 0;
    [SerializeField] private float diveHeight = 0;
    [SerializeField] private float diveWidth = 0;
    public Vector2[] wayPoints ;
    public bool loop;

    private int nextWaypoint = 1;
    private int step = 1;
    private float windUpTimer = 0;
    private GameObject player;
    private bool aggro = false;
    private bool windingUp = false;
    private Vector2 lungeDir= Vector2.zero;
    // Start is called before the first frame update
    private void Start()
    {
        wayPoints[0] = transform.position;
        player = GameManager.GetPlayer();
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector2 vectToPlayer = (Vector2)player.transform.position - rb2d.position;
        float distance = vectToPlayer.magnitude;
        if (!controller.IsArcing())
        {
            if (distance < lungeRange && vectToPlayer.y<=0)
            {
                if (!windingUp)
                {
                    if (!controller.IsFacing(player.transform.position)) controller.Turn();
                    animator.SetTrigger("WindUp");
                    windingUp = true;
                    lungeDir = vectToPlayer;
                }
            }
            else if (distance < aggroRange)
            {
                //chase
                aggro = true;
            }
            else if (distance > deaggroRange)
            {
                aggro = false;
            }

            if (windingUp)
            {
                windUpTimer += Time.deltaTime;
                if (windUpTimer >= windUpTime)
                {
                    controller.Arc(Vector2.down, diveHeight, diveWidth*Mathf.Sign(lungeDir.x), diveDuration, true, "linear", "sine");
                    animator.SetTrigger("Attack");
                    windUpTimer = 0;
                    windingUp = false;
                }
                else
                { 
                    controller.SetVelocity(Vector2.zero);
                    controller.HoriMove(0);
                    controller.VertMove(0);
                }
            }
            else
            {
                Vector2 dir;
                if (aggro)
                {
                    dir = (Vector2)player.transform.position - rb2d.position;
                    dir = dir.normalized;
                }
                //move to next waypoint
                else
                {
                    dir = (wayPoints[nextWaypoint] - rb2d.position);
                    if (dir.magnitude < 0.1)
                    {
                        nextWaypoint = (nextWaypoint + step);
                        if (loop)
                        {
                            nextWaypoint = nextWaypoint % wayPoints.Length;
                        }
                        else if (0 > nextWaypoint || nextWaypoint >= wayPoints.Length)
                        {
                            step *= -1;
                            nextWaypoint = (nextWaypoint + 2 * step);

                        }
                    }
                    dir = dir.normalized;
                }

                controller.HoriMove(dir.x);
                controller.VertMove(dir.y);
            }
        }

        animator.SetFloat("yVelocity", controller.GetVelocity().y);
        base.Update();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.localPosition, lungeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.localPosition, aggroRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.localPosition, deaggroRange);
    }
}
