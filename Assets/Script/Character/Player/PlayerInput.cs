using UnityEngine;
using static GameDataManager;
using static PlayerController;
using static CharacterManager;
using static InputManager;

public class PlayerInput : MonoBehaviour
{
    private PlayerController    controller;
    public void SetPlayerController(PlayerController _controller) { controller = _controller; }

    /// <summary>
    /// 入力変数
    /// </summary>
    [SerializeField]
    private float               horizontalinput;
    public float                Horizontalinput { get { return horizontalinput; }set { horizontalinput = value; } }
    
    [SerializeField]
    private float               verticalInput;
    public float                VerticalInput { get { return verticalInput; }set { verticalInput = value; } }

    [SerializeField]
    private bool                upKey;
    public bool                 IsUpKey() {  return upKey; }
    [SerializeField]
    private bool                downKey;
    public bool                 IsDownKey() {  return downKey; }
    [SerializeField]
    private bool                leftKey;
    public bool                 IsLeftKey() {  return leftKey; }
    [SerializeField]
    private bool                rightKey;
    public bool                 IsRightKey() {  return rightKey; }

    public bool AllDirectionMoveKey() { return upKey || downKey || leftKey || rightKey; }

    //以下はそれぞれのキー入力の代入変数
    [SerializeField]
    private bool                jumpUpKey;
    public bool                 IsJumpUpKey() { return jumpUpKey; }
    
    [SerializeField]
    private bool                jumpDownKey;
    public bool                 JumpDownKey { get { return jumpDownKey; }set { jumpDownKey = value; } }
    
    [SerializeField]
    private bool                jumpHoldKey;
    public bool                 JumpHoldKey { get { return jumpHoldKey; }set { jumpHoldKey = value; } }
    
    [SerializeField]
    private bool                crouchKey;
    public bool                 IsCrouchKey() { return crouchKey; }
    
    [SerializeField]
    private bool                shiftKeyDown;
    public bool                 ShiftKeyDown { get { return shiftKeyDown; }set { shiftKeyDown = value; } }
    
    [SerializeField]
    private bool                shiftHoldKey;
    public bool                 IsShiftHoldKey() { return shiftHoldKey; }
    
    [SerializeField]
    private bool                leftCtrlKey;
    public bool                 IsLeftCtrlKey() { return leftCtrlKey; }
    
    //マウス入力変数
    [SerializeField]
    private bool                mouseleftDownClick;
    public bool                 IsMouseLeftDownClick() { return mouseleftDownClick; }
    
    [SerializeField]
    private bool                mouseleftHoldClick;
    public bool                 IsMouseLeftHoldClick() { return mouseleftHoldClick; }
    
    [SerializeField]
    private bool                mouseleftUpClick;
    public bool                 IsMouseLeftUpClick() {  return mouseleftUpClick; }
    
    [SerializeField]
    private bool                mousemiddleClick;
    public bool                 IsMouseMiddle() { return mousemiddleClick;}
    public bool                 SetMouseMiddleClick(bool enabled) {  return mousemiddleClick = enabled; }
    
    [SerializeField]
    private bool                mouserightClick;
    public bool                 IsMouseRightClick() { return mouserightClick; }

    [SerializeField]
    private bool                mouseRightDownClick;
    public bool                 IsMouseRightDownClick() { return mouseRightDownClick; }

    /// <summary>
    /// ヒップドロップ関係
    /// </summary>
    private bool                hipDropFlag = false;
    public bool                 GetHipDropFlag { get { return hipDropFlag; } set { hipDropFlag = value; } }

    private float               pastKeyHorInput;
    private float               pastKeyVerInput;

    private bool                crouchToCrawing = false;

    private bool                gurid = false;
    public bool                 IsGurid { get { return gurid; } set { gurid = value; } }
    private ShieldBlockState    blockState = ShieldBlockState.Null;
    public ShieldBlockState     BlockState { get { return blockState; } set { blockState = value; } }

    private void Start()
    {
        hipDropFlag = false;
        crouchToCrawing = false;
        gurid = false;
    }

    public void AllKeyInput()
    {
        //マウス入力を初期化
        InitMouseKeyInput();
        if (controller.GetTimer().Timer_SpinAttack.IsEnabled() || controller.GetTimer().Timer_BurstAttack.IsEnabled() ||
            blockState == ShieldBlockState.SitBlock || GetHipDropFlag ||
            controller.GetTimer().Timer_StopMove.IsEnabled())
        {
            return;
        }
        //キー入力を初期化
        InitalizeKeyInput();
        KeyInitializeInput();
        //ゲームクリア時のキー初期化処理
        if (CurrentGameState != GameState.NowGame)
        {
            controller.GetStateMovement().StopVelocityXZ();
            GameClearInput();
            return;
        }
        //TODO : 以下アクションモーションの入力処理
        //待機
        IdleInput();
        //走る
        RunInput();
        //匍匐前進
        Crawling();
        //滑空
        GlideJump();
        //ヒップドロップ
        HipDropInput();
        //長ジャンプ
        LongJumpInput();
        //方向転換ジャンプ
        ChangingDirectionJumpInput();
        //通常ジャンプ
        JumpInput();
        //回避
        AllAvoidInput();
        //しゃがみ
        CrouchInput();
        //飛びつき攻撃
        JumpingAttack();
        //武器収納
        ChangeMode();
        //落下
        FallingInput();
    }

    public void KeyInitializeInput()
    {
        if(verticalInput == 0&&horizontalinput == 0 && crouchToCrawing)
        {
            crouchToCrawing = false;
        }
    }

    public void InitMouseKeyInput()
    {
        //マウスの入力検知
        if (controller.GetScriptableObject().AttackPower > 0)
        {
            mouseleftDownClick = Input.GetMouseButtonDown((int)MouseCode.Left);
        }
        if (controller.GetScriptableObject().ReadySpinAttackTimerCount > 0)
        {
            mouseleftHoldClick = Input.GetMouseButton((int)MouseCode.Left);
        }
        if (controller.GetScriptableObject().SpinAttackTimerCount > 0)
        {
            mouseleftUpClick = Input.GetMouseButtonUp((int)MouseCode.Left);
        }
        if (Input.GetMouseButtonDown(2) && controller.GetTag() == DataTag.Zelda)
        {
            mousemiddleClick = !mousemiddleClick;
        }
        if(controller.GetCurrentState() != ActionState.Grab&& controller.GetCurrentState() != ActionState.ClimbWall)
        {
            mouserightClick = Input.GetMouseButton((int)MouseCode.Right);
        }
        mouseRightDownClick = Input.GetMouseButtonDown((int)MouseCode.Right);
    }

    public virtual void InitalizeKeyInput()
    {
        //キーの初期化
        horizontalinput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        upKey = Input.GetKeyDown(KeyCode.W);
        downKey = Input.GetKeyDown(KeyCode.S);
        leftKey = Input.GetKeyDown(KeyCode.A);
        rightKey = Input.GetKeyDown(KeyCode.D);

        //Space入力検知
        if (controller.GetScriptableObject().MaxJumpCount > 0)
        {
            jumpUpKey = Input.GetKeyUp(KeyCode.Space);
            jumpDownKey = Input.GetKeyDown(KeyCode.Space);
            jumpHoldKey = Input.GetKey(KeyCode.Space);
        }
        if (controller.GetTag() != DataTag.Zelda)
        {
            crouchKey = Input.GetKey(KeyCode.C);
        }
        //LeftShift入力検知
        shiftKeyDown = Input.GetKeyDown(KeyCode.LeftShift);
        shiftHoldKey = Input.GetKey(KeyCode.LeftShift);
        //LeftCtrl入力検知
        leftCtrlKey = Input.GetKeyDown(KeyCode.LeftControl);
    }

    private void FallingInput()
    {
        bool noFallingFlag = controller.GetCurrentPos().y > controller.GetPastPos().y;
        if (noFallingFlag) { return; }
        ActionState state = controller.GetCurrentState();
        if (AllJumpState(state)) { return; }
        if(AllObstacleState(state)) { return; }
        if(state == ActionState.HipDrop) { return; }
        if (controller.Landing) { return; }
        controller.ChangeMotionState(ActionState.Falling);
    }

    private void IdleInput()
    {
        if (crouchKey) { return; }
        if (verticalInput != 0 || horizontalinput != 0) { return; }
        switch (controller.GetCurrentState())
        {
            case ActionState.JumpAttack:
            case ActionState.Jump:
            case ActionState.RunJump:
            case ActionState.GlideJump:
            case ActionState.Flip:
            case ActionState.ClimbWall:
            case ActionState.ReadySpinAttack:
                return;
        }
        if (controller.Landing)
        {
            controller.ChangeMotionState(ActionState.Idle);
        }
    }
    public void RunInput()
    {
        if(crouchKey||!controller.Landing) { return; }
        switch (controller.GetCurrentState())
        {
            case ActionState.JumpAttack:
            case ActionState.GlideJump:
            case ActionState.ClimbWall:
            case ActionState.ReadySpinAttack:
                return;
        }
        if (horizontalinput != 0 || verticalInput != 0)
        {
            controller.ChangeMotionState(ActionState.Run);
        }
    }

    public void Crawling()
    {
        if(controller.GetTag() != DataTag.SuperMario) { return; }
        switch (controller.GetCurrentState())
        {
            case ActionState.Jump:
            case ActionState.RunJump:
            case ActionState.Flip:
                return;
        }
        if (!crouchKey){return;}
        float vel = (controller.Velocity.x * controller.Velocity.x + controller.Velocity.z * controller.Velocity.z);
        if(Mathf.Sqrt(vel) > 0.1f&&controller.GetCurrentState() != ActionState.Crawling||
            controller.GetCurrentState() == ActionState.Idle||!controller.InputFlag&&horizontalinput == 0 && verticalInput == 0)
        {
            crouchToCrawing = false;
        }
        else
        {
            crouchToCrawing = true;
        }
        if ((horizontalinput != 0 || verticalInput != 0)&&crouchToCrawing)
        {
            controller.ChangeMotionState(ActionState.Crawling);
        }

    }

    public void GlideJump()
    {
        if (controller.GetTag() != DataTag.RatchetAndClank)                                                         {return;}
        if(!jumpHoldKey || controller.Landing || controller.GetTimer().Timer_JumpNoInput.IsEnabled() ||
            (controller.JumpCount < controller.GetScriptableObject().MaxJumpCount && !controller.LengthyJumpFlag))  {return;}
        //滑空ジャンプ時の入力
        controller.ChangeMotionState(ActionState.GlideJump);
    }

   public void HipDropInput()
    {
        if (controller.GetTag() != DataTag.SuperMario)              {return;}
        if(controller.GetCurrentState() == ActionState.Grab ||
            controller.GetCurrentState() == ActionState.ClimbWall)  {return;}
        if (controller.Landing || !shiftKeyDown || hipDropFlag)     {return;}
        //ヒップドロップ入力
        hipDropFlag = true;
        controller.ChangeMotionState(ActionState.HipDrop);
    }
    public void JumpInput()
    {
        //値により入力受け付け
        switch (controller.GetCurrentState())
        {
            case ActionState.Crawling:
            case ActionState.ChangingDirectionJump:
            case ActionState.JumpAttack:
            case ActionState.Grab:
            case ActionState.ClimbWall:
                return;
        }
        if (controller.GetScriptableObject().MaxJumpCount == 0) { return; }
        if (mousemiddleClick && controller.CurrentBattleFlag)   { return; }
        //キーの入力時間でジャンプ力を決める処理
        HoldPuchJumpKey();
        //通常のジャンプ入力
        switch (controller.GetTag())
        {
            case DataTag.RatchetAndClank:
                DubleJumpInput();
                break;
            case DataTag.SuperMario:
                ThreeJumpInput();
                break;
        }
    }

    private void HoldPuchJumpKey()
    {
        if (controller.Landing && jumpDownKey)
        {
            controller.JumpingFlag = true;
        }
        if (!controller.JumpingFlag) { return; }
        if (jumpUpKey || controller.JumpTime >= controller.GetMaxJumpTime())
        {
            controller.JumpingFlag = false;
            controller.JumpTime = 0;
        }
        else if (jumpHoldKey)
        {
            controller.JumpTime += Time.deltaTime;
        }
    }

    private void DubleJumpInput()
    {
        if (jumpDownKey && controller.Landing)
        {
            controller.JumpState = JumpState.FirstJump;
            switch (controller.GetCurrentState())
            {
                case ActionState.Run:
                    controller.ChangeMotionState(ActionState.RunJump);
                    break;
                default:
                    controller.ChangeMotionState(ActionState.Jump);
                    break;
            }
        }
        else if (jumpDownKey && !controller.Landing && controller.JumpCount > 0)
        {
            controller.JumpState = JumpState.SecondJump;
            controller.ChangeMotionState(ActionState.Jump);
        }
    }

    private void ThreeJumpInput()
    {
        if (crouchKey)              {return;}
        if (!jumpDownKey)           {return;}
        if (!controller.Landing)    {return;}
        //1回目のジャンプモーション処理
        switch (controller.JumpState)
        {
            case JumpState.Null:
                if(!controller.GetTimer().GetTimer_BurstJump().IsEnabled())
                {
                    controller.JumpState = JumpState.FirstJump;
                    controller.ChangeMotionState(ActionState.Jump);
                }
                break;
            case JumpState.FirstJump:
                if (controller.JumpCount == (int)JumpState.SecondJump &&
                    controller.GetTimer().GetTimer_BurstJump().IsEnabled())
                {
                    controller.JumpState = JumpState.SecondJump;
                    controller.ChangeMotionState(ActionState.Jump);
                }
                break;
            case JumpState.SecondJump:
                if (controller.JumpCount > (int)JumpState.SecondJump &&
                    controller.GetTimer().GetTimer_BurstJump().IsEnabled())
                {
                    controller.JumpState = JumpState.ThirdJump;
                    controller.ChangeMotionState(ActionState.Jump);
                }
                break;
        }
    }

    public void LongJumpInput()
    {
        if (controller.GetTag() != DataTag.SuperMario)  { return; }
        if (!crouchKey || !jumpDownKey)                 { return; }

        //幅跳び入力
        float vel = (controller.Velocity.x * controller.Velocity.x + controller.Velocity.z * controller.Velocity.z);
        if (controller.Landing&&Mathf.Sqrt(vel) > 0.1f&&verticalInput > 0)
        {
            controller.HighJumpFlag = true;
            controller.ChangeMotionState(ActionState.RangeJump);
        }
        //バク転入力
        else
        {
            controller.HighJumpFlag = true;
            controller.ChangeMotionState(ActionState.LongJump);
        }
    }

    public void ChangingDirectionJumpInput()
    {
        if (controller.GetScriptableObject().MaxJumpCount == 0)     {return;}
        if (mousemiddleClick && controller.CurrentBattleFlag)       {return;}
        if (controller.GetCurrentState() == ActionState.Crawling)   {return;}
        if (controller.GetTag() != DataTag.SuperMario)              {return;}
        if (crouchKey)                                              {return;}
        if (!jumpDownKey||!CheckDirectionInput())                   {return;}
        controller.ChangeMotionState(ActionState.ChangingDirectionJump);
    }

    private bool CheckDirectionInput()
    {
        if (!controller.Landing) { return false; }
        bool enabled = false;
        if (controller.GetPastState() != ActionState.Jump&& controller.GetPastState() != ActionState.RunJump&&
            (pastKeyVerInput > 0&&verticalInput < 0||
            pastKeyVerInput < 0 && verticalInput > 0||
            pastKeyHorInput > 0&&horizontalinput < 0||
            pastKeyHorInput < 0 && horizontalinput > 0))
        {
            enabled = true;
        }
        pastKeyHorInput = horizontalinput;
        pastKeyVerInput = verticalInput;

        return enabled;
    }

    public void AllAvoidInput()
    {
        switch (controller.GetTag())
        {
            case DataTag.Zelda:
                RollingInput();
                break;
            case DataTag.RatchetAndClank:
                FlipInput();
                break;
        }
    }

    private void RollingInput()
    {
        if (!shiftKeyDown) { return; }
        if (controller.GetCurrentState() == ActionState.Run || controller.GetTPSCamera().FocusModeFlag)
        {
            if (mousemiddleClick && controller.CurrentBattleFlag &&
                horizontalinput == 0 &&
                (verticalInput > 0 || verticalInput == 0))
            {
                return;
            }
            controller.ChangeMotionState(ActionState.Flip);
        }
    }

    private void FlipInput()
    {
        if (!controller.Landing)    {return;}
        if (!crouchKey)             {return;}
        if (AllDirectionMoveKey())
        {
            controller.ChangeMotionState(ActionState.Flip);
        }
    }

    public void CrouchInput()
    {
        if(!crouchKey)                              {return;}
        if (controller.GetTag() == DataTag.Zelda)   {return;}
        switch (controller.GetCurrentState())
        {
            case ActionState.Jump:
            case ActionState.Flip:
            case ActionState.Grab:
            case ActionState.ClimbWall:
                return;
        }
        if (!controller.Landing || controller.HighJumpFlag || crouchToCrawing) {return;}
        controller.ChangeMotionState(ActionState.Crouch);
    }

    public void JumpingAttack()
    {
        if (controller.GetTag() != DataTag.SuperMario) { return; }
        if (!mouseleftDownClick) { return; }
        //プレイヤーのスピードを計測
        Vector3 vel = controller.Velocity;
        float maxSpeed = vel.x*vel.x+vel.z*vel.z;
        if (Mathf.Sqrt(maxSpeed) >= controller.GetScriptableObject().MaxSpeed)
        {
            controller.JumpAttackFlag = true;
            controller.ChangeMotionState(ActionState.JumpAttack);
        }
    }

    public void ChangeMode()
    {
        if (controller.GetTag() != DataTag.Zelda) { return; }
        if (!leftCtrlKey || controller.GetCurrentState() != ActionState.Idle) { return; }
        controller.ChangeMotionState(ActionState.ModeChange);
    }

    public void GameClearInput()
    {
        controller.InputFlag = false;
        mouseleftDownClick = false;
        mouseleftHoldClick = false;
        mouseleftUpClick = false;
        mousemiddleClick = false;
        mouserightClick = false;
        horizontalinput = 0;
        verticalInput = 0;
        jumpUpKey = false;
        jumpDownKey = false;
        jumpHoldKey = false;
        crouchKey = false;
        shiftKeyDown = false;
        shiftHoldKey = false;
        leftCtrlKey = false;
    }
}
