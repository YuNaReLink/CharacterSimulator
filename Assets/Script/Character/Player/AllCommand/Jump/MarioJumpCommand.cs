using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static CharacterManager;

public class MarioJumpCommand : InterfaceJumpCommand
{
    private PlayerController controller = null;

    private const float maxFirstJumpPower = 1000;

    private const float maxSecondJumpPower = 1500;

    private const float maxThirdJumpPower = 2000;

    public MarioJumpCommand(PlayerController _controller)
    {
        controller = _controller;
    }
    public void HoldJumpForce(float _jumppower)
    {
        float holdTime = Mathf.Clamp(controller.JumpTime, 0f, controller.GetMaxJumpTime());
        _jumppower = Mathf.Lerp(500, _jumppower, holdTime / controller.GetMaxJumpTime());
        controller.GetCharacterRb().AddForce(controller.transform.up * _jumppower, ForceMode.Impulse);
    }

    public void Execute(float _maxspeed)
    {
        GetUpAttackJump();
        if (controller.GetStateInput().GetHipDropFlag) { return; }
        if (!controller.GetStateInput().IsCrouchKey() && !controller.GetStateInput().IsShiftHoldKey())
        {
            //キーの入力時間でジャンプ量が変わる三段ジャンプ
            BurstJump();
            //方向を切り替える時のジャンプの処理
            ChangingDirectionJumpCommand();
        }
        else
        {
            if(controller.GetStateInput().IsCrouchKey() && controller.GetStateInput().JumpDownKey)
            {
                UpLongJumpCommand(_maxspeed);
                RangeJumpCommand(_maxspeed);
            }
        }
        JumpAccele();
    }
    private void GetUpAttackJump()
    {
        if(controller.GetCurrentState() != ActionState.JumpAttack || !controller.GetStateInput().JumpDownKey) { return; }
        controller.JumpAttackFlag = false;
        controller.JumpState = JumpState.ThirdJump;
        controller.ChangeMotionState(ActionState.Jump);
        controller.GetTimer().Timer_ForwardAccele.End();
        controller.JumpForce(1000f);
        controller.GetStateInput().JumpDownKey = false;
        controller.Landing = false;
    }

    private void BurstJump()
    {
        if(controller.GetCurrentState() != ActionState.Jump&&controller.GetCurrentState() != ActionState.RunJump) { return; }
        if(controller.GetCurrentState() == ActionState.Flip) { return; }
        if (!controller.JumpingFlag) { return; }
        //通常のジャンプ処理(maxJumpCountの回数だけ可能)
        float t = controller.JumpTime / controller.GetMaxJumpTime();
        if (t >= 1)
        {
            controller.JumpingFlag = false;
            controller.JumpTime = 0;
        }
        if (CheckMaxJumpingPower())
        {
            return;
        }
        if (controller.JumpCount <= controller.GetScriptableObject().MaxJumpCount)
        {
            float basejumppower = controller.GetScriptableObject().FirstJumpPower;
            switch (controller.JumpState)
            {
                case JumpState.SecondJump:
                    basejumppower *= controller.GetScriptableObject().SecondJumpPower;
                    break;
                case JumpState.ThirdJump:
                    basejumppower *= controller.GetScriptableObject().ThirdJumpPower;
                    break;
            }
            controller.JumpForce(basejumppower);
            controller.JumpingPower += basejumppower;
            HoldJumpForce(basejumppower);
            if (controller.GetStateInput().JumpDownKey)
            {
                controller.GetSEController().JumpSEPlay();
                controller.GetTimer().GetTimer_BurstJump().End();
                controller.BurstJumpFlag = false;
                controller.JumpCount++;
            }
            //着地をfalseに
            controller.Landing = false;
            //ジャンプ入力をfalseに
            controller.GetStateInput().JumpDownKey = false;
        }

    }

    private bool CheckMaxJumpingPower()
    {
        switch (controller.JumpState)
        {
            case JumpState.FirstJump:
                if (controller.JumpingPower >= maxFirstJumpPower)
                {
                    return true;
                }
                break;
            case JumpState.SecondJump:
                if (controller.JumpingPower >= maxSecondJumpPower)
                {
                    return true;
                }
                break;
            case JumpState.ThirdJump:
                if (controller.JumpingPower >= maxThirdJumpPower)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    private void ChangingDirectionJumpCommand()
    {
        if(controller.GetCurrentState() != ActionState.ChangingDirectionJump) { return; }
        if (controller.HighJumpFlag || controller.GetStateInput().IsShiftHoldKey()) { return; }
        if (controller.GetCurrentState() == ActionState.Flip || !controller.Landing) { return; }
        controller.JumpForce(maxSecondJumpPower);
        controller.GetTimer().Timer_JumpNoInput.StartTimer(controller.GetScriptableObject().MaxNoJumpInputCount);
        //着地をfalseに
        controller.Landing = false;
        //ジャンプ入力をfalseに
        controller.GetStateInput().JumpDownKey = false;
        controller.GetSEController().JumpSEPlay();
    }

    private void UpLongJumpCommand(float _maxspeed)
    {
        if(controller.GetCurrentState() != ActionState.LongJump) {  return; }
        if (!controller.Landing) { return; }
        controller.JumpForce(controller.GetScriptableObject().FlipJumpPower);
        controller.GetTimer().Timer_JumpNoInput.StartTimer(controller.GetScriptableObject().MaxNoJumpInputCount);
        controller.HighJumpFlag = false;
        controller.Landing = false;
        controller.GetSEController().JumpSEPlay();
    }

    private void RangeJumpCommand(float _maxspeed)
    {
        if (controller.GetCurrentState() != ActionState.RangeJump) { return; }
        if (controller.GetCurrentState() == ActionState.Crawling) { return; }
        if (!controller.Landing) { return; }
        controller.GetTimer().Timer_LongJump.StartTimer(1f);
        controller.JumpForce(controller.GetScriptableObject().FlipJumpPower);
        controller.GetTimer().Timer_JumpNoInput.StartTimer(controller.GetScriptableObject().MaxNoJumpInputCount);
        controller.HighJumpFlag = false;
        controller.Landing = false;
        controller.GetSEController().JumpSEPlay();
    }

    private void JumpAccele()
    {
        if (controller.GetTimer().Timer_LongJump.IsEnabled())
        {
            if (!controller.Landing)
            {
                if (controller.GetStateInput().JumpHoldKey && !controller.GetTimer().Timer_JumpNoInput.IsEnabled() &&
                    !controller.LengthyJumpFlag && controller.GetCurrentState() != ActionState.RangeJump)
                {
                    controller.KeepLongJump(controller.GetScriptableObject().KeepJumpForce);
                    controller.JumpingPower += controller.GetScriptableObject().KeepJumpForce;
                    controller.JumpCount = controller.GetScriptableObject().MaxJumpCount;
                }
                else if (controller.LengthyJumpFlag)
                {
                    controller.MoveXZ(controller.transform.forward * 5f);
                }
            }
        }
        else if (!controller.Landing &&
            (controller.GetCurrentState() == ActionState.ChangingDirectionJump || controller.GetCurrentState() == ActionState.LongJump))
        {
            controller.MoveXZ(-controller.transform.forward * 0.5f);
        }
    }
}
