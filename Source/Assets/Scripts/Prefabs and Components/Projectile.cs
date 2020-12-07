using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, ResetableInterface

{
    public float drag = 0f;
    [SerializeField] private int destroyOnXhits =0;
    [SerializeField] private bool destroyGameObject = false;
    [SerializeField] private bool camShakeOnImpact = false;
    [SerializeField] private float camShakeIntensity = .2f;
    [SerializeField] private float camShakeDuration = .2f;
    private int hitsLeft =1;
    protected Animator animator;

    [SerializeField] protected Rigidbody2D rb2d;
    [SerializeField] protected Collider2D col2d;
    [SerializeField] protected float gravityModifier;
    [SerializeField] protected Vector2 initialVelocity;
    [SerializeField] protected LayerMask passThroughLayers;

    [HideInInspector]
    [SerializeField] protected Vector2 velocity;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        col2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime - velocity.normalized * drag;

        Vector2 deltaposition = velocity * Time.fixedDeltaTime;

        rb2d.position += deltaposition;
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)initialVelocity/10f);
    }

    public virtual void Fire(Vector2 startPos, Vector2 dir)
    {
        if (animator != null)
        {
            animator.SetBool("Destroyed", false);
        }
        hitsLeft = destroyOnXhits;
        transform.position = startPos;
        velocity = dir * initialVelocity.magnitude;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (passThroughLayers != (passThroughLayers | (1 << col.gameObject.layer)))
        { 
            hitsLeft--;
        }


        if (hitsLeft == 0)
        {
            if (camShakeOnImpact)
            {
                Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
                if (viewPos.x<1.5 && viewPos.x>-0.5 && viewPos.y < 1.5 && viewPos.y > -0.5)
                {
                    float dropOff = (1-Mathf.Abs(Mathf.Abs(viewPos.x)-0.5f));
                    CamController.Instance.CameraShake(camShakeIntensity* dropOff, camShakeDuration);

                }
            }
            velocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetBool("Destroyed", true);
            }
            else
            {
                OnDestroyAnimFinish();
            }
        }
    }

    public virtual void OnDestroyAnimFinish()
    {
        if (destroyGameObject)
            Destroy(this.gameObject);
        else
            this.gameObject.SetActive(false);
    }

    public void Reset()
    {
        if (animator != null)
        {
            animator.SetBool("Destroyed", false);
        }
        hitsLeft = destroyOnXhits;
        velocity = initialVelocity;
    }
}
