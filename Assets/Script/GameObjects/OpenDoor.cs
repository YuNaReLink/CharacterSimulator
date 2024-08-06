using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject movePosition;
    [SerializeField]
    private PlayerHitExecute hitExecute;
    [SerializeField]
    private Collider doorCollider;

    [SerializeField]
    private float playerSpeed = 3f;

    //開閉扉に必要なフラグ
    private bool start = false; 
    private bool open = false;
    private bool movePlayer = false;
    private bool close = false;

    [SerializeField]
    private Vector3 baseDoorPos = Vector3.zero;

    [SerializeField]
    private Vector3 moveDoorPos = new Vector3(0, 7, 0);

    [SerializeField]
    private float openSpeed = 10f;

    private GameObject player;
    private PlayerController controller;

    //錠前オブジェクト
    [SerializeField]
    private LookObject lookObject;


    private void Start()
    {
        if(movePosition != null)
        {
            hitExecute = movePosition.GetComponent<PlayerHitExecute>();
        }

        lookObject = GetComponentInChildren<LookObject>();

        open = false;
        movePlayer = false;
        close = false;

        start = false;

        baseDoorPos = transform.localPosition;
    }

    private void Update()
    {
        //0番目
        if (!open&&!movePlayer&&!close)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                start = true;
            }
        }



        //2番目
        if (open)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveDoorPos, openSpeed * Time.deltaTime);
            //3番目
            Vector3 doorpos = transform.localPosition;
            if(doorpos.y >= moveDoorPos.y)
            {
                transform.localPosition = new Vector3(0, moveDoorPos.y, 0);
                open = false;
                movePlayer = true;
            }
        }

        if (!movePlayer)
        {
            if (hitExecute.PlayerHit)
            {
                Vector3 changePos = movePosition.transform.localPosition;
                changePos.z *= -1;
                movePosition.transform.localPosition = changePos;
                hitExecute.PlayerHit = false;
            }
        }
        else
        {
            //4番目
            Vector3 lookPos = movePosition.transform.position;
            lookPos.y = player.transform.position.y;
            player.transform.LookAt(lookPos);
            player.transform.position = Vector3.MoveTowards(player.transform.position, movePosition.transform.position, playerSpeed * Time.deltaTime);
            controller.ChangeMotionState(CharacterManager.ActionState.Run);
            //5番目
            if (player.transform.position == movePosition.transform.position)
            {
                movePlayer = false;
                close = true;
            }
        }
        

        //6番目
        if (close)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, baseDoorPos, openSpeed * Time.deltaTime);
            if(transform.localPosition.y <= baseDoorPos.y)
            {
                doorCollider.isTrigger = false;
                transform.localPosition = new Vector3(0,baseDoorPos.y,0);
                close = false;
                controller.MoveStopFlag = false;
                hitExecute.enabled = true;
                player = null;
                controller = null;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.tag != "Player") { return; }
        player = other.gameObject;
        controller = player.GetComponent<PlayerController>();
        //1番目
        bool startOpenTheDoor = !open && !movePlayer && !close;
        if (startOpenTheDoor)
        {
            if(start)
            {
                if(lookObject != null)
                {
                    if(GameDataManager.GetKeyStates().Count <= 0)
                    {
                        return;
                    }
                    GameDataManager.GetKeyStates().Count--;
                    SEController se = GetComponent<SEController>();
                    se.SEPlay(0);
                    lookObject.UnLock();
                    lookObject.gameObject.transform.parent = null;
                    lookObject = null;
                }


                doorCollider.isTrigger = true;
                open = true;
                hitExecute.enabled = false;
                controller.MoveStopFlag = true;
                controller.ChangeMotionState(CharacterManager.ActionState.Idle);
                start = false;
                return;
            }
        }
    }
}
