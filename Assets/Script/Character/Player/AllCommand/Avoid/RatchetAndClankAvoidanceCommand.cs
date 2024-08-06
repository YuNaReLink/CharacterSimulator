using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;

public class RatchetAndClankAvoidanceCommand : InterfaceAvoidanceCommand
{
    private PlayerController controller = null;
    public RatchetAndClankAvoidanceCommand(PlayerController _controller)
    {
        controller = _controller;
    }

    public void Execute()
    {
        if (controller.Landing && controller.GetCurrentState() == ActionState.Flip)
        {
            FlipDirection();
            AutoAccele();
        }
    }

    private void FlipDirection()
    {
        if (!controller.CheckAvoidFlag()) { return; }
        if (controller.GetTimer().Timer_Avoid.IsEnabled()) { return; }
        if (controller.GetStateInput().IsLeftKey())
        {
            controller.GetTimer().Timer_Avoid.StartTimer(controller.GetScriptableObject().HorizontalTimerCount);
            controller.AvoidFlag[(int)AvoidState.Left] = true;
            controller.JumpForce(controller.GetScriptableObject().FlipJumpPower);
            //’…’n‚ðfalse‚É
            controller.Landing = false;
            controller.GetSEController().FlipSEPlay();
        }
        else if (controller.GetStateInput().IsRightKey())
        {
            controller.GetTimer().Timer_Avoid.StartTimer(controller.GetScriptableObject().HorizontalTimerCount);
            controller.AvoidFlag[(int)AvoidState.Right] = true;
            controller.JumpForce(controller.GetScriptableObject().FlipJumpPower);
            //’…’n‚ðfalse‚É
            controller.Landing = false;
            controller.GetSEController().FlipSEPlay();
        }
        else if (controller.GetStateInput().IsDownKey())
        {
            controller.GetTimer().Timer_Avoid.StartTimer(controller.GetScriptableObject().BackAcceleTimerCount);
            controller.AvoidFlag[(int)AvoidState.Down] = true;
            controller.JumpForce(controller.GetScriptableObject().FlipJumpPower);
            //’…’n‚ðfalse‚É
            controller.Landing = false;
            controller.GetSEController().FlipSEPlay();
        }
    }

    private void AutoAccele()
    {
        if (controller.CheckAvoidFlag()) { return; }
        if (!controller.GetTimer().Timer_Avoid.IsEnabled()) { return; }
        if (controller.Landing) { return; }
        if (controller.AvoidFlag[(int)AvoidState.Left])
        {
            controller.MoveXZ(-controller.transform.right * controller.GetScriptableObject().RollingPower);
        }
        else if (controller.AvoidFlag[(int)AvoidState.Right])
        {
            controller.MoveXZ(controller.transform.right * controller.GetScriptableObject().RollingPower);
        }
        else if (controller.AvoidFlag[(int)AvoidState.Up])
        {
            controller.MoveXZ(controller.transform.forward * controller.GetScriptableObject().RollingPower);
        }
        else if (controller.AvoidFlag[(int)AvoidState.Down])
        {
            controller.MoveXZ(-controller.transform.forward * controller.GetScriptableObject().RollingPower);
        }
    }
}
