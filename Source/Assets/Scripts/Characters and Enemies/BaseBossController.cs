using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBossController : Character
{
    [Header("Character Settings")]
    [SerializeField] protected float phase2TriggerPoint = 50;
    [SerializeField] protected float roarDuration = 1.5f;
    [SerializeField] protected float roarIntensity = 3f;

    protected GameObject player;
    protected float timer = 1f;
    protected bool inCombatMode = true;
    protected bool phase2Triggered = false;

    protected override void Awake()
    {
        base.Awake();
        GameManager.SetGameplayEnabled += SetCombatEnabled;
    }

    protected virtual void Start()
    {
        player = GameManager.GetPlayer();

    }

    private void OnDestroy()
    {
        GameManager.SetGameplayEnabled -= SetCombatEnabled;
    }

    private void SetCombatEnabled(bool isEnabled)
    {
        inCombatMode = isEnabled;
    }


    // Update is called once per frame
    protected override void Update()
    {
        if (inCombatMode)
        {
            animator.SetBool("Running", Mathf.Abs(controller.GetVelocity().x) > 0 || controller.IsDashing());

            if (health.Get() > 0)
            {
                if (!phase2Triggered)
                {
                    Phase1();
                }
                else
                {
                    Phase2();
                }
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            base.Update();
        }
    }

    public override void TakeDamage(float dmg, Vector2 source)
    {
        if (invulTimer <= 0 && health.Get() > 0 && !animator.GetBool("Roar"))
        {
            if (hurtSoundClip != null) AudioManager.PlayClip(hurtSoundClip);
            health.Modify(-dmg);
            animator.SetTrigger("Hurt");
            if (health.Get() == 0)
            {
                controller.Halt();
                animator.SetBool("Dead", true);
                if (hurtbox != null) hurtbox.active = false;
                dead = true;
            }
            else
            {
                invulTimer = invulnerabilityDuration;
            }

            if (!phase2Triggered && health.Get() <= phase2TriggerPoint)
            {
                animator.SetBool("Roar",true);
                timer = roarDuration;
                phase2Triggered = true;
                TriggerPhase2();
            }
        }
    }


    private void Roar()
    {
        CamController.Instance.CameraShake(roarDuration, roarIntensity);
        Invoke("StopRoar", timer + .2f);
    }

    private void StopRoar()
    {
        animator.SetBool("Roar", false);
    }


    protected abstract void Phase1();
    protected abstract void Phase2();
    protected abstract void TriggerPhase2();

    protected bool InMidAction()
    {
        return controller.IsArcing() || controller.IsDashing() || timer > 0;
    }

    public override void Reset()
    {
        phase2Triggered = false;
        base.Reset();
    }
}
