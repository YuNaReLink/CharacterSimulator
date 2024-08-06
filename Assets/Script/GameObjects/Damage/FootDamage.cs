using UnityEditor;

public class FootDamage : BaseDamageObject
{
    public override DamageType GetDamageType()
    {
        return DamageType.Foot;
    }
}
