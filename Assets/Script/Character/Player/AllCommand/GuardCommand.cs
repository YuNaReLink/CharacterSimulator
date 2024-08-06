using UnityEditor;
using static PlayerController;
using static CharacterManager;
using UnityEngine;

public class GuardCommand
{
    private PlayerController controller = null;
    public GuardCommand(PlayerController _controller)
    {
        controller = _controller;
    }
    public void Execute()
    {
        if (controller.GetTag() != DataTag.Zelda) { return; }
        if (!controller.Landing) { return; }
        //ÇµÇ·Ç™Ç›ñhå‰èåè
        bool sitguard = controller.GetStateInput().IsMouseRightClick() && !controller.GetTPSCamera().ResetFlag &&
                        !controller.GetTPSCamera().FocusModeFlag;
        //íçñ⁄ñhå‰èåè
        bool focusblock = controller.GetStateInput().IsMouseRightClick() &&
                          controller.GetTPSCamera().FocusModeFlag;
        if (sitguard)
        {
            controller.GetStateInput().BlockState = ShieldBlockState.SitBlock;
            //ÉKÅ[ÉhÉÇÅ[ÉVÉáÉìÇê›íË
            controller.ChangeMotionState(ActionState.Guard);
            if (controller.GetStateInput().IsMouseRightDownClick())
            {
                controller.GetSEController().ShieldSEPlay();
            }
        }
        else if (focusblock)
        {
            controller.GetStateInput().BlockState = ShieldBlockState.FocusBlock;
            if (controller.GetStateInput().IsMouseRightDownClick())
            {
                controller.GetSEController().ShieldSEPlay();
            }
        }
        else
        {
            controller.GetStateInput().BlockState = ShieldBlockState.Null;
        }
    }
}
