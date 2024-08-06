using UnityEngine;

public class BaseDamageObject : MonoBehaviour
{
    [SerializeField]
    protected float damage;
    public float GetSetDamage { get { return damage; } set { damage = value; } }
    
    public enum DamageType
    {
        Null = 0,
        Sword,
        Foot,
        Arm,
        Bullet,
        BodyAttack,
        Trap,
        DataEnd,
    }
    public virtual DamageType GetDamageType() { return DamageType.Null; }
    [SerializeField]
    private bool wallHit = false;

    public bool IsWallHit() { return wallHit; }
    public bool SetWallHit(bool _enabled) { return wallHit = _enabled; }
    public virtual void OnTriggerEnter(Collider other)
    {
        wallHit = false;
        if (other.tag != "Obstacle") {  return; }
        wallHit = true;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag != "Obstacle") { return; }
        wallHit = false;
    }

}
