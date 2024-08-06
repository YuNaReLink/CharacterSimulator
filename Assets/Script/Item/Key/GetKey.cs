using UnityEditor;
using UnityEngine;


//Œ®‚ðŽæ“¾‚·‚é‚½‚ß‚ÌƒNƒ‰ƒX
public class GetKey : ItemExecute
{
    private GenerateEffects effect;
    [SerializeField]
    private Collider collider;
    void Start()
    {
        effect = GetComponent<GenerateEffects>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") { return; }
        if(collider != null)
        {
            collider.enabled = false;
        }
        SelfDestroy();
        effect.GenerateEffect(0, transform.position);
        GameDataManager.GetKeyStates().Count++;
    }
}
