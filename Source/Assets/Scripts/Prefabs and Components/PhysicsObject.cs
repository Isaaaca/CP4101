using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [Header("Physics Settings")]
    public float baseGravityModifier = 1f;
    public float drag = 0f;
    public Collider2D body;

    [SerializeField] private float minGroundNormalY = 0.65f;
    [SerializeField] private LayerMask physicsIgnoredLayers = 0;

    protected float gravityModifier;
    protected bool grounded;
    protected Vector2 targetVelocity;
    protected Vector2 groundNormal;
    protected Vector2 velocity; 
    protected Rigidbody2D rb2d;
    protected ContactFilter2D downwardContactFilter;
    protected ContactFilter2D upwardContactFilter;
    protected ContactFilter2D oneWayContactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    protected bool ignoreOneWay = false;

    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        gravityModifier = baseGravityModifier;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        LayerMask layerMask = (Physics2D.GetLayerCollisionMask(gameObject.layer) | physicsIgnoredLayers) ^ physicsIgnoredLayers;
        downwardContactFilter.useTriggers = false;
        downwardContactFilter.SetLayerMask(layerMask);
        upwardContactFilter.useTriggers = false;
        upwardContactFilter.SetLayerMask(layerMask - LayerMask.GetMask("OneWay"));
        oneWayContactFilter.useTriggers = false;
        oneWayContactFilter.SetLayerMask(LayerMask.GetMask("OneWay"));
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        targetVelocity = Vector2.zero;
        ComputeVelocity();

    }

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        Collider2D[] oneWayContacts = new Collider2D[1];

        if (body.OverlapCollider(oneWayContactFilter, oneWayContacts) > 0)
        {
            ignoreOneWay = velocity.y > 0 || oneWayContacts[0].ClosestPoint(transform.position).y - shellRadius> transform.position.y;
        }
        else
        {
            ignoreOneWay = velocity.y > 0 ;
        }

        grounded = false;
        if (velocity.y <= 0 && gravityModifier!=0)
        {
            //GROUND CHECK
            GroundCheck(Vector2.down);
        }
        
        velocity.x = targetVelocity.x;
        if (targetVelocity.y != 0) velocity.y = targetVelocity.y;

        if (!grounded)
        {
            velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime - velocity.normalized * (grounded ? 0 : drag);
            Vector2 deltaposition = velocity * Time.fixedDeltaTime;
            Movement(deltaposition.y * Vector2.up);
            Movement(deltaposition.x * Vector2.right);
        }
        else
        {
            if (groundNormal.x == 0) GroundCheck(velocity);
            Vector2 groundTangent = new Vector2(groundNormal.y, -groundNormal.x);
            Vector2 deltaposition = groundTangent.normalized * velocity.magnitude*Mathf.Sign(velocity.x);
            Movement(deltaposition * Time.fixedDeltaTime);
        }

        /* velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime - velocity.normalized*(grounded ? 0 : drag);
         velocity.x = targetVelocity.x;
         if (targetVelocity.y != 0) velocity.y = targetVelocity.y;

         grounded = false;

         Vector2 deltaposition = velocity * Time.fixedDeltaTime;

         Vector2 move; 
         move = Vector2.up * deltaposition.y;
         Movement(move, true);

         if (!grounded) SlopeCheck();

         Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
         move = (grounded? moveAlongGround:Vector2.right)* deltaposition.x ;
         Movement(move, false);*/

    }

    void Movement(Vector2 move)
    {
        float distance = move.magnitude;
        ContactFilter2D contactFilter = (move.y > 0 ||ignoreOneWay) ? upwardContactFilter : downwardContactFilter;

        if (distance > minMoveDistance)
        {

            int count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i<count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }
            if (hitBufferList.Count > 0)
            {
                int nearestHit = -1;
                float smallestDistance = float.MaxValue;
                for (int i = 0; i < hitBufferList.Count; i++)
                {
                    if (hitBufferList[i].distance < smallestDistance)
                    {
                        if(!ignoreOneWay||
                            hitBufferList[i].collider.gameObject.layer != LayerMask.NameToLayer("OneWay"))
                        {
                            nearestHit = i;
                            smallestDistance = hitBufferList[i].distance;
                        }
                    }
                }

                if (nearestHit != -1)
                {
                    Vector2 currentNormal = hitBufferList[nearestHit].normal;
                    //bonking ceiling
                    if (velocity.y > 0 && Mathf.Sign(move.y) != Mathf.Sign(currentNormal.y))
                    {
                        velocity.y = 0;
                    }
                    float modifiedDistance = hitBufferList[nearestHit].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            
        }
        rb2d.position += move.normalized * distance;
    }

    private void GroundCheck(Vector2 direction)
    {
        
        hitBufferList.Clear();
        int count = body.Cast(direction, downwardContactFilter, hitBuffer, .1f);
        for (int i = 0; i < count; i++)
        {
            hitBufferList.Add(hitBuffer[i]);
        }
        float smallestDistance = float.MaxValue;
        for (int i = 0; i < hitBufferList.Count; i++)
        {
            if (!ignoreOneWay || hitBufferList[i].point.y < rb2d.position.y)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    groundNormal = currentNormal;
                    velocity.y = 0;
                    if (hitBufferList[i].distance < smallestDistance)
                    {
                        smallestDistance = hitBufferList[i].distance;
                    }
                }
            }
        }
        if (smallestDistance != float.MaxValue && smallestDistance > shellRadius)
        {
            rb2d.position += direction.normalized * (smallestDistance - shellRadius);
        }
        
    }
}
