using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveTrap : MonoBehaviour
{
    //進む向き
    [SerializeField]
    private Vector3 moveDirection = new Vector3(0,0,1);
    //移動スピード
    [SerializeField]
    private float moveSpeed = 25f;
    //開始座標
    [SerializeField]
    private Vector3 basePosition = Vector3.zero;
    //光線の距離
    [SerializeField]
    private float rayDistance = 100f;
    //動くかのフラグ
    private bool moveForward = false;
    //光線に何か当たったかのフラグ
    private bool rayHitWall = false;
    //オブジェクトの回転を行うクラス
    [SerializeField]
    private RotateObject rotateObject;

    // Start is called before the first frame update
    void Start()
    {
        basePosition = transform.position;
        moveForward = false;
        rayHitWall = false;

        rotateObject = GetComponentInChildren<RotateObject>();
        if(rotateObject != null)
        {
            rotateObject.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーを見つける・移動する・開始位置に戻る、の三点処理
        if (!moveForward&&!rayHitWall)
        {
            CheckRayHitByPlayer();
        }
        else if(moveForward&&!rayHitWall)
        {
            MoveForward();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, basePosition,Time.deltaTime);
            Vector3 sub = transform.position - basePosition;
            float distance = sub.magnitude;
            if(distance <= 0.1f)
            {
                rayHitWall = false;
                rotateObject.enabled = false;
            }
        }
    }

    private void CheckRayHitByPlayer()
    {
        Ray playerCheckForwardRay = new Ray(transform.position,transform.forward);
        Ray playerCheckBackRay = new Ray(transform.position,-transform.forward);
        Ray playerCheckRightRay = new Ray(transform.position,transform.right);
        Ray playerCheckLeftRay = new Ray(transform.position,-transform.right);

        Ray[] rays = new Ray[]
        {
            playerCheckForwardRay,
            playerCheckBackRay,
            playerCheckRightRay,
            playerCheckLeftRay,
        };

        for(int i = 0;i < rays.Length; i++)
        {
            // 配列を用意しておき、そこに結果が格納する
            RaycastHit[] hits = new RaycastHit[5];

            // Rayの原点がorigin, Rayの方向がdirectoin, maxDistanceがヒットが発生する最大距離
            Physics.RaycastNonAlloc(rays[i], results: hits, rayDistance);
            Debug.DrawRay(rays[i].origin, rays[i].direction * rayDistance, Color.red);
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider != null)
                {
                    if (hits[j].collider.tag == "Player")
                    {
                        moveForward = true;
                        switch (i)
                        {
                            case 0:
                                moveDirection = new Vector3(0, 0, 1);
                            break;
                            case 1:
                                moveDirection = new Vector3(0, 0, -1);
                            break;
                            case 2:
                                moveDirection = new Vector3(1, 0, 0);
                            break;
                            case 3:
                                moveDirection = new Vector3(-1, 0, 0);
                            break;
                        }
                        return;
                    }
                }
            }
        }

    }

    private void MoveForward()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        rotateObject.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"|| other.tag == "FocusSight"||
           other.tag == "Untagged") { return; }
        moveForward = false;
        rayHitWall = true;
    }
}
