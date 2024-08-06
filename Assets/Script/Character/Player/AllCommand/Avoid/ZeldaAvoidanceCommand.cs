using UnityEditor;
using UnityEngine;
using static PlayerController;
using static CharacterManager;

//���[�����O�A����A���]�A�O�]�A�o�N�]�Ȃǂ̏������܂Ƃ߂��N���X
public class ZeldaAvoidanceCommand : InterfaceAvoidanceCommand
{
    private PlayerController controller = null;
    public ZeldaAvoidanceCommand(PlayerController _controller)
    {
        controller = _controller;
    }
    public void Execute()
    {
        if (controller.CheckAvoidFlag() && !controller.GetTimer().Timer_Avoid.IsEnabled() && controller.Landing && controller.GetCurrentState() == ActionState.Flip)
        {
            //���ڑΏۂ�����Ȃ�
            if (controller.GetTPSCamera().FocusModeFlag && controller.GetStateInput().IsMouseMiddle())
            {
                //4�����ɑΉ��������[�����O�A�N�V��������
                RollingDirection();
            }
            else if (!controller.GetStateInput().IsMouseMiddle())
            {
                controller.AvoidFlag[(int)AvoidState.Up] = true;
                controller.GetTimer().Timer_Avoid.StartTimer(controller.GetScriptableObject().ForwardAcceleTimerCount);
                //����Ȃ�������O�ɂ������[�����O�A�N�V��������
                controller.ForwardAccele(controller.transform.forward * controller.GetScriptableObject().RollingPower);
                if (controller.GetStateInput().ShiftKeyDown)
                {
                    controller.GetSEController().RollingSEPlay();
                }
            }
        }
    }

    private void RollingDirection()
    {
        if (controller.GetStateInput().Horizontalinput < 0)
        {
            controller.GetTimer().Timer_Avoid.StartTimer(controller.GetScriptableObject().HorizontalTimerCount);
            controller.AvoidFlag[(int)AvoidState.Left] = true;
            controller.ForwardAccele(-controller.transform.right * controller.GetScriptableObject().RollingPower);
            controller.GetSEController().RollingSEPlay();
        }
        else if (controller.GetStateInput().Horizontalinput > 0)
        {
            controller.GetTimer().Timer_Avoid.StartTimer(controller.GetScriptableObject().HorizontalTimerCount);
            controller.AvoidFlag[(int)AvoidState.Right] = true;
            controller.ForwardAccele(controller.transform.right * controller.GetScriptableObject().RollingPower);
            controller.GetSEController().RollingSEPlay();
        }
        else if (!controller.GetTimer().Timer_Avoid.IsEnabled() && controller.GetStateInput().VerticalInput < 0)
        {
            controller.GetTimer().Timer_Avoid.StartTimer(controller.GetScriptableObject().BackAcceleTimerCount);
            controller.AvoidFlag[(int)AvoidState.Down] = true;
            controller.JumpForce(controller.GetScriptableObject().FlipJumpPower);
            controller.RollingForwardAccele(controller.transform.forward * -controller.GetScriptableObject().RollingPower);
            //���n��false��
            controller.Landing = false;
            controller.GetStateInput().ShiftKeyDown = false;
            controller.ForwardAccele(-controller.transform.forward * controller.GetScriptableObject().RollingPower);
            controller.GetSEController().FlipSEPlay();
        }
        else
        {
            controller.GetTimer().Timer_Avoid.StartTimer(controller.GetScriptableObject().ForwardAcceleTimerCount);
            controller.AvoidFlag[(int)AvoidState.Up] = true;
            controller.ForwardAccele(controller.transform.forward * controller.GetScriptableObject().RollingPower);
            controller.GetSEController().RollingSEPlay();
        }
    }
}
