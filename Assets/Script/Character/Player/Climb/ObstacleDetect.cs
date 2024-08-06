using UnityEngine;
using static CharacterManager;

public class ObstacleDetect : MonoBehaviour
{

    [SerializeField]
    private float                   stepCheckOffSet = 0.5f;
    [SerializeField]
    private float                   stepCheckDistance = 0.7f;
    
    private bool                    lowStep = false;
    public bool                     LowStep() { return lowStep; }

    [SerializeField]
    private float                   wallUpCheckOffset = 2f;
    [SerializeField]
    private float                   wallMiddleCheckOffset = 1f;
    [SerializeField]
    private float                   wallBottomCheckOffset = 0.25f;
    [SerializeField]
    private float                   upperWallCheckOffset = 3.5f;
    [SerializeField]
    private float                   wallCheckDistance = 0.5f;

    [SerializeField]
    private float                   wallJumpPower;

    public void                     SetWallJumpPower(float _power) { wallJumpPower = _power; }

    [SerializeField]
    private float                   climbForward = 1.5f;

    [SerializeField]
    private float                   climbUp = 2.35f;

    //段差ジャンプフラグ
    private bool                    stepJumpFlag = false;
    public bool                     IsStepJumpFlag() {  return stepJumpFlag; }

    private bool                    noGarbToClimbFlag = false;

    //高い壁を登るジャンプフラグ
    private bool                    wallJumpFlag = false;

    private bool                    grabFlag = false;

    public bool                     IsGrabFlag() {  return grabFlag; }

    private bool                    climbFlag = false;

    private Vector3                 climbOldPos = Vector3.zero;

    private Vector3                 climbPos = Vector3.zero;

    private bool                    upForwardWallFlag = false;

    private bool                    middleWallFlag = false;

    private bool                    bottomForwardWallFlag = false;

    private bool                    upperWallFlag = false;
    //崖をジャンプしたか判定するbool型

    private bool                    cliffJump = false;

    public bool                     CliffJumpFlag { get { return cliffJump; }set { cliffJump = value; } }


    /// <summary>
    /// 壁登り関係のタイマーのカウント
    /// </summary>
    private readonly float stopWallActionCount1 = 0.25f;
    private readonly float stopWallActionCount2 = 0.5f;
    private readonly float stepJumpCoolDownCount = 0.5f;
    private readonly float wallGlabStoperCount1 = 1f;
    private readonly float wallGlabStoperCount2 = 0.25f;


    public void Execute(PlayerController controller)
    {
        //タイマーが作動中は実行しない
        if (controller.GetTimer().Timer_StopWallAction.IsEnabled()) { return; }
        //段差を飛び越える動作
        StepJumpCommand(controller);
        //高い壁に向かってする動作
        WallJumpCommand(controller);
        //壁に掴まる動作
        GrabCommand(controller);
        //壁を登る動作
        Climb(controller);
    }

    private bool MoveKeyInput(PlayerController controller)
    {
        if (controller.GetStateInput().IsUpKey())
        {
            return true;
        }
        return false;
    }

    //  イージング関数
    private float Ease(float x)
    {
        return x * x * x;
    }

    private void StepCheck()
    {
        lowStep = false;
        //プレイヤーの前に段差があるかを確認
        Ray stepCheckRay = new Ray(transform.position + transform.forward * stepCheckOffSet,-transform.up);

        lowStep = Physics.Raycast(stepCheckRay, stepCheckDistance);
        Debug.DrawRay(stepCheckRay.origin, stepCheckRay.direction * stepCheckDistance, Color.red);
    }

    private bool WallCheck()
    {
        //  壁判定に使用する変数
        Ray wallUpCheckRay = new Ray(transform.position + Vector3.up * wallUpCheckOffset, transform.forward);
        Ray wallMiddleCheckRay = new Ray(transform.position + Vector3.up * wallMiddleCheckOffset, transform.forward);
        Ray wallBottomCheckRay = new Ray(transform.position + Vector3.up * wallBottomCheckOffset, transform.forward);
        Ray upperCheckRay = new Ray(transform.position + Vector3.up * upperWallCheckOffset, transform.forward);
        //  壁判定を格納
        RaycastHit[] hit = new RaycastHit[4];
        upForwardWallFlag = Physics.Raycast(wallUpCheckRay, out hit[0], wallCheckDistance);
        middleWallFlag = Physics.Raycast(wallMiddleCheckRay, out hit[1], wallCheckDistance);
        bottomForwardWallFlag = Physics.Raycast(wallBottomCheckRay, out hit[2], wallCheckDistance);
        upperWallFlag = Physics.Raycast(upperCheckRay, out hit[3], wallCheckDistance);
        
        Debug.DrawRay(wallUpCheckRay.origin, wallUpCheckRay.direction * wallCheckDistance, Color.red);
        Debug.DrawRay(wallMiddleCheckRay.origin, wallMiddleCheckRay.direction * wallCheckDistance, Color.red);
        Debug.DrawRay(wallBottomCheckRay.origin, wallBottomCheckRay.direction * wallCheckDistance, Color.red);
        Debug.DrawRay(upperCheckRay.origin, upperCheckRay.direction * wallCheckDistance, Color.red);
        //レイキャストで当たっているものがなかったらリターン
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider == null) { continue; }
            //当たったものがもし敵だったら
            if (hit[i].collider.gameObject.tag == "WallFloor"||
                hit[i].collider.gameObject.tag == "Foot") 
            {
                continue; 
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void WallActionInput(PlayerController controller)
    {
        //段差を確認
        StepCheck();
        //壁があるかチェック、なかったら早期リターン
        if (!WallCheck()) { return; }
        //タイマーが作動していたら早期リターン
        if (controller.GetTimer().Timer_StopWallAction.IsEnabled()) { return; }
        if (climbFlag) { return; }
        //掴まり中か登り中なら早期リターン
        if (grabFlag) 
        {
            InitializeFlag();
            InitializeState(controller);
            return; 
        }
        else
        {
            if(controller.GetCurrentState() == ActionState.Grab)
            {
                controller.ChangeMotionState(ActionState.Idle);
            }
        }
        //段差ジャンプか直登り中か壁ジャンプ中なら早期リターン
        if(stepJumpFlag|| noGarbToClimbFlag||wallJumpFlag) { return; }

        switch (controller.GetTag())
        {
            case DataTag.Zelda:
                WallZeldaActionInput(controller);
                break;
            case DataTag.RatchetAndClank:
            case DataTag.SuperMario:
                if (!controller.Landing&&
                    !upForwardWallFlag && middleWallFlag &&
                    bottomForwardWallFlag && !upperWallFlag)
                {
                    grabFlag = true;
                }
                break;
        }
        //フラグ初期化
        InitializeFlag();
    }

    private void WallZeldaActionInput(PlayerController controller)
    {
        if (controller.Landing)
        {
            if (!controller.GetTimer().Timer_StepJumpCoolDown.IsEnabled())
            {
                //条件に合うなら段差ジャンプフラグをtrueに
                if (!upForwardWallFlag && !middleWallFlag &&
                    bottomForwardWallFlag &&
                    !upperWallFlag)
                {
                    controller.GetTimer().Timer_StopWallAction.StartTimer(stopWallActionCount1);
                    controller.GetTimer().Timer_StopWallAction.OnCompleted += () => {
                        stepJumpFlag = true;
                        InitializeFlag(); 
                    };
                }
            }
            //条件に合うなら直登りフラグをtrueに
            if (!upForwardWallFlag && middleWallFlag && bottomForwardWallFlag &&
                !upperWallFlag)
            {
                controller.GetTimer().Timer_StopWallAction.StartTimer(stopWallActionCount2);
                controller.GetTimer().Timer_StopWallAction.OnCompleted += () => { InitializeFlag(); };
                noGarbToClimbFlag = true;
            }

            //条件に合うなら壁ジャンプフラグをtrueに
            if (upForwardWallFlag && middleWallFlag && bottomForwardWallFlag &&
                !upperWallFlag)
            {
                controller.GetTimer().Timer_StopWallAction.StartTimer(stopWallActionCount2);
                controller.GetTimer().Timer_StopWallAction.OnCompleted += () => { InitializeFlag(); };
                wallJumpFlag = true;
            }
        }
        else
        {
            if (controller.GetTimer().Timer_WallGlabStoper.IsEnabled()) { return; }
            if (bottomForwardWallFlag && middleWallFlag &&
                !upForwardWallFlag && !upperWallFlag)
            {
                grabFlag = true;
            }
        }
    }

    private void InitializeFlag()
    {
        if (!upForwardWallFlag && !middleWallFlag && !bottomForwardWallFlag &&
            !upperWallFlag)
        {
            stepJumpFlag = false;
            wallJumpFlag = false;
            grabFlag = false;
            noGarbToClimbFlag = false;
        }
    }

    private void InitializeState(PlayerController controller)
    {
        if (!controller.Landing&&!upForwardWallFlag &&
            !middleWallFlag && !bottomForwardWallFlag &&
            !upperWallFlag)
        {
            controller.GetCharacterRb().useGravity = true;
            controller.ChangeMotionState(ActionState.Idle);
        }
    }

    private void StepJumpCommand(PlayerController controller)
    {
        if (!stepJumpFlag || !controller.InputFlag) { return; }
        controller.GetTimer().Timer_StepJumpCoolDown.StartTimer(stepJumpCoolDownCount);
        controller.GetCharacterRb().velocity = Vector3.zero;
        controller.ChangeMotionState(ActionState.Jump);
        controller.JumpForce(wallJumpPower);
        controller.GetSEController().StepJumpSEPlay();
        stepJumpFlag = false;
    }

    private void WallJumpCommand(PlayerController controller)
    {
        if (controller.GetTimer().Timer_WallGlabStoper.IsEnabled()) { return; }
        if (!wallJumpFlag || !controller.InputFlag) { return; }
        controller.ChangeMotionState(ActionState.Jump);
        controller.JumpForce(wallJumpPower);
        controller.GetTimer().Timer_WallGlabStoper.StartTimer(wallGlabStoperCount2);
        grabFlag = true;
        wallJumpFlag = false;
    }

    private void GrabCommand(PlayerController controller)
    {
        if (controller.GetTimer().Timer_WallGlabStoper.IsEnabled()||
            !grabFlag || !bottomForwardWallFlag || upperWallFlag)
        {
            return; 
        }
        if (controller.GetCharacterRb().useGravity)
        {
            controller.GetCharacterRb().useGravity = false;
            controller.GetSEController().GrabSEPlay();
        }
        controller.GetStateMovement().StopVelocityXYZ();
        controller.ChangeMotionState(ActionState.Grab);
        if (MoveKeyInput(controller))
        {
            SetClimbPostion();
        }
        else if(controller.GetStateInput().IsDownKey())
        {
            controller.GetCharacterRb().useGravity = true;
            grabFlag = false;
            controller.GetTimer().Timer_WallGlabStoper.StartTimer(wallGlabStoperCount1);
        }
    }

    private void SetClimbPostion()
    {
        //  開始位置を保持
        climbOldPos = transform.position;
        //  終了位置を算出
        climbPos = transform.position + transform.forward * climbForward + Vector3.up * climbUp;
        //  掴みを解除
        grabFlag = false;
        //  よじ登りを実行
        climbFlag = true;
        //直登りフラグを解除
        noGarbToClimbFlag = false;
    }

    private void Climb(PlayerController controller)
    {
        if (noGarbToClimbFlag)
        {
            SetClimbPostion();
            controller.GetCharacterRb().useGravity = false;
        }

        if (!climbFlag) { return; }
        controller.ChangeMotionState(ActionState.ClimbWall);

        //  よじ登りモーションの進行度を取得
        AnimatorStateInfo animInfo = controller.GetAnim().GetCurrentAnimatorStateInfo(0);
        if (!animInfo.IsName("hangToCrouch")) { return; }
        float f = animInfo.normalizedTime;
        //  左右は後半にかけて早く移動する
        float x = Mathf.Lerp(climbOldPos.x, climbPos.x, Ease(f));
        float z = Mathf.Lerp(climbOldPos.z, climbPos.z, Ease(f));
        //  上下は等速直線で移動
        float y = Mathf.Lerp(climbOldPos.y, climbPos.y, f);

        //  座標を更新
        transform.position = new Vector3(x, y, z);
        controller.GetCharacterRb().useGravity = false;
        //  進行度が8割を超えたらよじ登りの終了
        if (f >= 0.8f)
        {
            climbFlag = false;
            controller.GetCharacterRb().useGravity = true;
            controller.ChangeMotionState(ActionState.Idle);
        }
    }

    public bool CheckWallNoHit()
    {
        bool checkwallnohit = !upForwardWallFlag && !middleWallFlag && !bottomForwardWallFlag && !upperWallFlag;
        return checkwallnohit ? true : false;
    }
}
