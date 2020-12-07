using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemyController : Character
{

    [SerializeField] private Transform groundDetection = null;
    [SerializeField] private bool goOnSlope = false;
    private int layer_mask;

    private void Start()
    {
        layer_mask = LayerMask.GetMask("Obstacle");
    }
    protected override void Update()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down,0.3f,layer_mask);
        Collider2D wallInfo = Physics2D.OverlapPoint(groundDetection.position,layer_mask);
        if (groundInfo.collider == false || wallInfo!=false || (!goOnSlope && groundInfo.normal.y!=1))
        {
            controller.Turn();
        }
        controller.HoriMove(controller.IsFacingRight() ? 1: -1);

        base.Update();
    }
}