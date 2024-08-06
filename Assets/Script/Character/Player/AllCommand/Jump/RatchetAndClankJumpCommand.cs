using UnityEditor;
using UnityEngine;
using static CharacterManager;

//ジャンプ関係の処理をまとめたクラス
public class RatchetAndClankJumpCommand : InterfaceJumpCommand
{
    private PlayerController controller = null;

    private const float maxKeepJumpPower = 2000;

    public RatchetAndClankJumpCommand(PlayerController _controller)
    {
        controller = _controller;
    }

    public void Execute(float _maxspeed)
    {
        if (!controller.GetStateInput().IsCrouchKey()&& !controller.GetStateInput().IsShiftHoldKey())
        {
            //プレイヤーの状態チェック
            switch (controller.GetCurrentState())
            {
                case ActionState.ChangingDirectionJump:
                case ActionState.JumpAttack:
                    return;
            }
            if (!controller.GetStateInput().IsShiftHoldKey() || controller.GetPastState() != ActionState.Crouch)
            {
                BaseJumpCommand(_maxspeed);
            }
        }
        else
        {
            if (controller.GetStateInput().JumpHoldKey && controller.GetStateInput().IsShiftHoldKey() &&
            controller.GetPastState() == ActionState.Run && controller.GetCurrentState() == ActionState.RunJump)
            {
                ForwardLengthyJumpCommand(_maxspeed);
            }
            else if (controller.GetStateInput().IsCrouchKey() && controller.GetStateInput().JumpDownKey)
            {
                UpLongJumpCommand(_maxspeed);
            }
        }
        JumpAccele();
    }

    private void ForwardLengthyJumpCommand(float _maxspeed)
    {
        if (controller.Landing && !controller.GetTimer().Timer_LongJump.IsEnabled())
        {
            controller.LengthyJumpFlag = true;
            controller.GetTimer().Timer_LongJump.StartTimer(controller.GetScriptableObject().MaxLongJumpCount);
            controller.JumpForce(controller.GetScriptableObject().FirstJumpPower * controller.GetScriptableObject().SecondJumpPower);
        }
        if (!controller.GetTimer().Timer_LongJump.IsEnabled())
        {
            controller.GetStateInput().JumpHoldKey = false;
        }
    }

    private void UpLongJumpCommand(float _maxspeed)
    {
        controller.HighJumpFlag = true;
        if (controller.LengthyJumpFlag || controller.GetStateInput().IsShiftHoldKey()) { return; }
        if (!controller.GetTimer().Timer_LongJump.IsEnabled() && controller.HighJumpFlag && controller.Landing)
        {
            controller.JumpForce(controller.GetScriptableObject().FirstJumpPower);
            controller.GetTimer().Timer_LongJump.StartTimer(controller.GetScriptableObject().MaxLongJumpCount);
            controller.GetTimer().Timer_LongJump.OnCompleted += () =>
            {
                controller.LengthyJumpFlag = false;
            };
            controller.GetTimer().Timer_JumpNoInput.StartTimer(controller.GetScriptableObject().MaxNoJumpInputCount);
            controller.HighJumpFlag = false;
            controller.Landing = false;
            controller.GetSEController().JumpSEPlay();
        }
    }

    private void BaseJumpCommand(float _maxspeed)
    {
        if (controller.HighJumpFlag || controller.GetStateInput().IsShiftHoldKey()) { return; }
        DoubleJump(_maxspeed);
    }

    private void DoubleJump(float _maxspeed)
    {
        if (controller.GetTimer().Timer_JumpNoInput.IsEnabled()) {  return; }
        if(controller.GetCurrentState() != ActionState.Jump && controller.GetCurrentState() != ActionState.RunJump) { return; }
        if (!controller.GetStateInput().JumpDownKey) {  return; }
        //通常のジャンプ処理(maxJumpCountの回数だけ可能)
        if (controller.JumpCount < controller.GetScriptableObject().MaxJumpCount)
        {
            float jumppower = controller.GetScriptableObject().FirstJumpPower;
            if (controller.JumpCount == 1)
            {
                jumppower *= controller.GetScriptableObject().SecondJumpPower;
            }
            //ジャンプ力加算
            controller.JumpForce(jumppower);
            controller.JumpCount++;
            controller.GetTimer().Timer_JumpNoInput.StartTimer(controller.GetScriptableObject().MaxNoJumpInputCount);
            //ジャンプ入力をfalseに
            controller.GetStateInput().JumpDownKey = false;
            //SE再生
            controller.GetSEController().JumpSEPlay();
            //着地をfalseに
            controller.Landing = false;
        }
    }

    private float InputSpeed()
    {
        float inputnumber = 0;
        inputnumber = controller.GetStateInput().Horizontalinput * controller.GetStateInput().Horizontalinput + controller.GetStateInput().VerticalInput * controller.GetStateInput().VerticalInput;
        inputnumber = Mathf.Sqrt(inputnumber);
        return inputnumber;
    }

    private void JumpAccele()
    {
        if (controller.JumpingPower >= maxKeepJumpPower)
        {
            return;
        }
        if (controller.GetTimer().Timer_LongJump.IsEnabled())
        {
            if (!controller.Landing)
            {
                //Space長押しのジャンプ
                if (controller.GetStateInput().JumpHoldKey && !controller.GetTimer().Timer_JumpNoInput.IsEnabled() &&
                    !controller.LengthyJumpFlag && controller.GetCurrentState() != ActionState.RangeJump)
                {
                    controller.KeepLongJump(controller.GetScriptableObject().KeepJumpForce);
                    controller.JumpingPower += controller.GetScriptableObject().KeepJumpForce;
                    controller.JumpCount = controller.GetScriptableObject().MaxJumpCount;
                }
                //ShiftとSpace同時押した時の前方向に進む処理
                else if (controller.LengthyJumpFlag)
                {
                    controller.MoveXZ(controller.transform.forward * 0.5f);
                }
            }
        }
        else if (!controller.Landing &&controller.GetCurrentState() == ActionState.LongJump)
        {
            controller.MoveXZ(-controller.transform.forward * 0.5f);
        }
    }
}
