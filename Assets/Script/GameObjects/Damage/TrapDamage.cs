using UnityEditor;

public class TrapDamage : BaseDamageObject
{
    public override DamageType GetDamageType()
    {
        return DamageType.Trap;
    }
}

