using UnityEngine;
using static CharacterManager;
using static Config;


//プレイヤーのモーション処理をまとめたクラス
public class PlayerMotionController
{
    private PlayerController controller = null;
    public PlayerMotionController(PlayerController _controller)
    {
        controller = _controller;
    }
    public string SetMotion(ActionState _state)
    {
        string name = null;
        switch (_state)
        {
            case ActionState.Idle:
                if ((controller.GetTPSCamera().ResetFlag && controller.GetStateInput().IsMouseRightClick()) ||
                    (!controller.GetTPSCamera().ResetFlag && controller.GetStateInput().IsMouseMiddle()))
                {
                    name = "shieldBlockIdle";
                }
                else if (controller.CurrentBattleFlag)
                {
                    name = "battleIdle";
                }
                else
                {
                    name = "Idle";
                }
                break;
            case ActionState.Falling:
                name = "fallingIdle";
                break;
            case ActionState.Run:
                if (controller.GetStateInput().IsMouseMiddle())
                {
                    name = SelectRunMotion();
                }
                else if (controller.CurrentBattleFlag)
                {
                    name = "battleRun";
                }
                else
                {
                    name = "run_com";
                }
                break;
            case ActionState.Jump:
                switch (controller.GetTag())
                {
                    case DataTag.RatchetAndClank:
                        if (controller.JumpCount <= 0)
                        {
                            name = "battlejump";
                        }
                        else
                        {
                            name = "secondJump";
                        }
                        break;
                    case DataTag.SuperMario:
                        name = SelectJumpMotion();
                        break;
                }
                break;
            case ActionState.RunJump:
                switch (controller.GetTag())
                {
                    case DataTag.RatchetAndClank:
                        name = "battlerunjump";
                        break;
                    case DataTag.SuperMario:
                        name = SelectJumpMotion();
                        break;
                }
                break;
            case ActionState.GlideJump:
                name = "glideJump";
                break;
            case ActionState.LongJump:
                name = "backFlip";
                break;
            case ActionState.RangeJump:
                name = "highJump";
                break;
            case ActionState.ChangingDirectionJump:
                name = "sideFlip";
                break;
            case ActionState.HipDrop:
                name = "hipDrop";
                break;
            case ActionState.Crouch:
                name = "crouching";
                break;
            case ActionState.Crawling:
                name = "proneForward";
                break;
            case ActionState.Flip:
                if (controller.GetTag() == DataTag.Zelda)
                {
                    if (controller.GetStateInput().IsMouseMiddle())
                    {
                        name = SelectRollingMotion();
                    }
                    else
                    {
                        name = "rolling";
                    }
                }
                else if (controller.GetTag()== DataTag.RatchetAndClank)
                {
                    name = SelectFlipMotion();
                }
                break;
            case ActionState.Attack:
                switch (controller.AttackState)
                {
                    case AttackStateCount.FirstAttack:
                        name = "slash1";
                        break;
                    case AttackStateCount.SecondAttack:
                        name = "slash2";
                        break;
                    case AttackStateCount.ThirdAttack:
                        name = "slash3";
                        break;
                }
                break;
            case ActionState.JumpAttack:
                if (controller.GetTag() != DataTag.SuperMario)
                {
                    name = "jumpAttack";
                }
                else
                {
                    name = "jumpingAttack";
                }
                break;
            case ActionState.ReadySpinAttack:
                name = "readySpinAttack";
                break;
            case ActionState.SpinAttack:
                name = "spinAttack";
                break;
            case ActionState.CrouchAttack:
                if (controller.GetTag() == DataTag.Zelda)
                {
                    name = "shieldSlach";
                }
                else if (controller.GetTag() == DataTag.SuperMario)
                {
                    name = "roundhousekick";
                }
                break;
            case ActionState.ModeChange:
                name = "weaponSwitch";
                controller.CurrentBattleFlag = !controller.CurrentBattleFlag;
                break;
            case ActionState.Guard:
                name = "shieldBlockSitIdle";
                break;
            case ActionState.Grab:
                name = "hangingIdle";
                break;
            case ActionState.ClimbWall:
                name = "hangToCrouch";
                break;
            case ActionState.Damage:
                name = "damage";
                break;
            case ActionState.Die:
                name = "death";
                break;
            case ActionState.VictoryPose:
                name = "gameClear";
                break;
        }
        return name;
    }

    private string SelectRunMotion()
    {
        string name = null;
        PlayerInput playerInput = controller.GetStateInput();
        if (playerInput.Horizontalinput < 0)
        {
            name = "focusLeftMove";
        }
        else if (playerInput.Horizontalinput > 0)
        {
            name = "focusRightMove";
        }
        else if (playerInput.VerticalInput < 0)
        {
            name = "backRun";
        }
        else if (playerInput.VerticalInput > 0)
        {
            name = "focusForwardRun";
        }
        return name;
    }
    private string SelectJumpMotion()
    {
        string name = null;
        switch (controller.JumpState)
        {
            case JumpState.FirstJump:
                name = "jump_com";
                break;
            case JumpState.SecondJump:
                name = "secondJumpVer2";
                break;
            case JumpState.ThirdJump:
                name = "secondJump";
                break;
        }
        return name;
    }
    //入力キーによりローリングモーションを選択する
    private string SelectRollingMotion()
    {
        string name = null;
        PlayerInput playerInput = controller.GetStateInput();
        if (playerInput.Horizontalinput < 0)
        {
            name = "leftRolling";
        }
        else if (playerInput.Horizontalinput > 0)
        {
            name = "rightRolling";
        }
        else if (playerInput.VerticalInput < 0)
        {
            name = "backFlip";
        }
        else if (playerInput.VerticalInput > 0 || playerInput.VerticalInput == 0)
        {
            name = "rolling";
        }
        return name;
    }
    private string SelectFlipMotion()
    {
        string name = null;
        PlayerInput playerInput = controller.GetStateInput();
        if (playerInput.Horizontalinput < 0)
        {
            name = "leftSideFlip";
        }
        else if (playerInput.Horizontalinput > 0)
        {
            name = "rightSideFlip";
        }
        else if (playerInput.VerticalInput < 0)
        {
            name = "backFlip";
        }
        return name;
    }


    public void InputEndMotion(ActionState state)
    {
        //アニメーションが終了したら
        if (controller.GetAnim().GetCurrentAnimatorStateInfo(0).normalizedTime < endMotionNormalizedTime){return;}
        //モーションが終わった時に行うモーション設定
        switch (state)
        {
            case ActionState.Run:
                for (int i = 0; i < (int)AvoidState.DataEnd; i++)
                {
                    controller.AvoidFlag[i] = false;
                }
                if (!controller.InputFlag && !controller.GetTimer().Timer_BurstAttack.IsEnabled() &&
                    controller.GetCurrentState() != ActionState.ModeChange)
                {
                    controller.ChangeMotionState(ActionState.Idle);
                }
                break;
            case ActionState.Flip:
                controller.GetStateMovement().StopVelocityXZ();
                for (int i = 0; i < (int)AvoidState.DataEnd; i++)
                {
                    controller.AvoidFlag[i] = false;
                }
                if (!controller.GetTimer().Timer_BurstAttack.IsEnabled() &&
                    controller.GetCurrentState() != ActionState.ModeChange)
                {
                    controller.ChangeMotionState(ActionState.Idle);
                }
                break;
            case ActionState.Attack:
                if (controller.GetTimer().Timer_ForwardAccele.IsEnabled()) { return; }
                controller.GetTimer().Timer_BurstAttack.End();
                controller.AttackState = AttackStateCount.Null;
                controller.ChangeMotionState(ActionState.Idle);
                break;
            case ActionState.JumpAttack:
                if (!JumpAttackEndEnabled()) { return; }
                controller.ChangeMotionState(ActionState.Idle);
                controller.JumpAttackFlag = false;
                controller.GetTimer().Timer_ForwardAccele.End();
                break;
            case ActionState.SpinAttack:
                if (!controller.GetTimer().Timer_SpinAttack.IsEnabled())
                {
                    controller.ChangeMotionState(ActionState.Idle);
                    controller.GetTimer().Timer_SpinAttack.End();
                }
                break;
            case ActionState.RunJump:
            case ActionState.Jump:
            case ActionState.LongJump:
            case ActionState.RangeJump:
            case ActionState.GlideJump:
            case ActionState.ChangingDirectionJump:
                if (controller.Landing)
                {
                    controller.ChangeMotionState(ActionState.Idle);
                }
                break;
            case ActionState.HipDrop:
                if (controller.Landing && !controller.GetTimer().Timer_StopMove.IsEnabled())
                {
                    controller.ChangeMotionState(ActionState.Idle);
                }
                break;
            case ActionState.ModeChange:
                controller.ChangeMotionState(ActionState.Idle);
                break;
            case ActionState.Guard:
                if (controller.GetStateInput().BlockState == ShieldBlockState.Null)
                {
                    controller.ChangeMotionState(ActionState.Idle);
                }
                break;
            case ActionState.Damage:
                controller.ChangeMotionState(ActionState.Idle);
                controller.MoveStopFlag = false;
                break;
            case ActionState.VictoryPose:
                controller.ChangeMotionState(ActionState.Idle);
                break;
        }
    }

    private bool JumpAttackEndEnabled()
    {
        bool flag1 = controller.GetTimer().Timer_ForwardAccele.IsEnabled()&&controller.GetTag() == DataTag.Zelda;
        bool flag2 = controller.Velocity != Vector3.zero&& controller.GetTag() == DataTag.SuperMario;
        if (flag1||flag2)
        {
            return false;
        }
        return true;
    }


    public void MotionPause()
    {
        if (AnimationStopPattern())
        {
            controller.GetAnim().enabled = false;
        }
        else
        {
            controller.GetAnim().enabled = true;
        }
    }

    private float stopMotionNormalizedTime = 0.5f;

    private bool AnimationStopPattern()
    {
        switch (controller.GetCurrentState())
        {
            case ActionState.JumpAttack:
                if (controller.GetAnim().GetCurrentAnimatorStateInfo(0).normalizedTime >= stopMotionNormalizedTime && !controller.Landing &&
                   controller.GetAnim().GetCurrentAnimatorStateInfo(0).IsName("jumpAttack"))
                {
                    return true;
                }
                break;
        }
        return false;
    }
}
