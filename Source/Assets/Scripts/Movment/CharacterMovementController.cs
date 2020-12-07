using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : PhysicsObject
{
    [Header("Movement Settings")]
    public float groundSpeed = 7f;
    public float airSpeed = 8f;
    public float jumpTakeOffSpeed = 15f;
    public float smallJumpDuration = 0.01f;
    public float fallingGravityMultiplier = 1.3f;
    public float cancelJumpGravity = 3;

    [SerializeField]protected bool facingRight=false;

    protected SpriteRenderer spriteRenderer;
    protected bool dashing;
    protected bool arcing;
    protected bool jumped = false;
    private float smallJumpAirtime;

    private float dashElapsedTime;
    private float mdashDistance = 2.5f;
    private float mdashDuration = 0.32f;
    private bool dashInput = false;
    private Vector2 dashDir;
    private VelocityFunctions dashFunction;

    
    private float arcElapsedTime;
    private float arcHeight = 0.32f;
    private float arcWidth = 0.32f;
    private float arcDuration = 0.32f;
    private bool arcInput = false;
    private Vector2 arcHeightDir;
    private Vector2 arcWidthDir;
    private VelocityFunctions arcWfunc;
    private VelocityFunctions arcHfunc;
    
    private float xMovement;
    private float yMovement;
    private bool jumpInput = false;
    private bool releasedJumpInput = true;
    private bool turning = false;

    private enum VelocityFunctions
    {
        Sine,
        EaseIn,
        EaseOut,
        Linear
    }


    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dashElapsedTime = mdashDuration;
        smallJumpAirtime =0;
    }

    public void Jump()
    {
        jumpInput = true;
    }

    public void ReleaseJump()
    {
        releasedJumpInput = true;
        jumpInput = false;
    }

    public void HoriMove(float xVelocity)
    {
        xMovement = xVelocity;
    }

    public void VertMove(float yVelocity)
    {
        yMovement = yVelocity;
    }

    public bool Dash(Vector2 direction, float distance, float duration, bool turn, string fucntion)
    {
        if (System.Enum.TryParse(fucntion, true, out dashFunction))
        {
            dashInput = true;
            dashDir = direction.normalized;
            mdashDistance = distance;
            mdashDuration = duration;
            dashElapsedTime = 0;
            turning = turn;
        }
        else
            return false;
        return true;
        
    }

    public bool Arc(Vector2 direction, float height, float width, float duration, bool turn, string xfucntion, string yfunction)
    {
        if (System.Enum.TryParse(xfucntion, true, out arcWfunc)
            && System.Enum.TryParse(yfunction, true, out arcHfunc))
        {
            arcInput = true;
            arcHeightDir = direction.normalized;
            arcWidthDir = new Vector2(-arcHeightDir.y,arcHeightDir.x);
            arcHeight = Mathf.Abs(height);
            arcWidth = width;
            arcDuration = duration;
            arcElapsedTime = 0;
            turning = turn;
        }
        else 
            return false;
        return true;
    }
    
    public bool IsFacingRight()
    {
        return facingRight;
    }
    public bool IsFacing(Vector2 target)
    {
        return (target - rb2d.position).x > 0 ? IsFacingRight() : !IsFacingRight();
    }

    public bool IsGrounded()
    {
        return grounded;
    }
    public bool IsDashing()
    {
        return dashing;
    }
    public bool IsArcing()
    {
        return arcing;
    }
    public Vector2 GetVelocity()
    {
        return velocity;
    }
    public void SetVelocity(Vector2 velocity)
    {
        this.velocity= velocity;
    }
    public void Turn()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;

        /*spriteRenderer.flipX = !spriteRenderer.flipX;
        facingRight = !facingRight;

        List<Transform> childTranforms = new List<Transform>(gameObject.GetComponentsInChildren<Transform>(true));
        childTranforms.Remove(transform);
        foreach (Transform child in childTranforms)
        {
            child.localPosition = new Vector3(-child.localPosition.x, child.localPosition.y, child.localPosition.z);

        }*/
    }
    public void Halt()
    {
        dashing = false;
        dashInput = false;
        arcInput = false;
        arcing = false;
        HoriMove(0);
        VertMove(0);
        gravityModifier = baseGravityModifier;
        velocity = Vector2.zero;
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        bool newFacingDir = facingRight;

        if (jumpInput && releasedJumpInput)
        {
            groundNormal = Vector2.up;
            jumped = true;
            grounded = false;
            releasedJumpInput = false;
            gravityModifier = baseGravityModifier;
            velocity.y = jumpTakeOffSpeed;
            smallJumpAirtime = smallJumpDuration;
        }
        if (grounded)
        {
            
            //only reset when landing or standing still. Else might get reset mid-jump when still registered as grounded.
            if (velocity.y <=0f)
            {
                jumped = false;
            }

        }
        else
        {
            if (jumped)
            {

                if (smallJumpAirtime > 0 && velocity.y >= 0)
                {
                    velocity.y = jumpTakeOffSpeed;
                    smallJumpAirtime -= Time.deltaTime;
                }
                else if (releasedJumpInput && !dashing && !arcing)
                {
                    gravityModifier = baseGravityModifier * cancelJumpGravity;
                }
            }
            if (velocity.y < 0)
            {
                gravityModifier = baseGravityModifier * fallingGravityMultiplier;
            }
        }

        if (dashInput)
        {
            dashing = true;
            dashInput = false;
            if (turning && dashDir.x!=0)
            {
                newFacingDir = dashDir.x > 0;
            }
            if((facingRight? (groundNormal.x>0 && dashDir.x>0): (groundNormal.x < 0 && dashDir.x < 0))|| !grounded)
            {
                groundNormal = Vector2.up;
                velocity.y = 0;
                gravityModifier = 0;
            }

        }
        
        if (arcInput)
        {
            arcing = true;
            arcInput = false;
        }


        if (dashing)
        {
            dashElapsedTime += Time.deltaTime;
            if (dashElapsedTime >= mdashDuration)
            {
                dashing = false;
                velocity = Vector2.zero;
                gravityModifier = baseGravityModifier;
            }
            else
            {
                move = ParametricMovement(dashDir, mdashDistance, mdashDuration, dashElapsedTime, dashFunction);
                if (Physics2D.OverlapPoint(rb2d.position + Vector2.down * 0.2f + dashDir*Time.deltaTime, LayerMask.GetMask("Obstacle")) != null)
                {
                    gravityModifier = baseGravityModifier;
                }
                else
                {
                    gravityModifier = 0;
                }
            }
        }
        else if (arcing)
        {
            if (arcElapsedTime >= arcDuration)
            {
                arcing = false;
                velocity = Vector2.zero;
                gravityModifier = baseGravityModifier;
            }
            else
            {
                move = BoomerangMovement(arcHeightDir, arcHeight, arcDuration, arcElapsedTime, arcHfunc);

                if (arcWidth != 0)
                {
                    move += ParametricMovement(arcWidthDir, arcWidth, arcDuration, arcElapsedTime, arcWfunc);
                }
                if (turning)
                {
                    newFacingDir = (move.x == 0 ? facingRight : move.x > 0.00f);
                }
                arcElapsedTime += Time.deltaTime;
            }
           
           
        } 
        else
        {
            move.x = xMovement * (grounded ? groundSpeed : airSpeed);
            move.y = yMovement * airSpeed;
            newFacingDir = (move.x == 0 ? facingRight : move.x > 0.00f);
        }


        if (newFacingDir!=facingRight)
        {
            Turn();
        }

        targetVelocity = move;

    }

    private Vector2 ParametricMovement(Vector2 dir, float distance, float duration, float elapsedTime, VelocityFunctions velocityFunction)
    {
        Vector2 result = Vector2.zero;
        switch (velocityFunction)
        {
            case VelocityFunctions.Linear:
                result = dir * distance / duration;
                break;
            case VelocityFunctions.EaseOut:
                result = dir * distance * 3 / duration * Mathf.Pow(elapsedTime / duration, 2f);
                break;
            case VelocityFunctions.EaseIn:
                result = dir * distance * 3 / 2 / duration * (-Mathf.Pow(elapsedTime / duration, 2f) + 1);
                break;
            case VelocityFunctions.Sine:
                result = dir * distance / duration * (Mathf.Sin(elapsedTime / duration * Mathf.PI - Mathf.PI / 2) + 1);
                break;
        }

        return result;
    }
    private Vector2 BoomerangMovement(Vector2 dir, float distance, float duration, float elapsedTime, VelocityFunctions velocityFunction)
    {
        Vector2 result = Vector2.zero;
        switch (velocityFunction)
        {
            case VelocityFunctions.Linear:
                result = dir * distance * 8 / duration * (-2 * elapsedTime / duration + 1);
                break;
            case VelocityFunctions.EaseOut:
                result = dir * distance * 6 / duration * Mathf.Pow(elapsedTime / duration, 2f);
                if (elapsedTime >= 0.794f * duration)
                    result = -result;
                break;
            case VelocityFunctions.EaseIn:
                result = dir * distance * 3  / duration * (-Mathf.Pow(elapsedTime / duration, 2f) + 1);
                if (elapsedTime >= 0.347f * duration)
                    result = -result;
                break;
            case VelocityFunctions.Sine:
                result = dir * distance *Mathf.PI/ duration * (Mathf.Cos(elapsedTime / duration * Mathf.PI));
                break;
        }

        return result;
    }

}
