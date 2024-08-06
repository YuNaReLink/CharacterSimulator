using UnityEditor;

public class BodyAttack : BaseDamageObject
{
    public override DamageType GetDamageType()
    {
        return DamageType.BodyAttack;
    }
}
