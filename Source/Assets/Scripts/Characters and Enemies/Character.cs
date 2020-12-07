using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, ResetableInterface
{
    public static event Action<Character> OnCharacterDeath = (character) => { };

    [Header("Generic Character Settings")]
    public Meter health;
    public float invulnerabilityDuration = 0.4f;
    [SerializeField] protected float lustRelief = 0f;
    [SerializeField] protected bool activeAfterDeath= false;
    [SerializeField] protected AudioClip hurtSoundClip= null;


    protected CharacterMovementController controller;
    protected Animator animator;
    protected SpriteRenderer sprite;
    protected Rigidbody2D rb2d;
    protected float invulTimer = 0f;
    protected Vector2 initialPosition;
    protected Hurtbox hurtbox;
    protected bool dead;
    protected virtual void Awake()
    {
        controller = GetComponent<CharacterMovementController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        initialPosition = rb2d.position;
        hurtbox = GetComponent<Hurtbox>();
    }

    protected virtual void Update()
    {
        if (invulTimer > 0)
        {
            invulTimer -= Time.deltaTime;
            if (invulTimer % 0.1f > 0.05f)
            {
                sprite.color = Color.red;
            }
            else
            {
                sprite.color = Color.white;
            }
            if (invulTimer <= 0)
                sprite.color = Color.white;
        }

        if (dead)
        {
            controller.HoriMove(0);
        }
    }
 

    public virtual void TakeDamage(float dmg, Vector2 source)
    {
        if (invulTimer<=0 && health.Get() > 0)
        {
            health.Modify(-dmg);
            animator.SetTrigger("Hurt");
            if(hurtSoundClip!= null) AudioManager.PlayClip(hurtSoundClip);
            if (health.Get() == 0)
            {
                animator.SetBool("Dead", true);
                if (hurtbox != null) hurtbox.active = false;
                dead = true;
            }
            else
            {
                invulTimer = invulnerabilityDuration;
            }
        }
    }

    public virtual void OnDeath()
    {
        Character.OnCharacterDeath(this);
        GameManager.GetPlayer().GetComponent<PlayerController>().ModifyLust(-lustRelief);
        gameObject.SetActive(activeAfterDeath);
    }

    public Meter GetHealth()
    {
        return health;
    }


    public virtual void Reset()
    {
        animator.SetBool("Dead", false);
        sprite.color = Color.white;
        if (hurtbox != null) hurtbox.active = true;
        dead = false;
        health.Set(health.GetMax());
        rb2d.position = initialPosition;
    }
}
