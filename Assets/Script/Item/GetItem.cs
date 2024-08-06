using UnityEngine;

public class GetItem :ItemExecute
{
    private GenerateEffects effect;
    void Start()
    {
        if(effect == null)
        {
            effect = GetComponent<GenerateEffects>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player") { return; }
        effect.GenerateEffect(0, transform.position);
        ClearFlag.SetClearFlag(true);
        SelfDestroy();
    }
}
