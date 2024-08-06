using UnityEditor;

public class ArmDamage : BaseDamageObject
{
    public override DamageType GetDamageType()
    {
        return DamageType.Arm;
    }
}
