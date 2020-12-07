using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering.Universal;

public class RadialAura : Aura
{
    public float radius;
    private Light2D glow;
    private CircleCollider2D auraCircle;

    protected override void Start()
    {
        base.Start();
        glow = GetComponent<Light2D>();
        glow.pointLightOuterRadius = radius* 1.125f;
        auraCircle = GetComponent<CircleCollider2D>();
        auraCircle.radius = radius;
    }


    protected override float GetStrength(Collider2D playerCol)
    {
        if (playerCol.bounds.Contains(transform.position))
        {
            return strength;
        }
        else
        {
            float distance = (playerCol.ClosestPoint(transform.position)-(Vector2)transform.position).magnitude;

            float resultantStrength = strength * (1 - (distance / radius));
            return resultantStrength;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

#if UNITY_EDITOR
    [ContextMenu("AutoAdjuctComponents")]
    void AdjustColliderAndLight()
    {
        glow = GetComponent<Light2D>();
        glow.pointLightOuterRadius = radius * 1.125f;
        auraCircle = GetComponent<CircleCollider2D>();
        auraCircle.radius = radius;
        PrefabUtility.RecordPrefabInstancePropertyModifications(glow);
        PrefabUtility.RecordPrefabInstancePropertyModifications(auraCircle);

    }
#endif
}
