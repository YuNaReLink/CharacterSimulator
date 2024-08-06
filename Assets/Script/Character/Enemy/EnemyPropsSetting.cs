using UnityEngine;

public class EnemyPropsSetting : PropsSettingBase
{
    [SerializeField]
    private Collider        damageCollider;
    public Collider         GetDamageCollider() { return damageCollider; }

    public void ActiveDamageCollider(bool _enabled)
    {
        if(damageCollider == null) { return; }
        damageCollider.enabled = _enabled;
    }

    public bool IsActiveDamageCollider() { return damageCollider.enabled; }
}
