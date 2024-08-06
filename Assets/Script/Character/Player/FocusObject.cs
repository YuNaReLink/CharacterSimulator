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
    //Rayを飛ばす方向
    private Vector3             direction;
    //Rayを飛ばす距離
    [SerializeField]
    private float               distance = 10f;
    //注目するためのフラグ
    private bool                focusFlag = false;
    public bool                 IsFocusFlag() { return focusFlag; }
    //注目する座標を保持するもの
    private GameObject          lockObject;
    //注目した敵の情報を保持するもの
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
            Debug.Log("focusAreaがアタッチされませんでした(Enemy)");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag != "FocusPoint"){ return; }
        //Rayを飛ばす方向を計算
        Vector3 temp = other.transform.position - transform.position;
        direction = temp.normalized;
        //Rayを飛ばす
        ray = new Ray(transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);  // Rayをシーン上に描画
        //主人公の方向
        var playerDirection = other.transform.position - transform.position;
        //プレイヤーの前方からの主人公の方向
        var angle = Vector3.Angle(transform.forward, playerDirection);
        //サーチする角度内だったら発見
        // Rayが最初に当たった物体を調べる
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
    //　サーチする角度表示
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -focusAngle, 0f) * transform.forward, focusAngle * 2f, focusArea.radius / 2.5f);
    }
#endif
*/
}
