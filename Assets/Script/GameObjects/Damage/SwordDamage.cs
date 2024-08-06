using System;
using UnityEditor;
using UnityEngine;

public class SwordDamage : BaseDamageObject
{
    [SerializeField]
    private bool hitGuardFlag = false;
    public bool HitGuardFlag{ get { return hitGuardFlag; }set { hitGuardFlag = value; } }
    public override DamageType GetDamageType()
    {
        return DamageType.Sword;
    }

    public override void OnTriggerEnter(UnityEngine.Collider other)
    {
        base.OnTriggerEnter(other);
        hitGuardFlag = false;
        if(other.tag == "Guard")
        {
            hitGuardFlag = true;
        }
    }
}
