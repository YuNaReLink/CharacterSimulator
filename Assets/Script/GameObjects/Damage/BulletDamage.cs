using UnityEditor;


public class BulletDamage : BaseDamageObject
{
    public override DamageType GetDamageType()
    {
        return DamageType.Bullet;
    }
}
