using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowController : Character
{
    [SerializeField] private Transform followTransform=null;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float increasedSpeed = 2f;
    [SerializeField] private float jumpLag = 0.8f;
    [SerializeField] private float distanceBeforeJumping = 2.8f;
    private bool speedUp = false;
    private bool jumped = false;

    // Update is called once per frame
    protected override void Update()
    {
        if (!dead)
        {
            Vector2 vectToFollow = followTransform.position - transform.position;
            float xDir = vectToFollow.x > 0 ? 1 : -1;

            if (Mathf.Abs(vectToFollow.x) < 0.2f)
            {
                speedUp = false;
                controller.HoriMove(0);
                if (!controller.IsFacing(GameManager.GetPlayer().transform.position)) controller.Turn();
                animator.SetBool("Running", false);
            }
            else if (vectToFollow.magnitude > maxDistance)
            {
                speedUp = true;
                controller.HoriMove(xDir * increasedSpeed);
                animator.SetBool("Running", true);
            }
            else
            {
                controller.HoriMove(xDir * (speedUp ? increasedSpeed : 1));
                animator.SetBool("Running", true);
            }

            if (vectToFollow.y > distanceBeforeJumping)
            {
                Invoke("Jump", jumpLag);

            }
            if (jumped && controller.GetVelocity().y < 0)
            {
                controller.ReleaseJump();
            }
            animator.SetFloat("yVelocity", controller.GetVelocity().y);
            animator.SetBool("Grounded", controller.IsGrounded());
        }

        if (invulTimer > 0)
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
                sprite.enabled = true;
        }

        if (dead)
        {
            controller.HoriMove(0);
        }

    }

    private void Jump()
    {
        controller.Jump();
        animator.SetTrigger("Jump");
        jumped = true;
    }

    public override void TakeDamage(float dmg, Vector2 source)
    {

        if (health.Get() > 0 && invulTimer <= 0)
        {
            invulTimer = invulnerabilityDuration;
            health.Modify(-dmg);
            controller.Halt();
            Vector2 knockbackDir = (rb2d.position - source);
            if (knockbackDir.x < 0 != controller.IsFacingRight()) controller.Turn();
            controller.Arc(Vector2.up, 0.4f, Mathf.Sign(knockbackDir.x) * -1f, 0.5f, false, "easein", "linear");
            animator.SetTrigger("Hurt");
            if (health.Get() == 0)
            {
                animator.SetBool("Dead", true);
                invulTimer = 0;
            }
        }
    }

    /*
    private void ReleaseJump()
    {
        controller.ReleaseJump();
    }*/
}
