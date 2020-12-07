using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour
{
    [SerializeField]protected float strength;
    private PlayerController player;

    protected virtual void Start()
    {
        player = GameManager.GetPlayer().GetComponent<PlayerController>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject == GameManager.GetPlayer())
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.ModifyLust(GetStrength(other) * Time.deltaTime);
            
        }
    }

    protected virtual float GetStrength(Collider2D playerCol)
    {
        return strength;
    }
}
