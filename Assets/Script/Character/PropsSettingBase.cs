using UnityEngine;

public class PropsSettingBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject    damageObject;
    public GameObject       GetDamageObject() { return damageObject; }

    [SerializeField]
    protected GameObject    shild;
    public GameObject       GetShild() { return shild; }
    [SerializeField]
    protected Collider      shildCollider;

    protected virtual void Start()
    {
        if(damageObject == null)
        {
            Debug.LogError("damageObjectがアタッチされていません");
        }
    }

    public void ActiveDamageObject(bool _enabled)
    {
        if(damageObject == null) { return; }
        damageObject.SetActive(_enabled);
    }
    public bool IsActiveDamageObject() { return damageObject.activeSelf; }

    public void ActiveShild(bool _enabled)
    {
        if (shild == null) { return; }
        shild.SetActive(_enabled);
    }
    public bool IsActiveShild() { return shild.activeSelf; }

    public void ActiveShildCollider(bool _enabled)
    {
        if (shildCollider == null) { return; }
        shildCollider.enabled = _enabled;
    }

    public bool IsActiveShildCollider() { return shildCollider.enabled; }
}
