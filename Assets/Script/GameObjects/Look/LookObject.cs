using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookObject : MonoBehaviour
{
    private Collider collider;

    public Collider GetCollider() { return collider; }

    private Rigidbody rigidbody;

    public Rigidbody GetRigidbody() {  return rigidbody; }

    private int destroyCount = 3;

    private void Start()
    {
        collider = GetComponentInChildren<Collider>();
        if(collider == null)
        {
            Debug.Log("colliderがアタッチされていません");
        }
        rigidbody = GetComponent<Rigidbody>();
        if(rigidbody == null)
        {
            Debug.Log("rigidbodyがアタッチされていません");
        }
    }

    public void UnLock()
    {
        collider.isTrigger = false;
        rigidbody.useGravity = true;
        Destroy(gameObject,destroyCount);
    }
}
