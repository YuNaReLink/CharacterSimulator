using UnityEditor;
using UnityEngine;


//鍵を取得するためのクラス
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
