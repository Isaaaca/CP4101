using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuffProjectile : Projectile
{
    
    public float duration = 2f;
    private float timer = 0f;


    public override void Fire(Vector2 startPos, Vector2 dir)
    {
        base.Fire(startPos, dir);
        timer = duration;

    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {

                if (animator != null)
                {
                    animator.Play("Destroy");
                }
                else
                {
                    OnDestroyAnimFinish();
                }
            }
        }
    }

}
