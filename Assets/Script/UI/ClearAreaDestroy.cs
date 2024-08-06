using UnityEngine;

public class ClearAreaDestroy : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player") { return; }
        ClearFlag.SetClearFlag(true);
        Destroy(gameObject);
    }
}
