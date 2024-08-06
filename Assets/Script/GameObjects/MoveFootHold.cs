using System.Collections.Generic;
using UnityEngine;

public class MoveFootHold : MonoBehaviour
{
    [Header("必要なコンポーネントを登録")]
    [SerializeField]
    private Rigidbody rigidbody = null;

    [Header("基本設定")]
    [SerializeField]
    private Vector3 speed = Vector3.zero;

    private List<Rigidbody> rigidbodies = new();

    private Vector3 basePosition = Vector3.zero;

    [SerializeField]
    private float addTargetPostion = 2;


    private void Start()
    {
        basePosition = transform.position;
    }

    private void FixedUpdate()
    {
        MovePlatform();
        AddVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        rigidbodies.Add(other.gameObject.GetComponent<Rigidbody>());
    }

    private void OnTriggerExit(Collider other)
    {
        rigidbodies.Remove(other.gameObject.GetComponent<Rigidbody>());
    }

    private void MovePlatform()
    {
        rigidbody.MovePosition(new Vector3(basePosition.x, basePosition.y + Mathf.PingPong(Time.time, addTargetPostion), basePosition.z));
    }
    private void AddVelocity()
    {
        if(rigidbody.velocity.sqrMagnitude <= 0.01f)
        {
            return;
        }

        for(int i = 0; i < rigidbodies.Count; i++)
        {
            if (rigidbodies[i] == null)
            {
                continue;
            }
            rigidbodies[i].AddForce(rigidbody.velocity, ForceMode.VelocityChange);
        }
    }
}
