using System.Collections.Generic;
using UnityEngine;

public class FocusObject : MonoBehaviour
{
    [SerializeField]
    private PlayerController    playerController;
    [SerializeField]
    private SphereCollider      focusArea;

    private Ray                 ray;
    private RaycastHit          hit;
    //Ray���΂�����
    private Vector3             direction;
    //Ray���΂�����
    [SerializeField]
    private float               distance = 10f;
    //���ڂ��邽�߂̃t���O
    private bool                focusFlag = false;
    public bool                 IsFocusFlag() { return focusFlag; }
    //���ڂ�����W��ێ��������
    private GameObject          lockObject;
    //���ڂ����G�̏���ێ��������
    private EnemyBase           enemy;
    public Vector3 GetFocusObjectPosition() 
    {
        if(lockObject == null)
        {
            return Vector3.zero;
        }
        return lockObject.transform.position; 
    }
    public bool                 FocusFlag {  get { return focusFlag; } set {  focusFlag = value; } }


    private void Start()
    {
        focusArea = GetComponent<SphereCollider>();
        if (focusArea == null)
        {
            Debug.Log("focusArea���A�^�b�`����܂���ł���(Enemy)");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag != "FocusPoint"){ return; }
        //Ray���΂��������v�Z
        Vector3 temp = other.transform.position - transform.position;
        direction = temp.normalized;
        //Ray���΂�
        ray = new Ray(transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);  // Ray���V�[����ɕ`��
        //��l���̕���
        var playerDirection = other.transform.position - transform.position;
        //�v���C���[�̑O������̎�l���̕���
        var angle = Vector3.Angle(transform.forward, playerDirection);
        //�T�[�`����p�x���������甭��
        // Ray���ŏ��ɓ����������̂𒲂ׂ�
        if (Physics.Raycast(ray.origin, ray.direction * distance, out hit))
        {
            if (hit.collider.CompareTag("FocusPoint"))
            {
                CheckSameEnemy(other);
            }
        }
    }

    private void CheckSameEnemy(Collider other)
    {
        if(lockObject != null) { return; }
        enemy = other.GetComponentInParent<EnemyBase>();
        enemy.GetSetFocusByMeFlag = true;
        focusFlag = true;
        lockObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FocusPoint")
        {
            RemoveEnemyList(other);
        }
    }

    private void RemoveEnemyList(Collider other)
    {
        if(lockObject == null) { return; }
        if(lockObject == other.gameObject)
        {
            enemy.GetSetFocusByMeFlag = false;
            focusFlag = false;
            lockObject = null;
        }
    }
/*
#if UNITY_EDITOR
    //�@�T�[�`����p�x�\��
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -focusAngle, 0f) * transform.forward, focusAngle * 2f, focusArea.radius / 2.5f);
    }
#endif
*/
}
