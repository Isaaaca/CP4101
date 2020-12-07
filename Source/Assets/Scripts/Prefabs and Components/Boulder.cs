using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : Projectile
{
    protected override void Start()
    {
        base.Start();
    }
    public override void OnDestroyAnimFinish()
    {
        CamController.Instance.CameraShake(0.2f, .8f);
        base.OnDestroyAnimFinish();
    }
}
