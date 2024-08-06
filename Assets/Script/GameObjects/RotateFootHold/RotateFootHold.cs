using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��]����I�u�W�F�N�g�ɏ�����I�u�W�F�N�g�𓯂�����]�A��]�������̈ړ���K�p����N���X
/// </summary>
public class RotateFootHold : MonoBehaviour
{
    [Header("�K�v�ȃR���|�[�l���g��o�^")]
    [SerializeField]
    private Rigidbody rigidbody = null;

    [Header("��{�ݒ�")]
    [SerializeField]
    private Vector3 speed = Vector3.zero;

    private List<Rigidbody> rigidbodies = new();

    private Quaternion baseRotation = new Quaternion();
    private Vector3 basePosition = Vector3.zero;


    private void Start()
    {
        baseRotation = transform.rotation;
        basePosition = transform.position;
    }

    private void FixedUpdate()
    {
        AddRotation();
    }


    private Quaternion GetDeltaRotation()
    {
        Quaternion deltaRotation = Quaternion.Inverse(baseRotation) * transform.rotation;
        baseRotation = transform.rotation;
        return deltaRotation;
    }

    private Vector3 GetDeltaPosition()
    {
        Vector3 deltaPosition = transform.position - basePosition;
        basePosition = transform.position;
        return deltaPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        rigidbodies.Add(other.gameObject.GetComponent<Rigidbody>());
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
        if(rigidbody == null) { return; }
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            if (rigidbodies[i] != rigidbody) { continue; }
            rigidbodies[i].position = rigidbody.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        rigidbodies.Remove(other.gameObject.GetComponent<Rigidbody>());
    }
    private void AddRotation()
    {

        Quaternion deltaRotation = GetDeltaRotation();
        Vector3 deltaPosition = GetDeltaPosition();
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            if (rigidbodies[i] == null)
            {
                continue;
            }
            if (rigidbodies[i].velocity.sqrMagnitude >= 0.01f)
            {
                continue;
            }
            rigidbodies[i].MoveRotation(rigidbodies[i].rotation * deltaRotation);

            // ���Έʒu���v�Z
            Vector3 relativePosition = rigidbodies[i].position - transform.position;
            Vector3 rotatedPosition = transform.position + deltaRotation * relativePosition;

            // �ړ���K�p
            rigidbodies[i].MovePosition(rotatedPosition);
        }
    }
}
